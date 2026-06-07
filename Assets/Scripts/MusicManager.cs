using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;

[System.Serializable]
public class TrackSettings
{
    public AudioClip clip;
    public float bpm;
}

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public ButtonSpawner spawner;
    public List<TrackSettings> trackLibrary;

    private List<NoteEvent> chart = new List<NoteEvent>();
    public float spawnDelay = 3f;
    private int _nextNoteIndex = 0;
    public float bpmMultiplier = 1.05f;
    public float delayReduction = 0.05f;
    private float _originalBpm;
    private float _currentBpm;
    private bool _isPlaying = false;
    private bool _runEnded = false;
    public float difficultyFactor = 1.0f;
    private float _baseSpawnDelay;
    private float _baseScrollSpeed;
    public float growthRate = 0.005f;
    private float _baseShrinkSpeed = 5.0f;
    public MusicalTails.Model.Core.InputHandler inputHandler = new();

    public event Action<int> OnGameOver;
    public event Action<int> OnSongComplete;

    public void RegisterHit(int points)
    {
        int multiplier = GetLaneMultiplier();
        inputHandler.AddScore(points * multiplier);
    }

    int GetLaneMultiplier()
    {
        if (GameSession.Instance != null)
            return GameSession.Instance.laneCount;
        if (LaneManager.Instance != null)
            return LaneManager.Instance.laneCount;
        return 4;
    }

    void Start()
    {
        if (GameSession.Instance == null || !GameSession.Instance.HasSelectedTrack)
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
            return;
        }

        Time.timeScale = 1f;
        GameplayScoreSaver.ResetForNewRun();
        difficultyFactor = 1.0f;
        _baseSpawnDelay = 3.0f;
        _baseScrollSpeed = 5.0f;
        ButtonController.globalScrollSpeed = _baseScrollSpeed;
        ButtonController.globalShrinkSpeed = 5.0f;

        ApplySessionSettings();

        if (musicSource.clip == null) return;

        _originalBpm = ResolveBpm(musicSource.clip);
        _currentBpm = _originalBpm;
        float beatInterval = 60f / _originalBpm;

        var chartGen = GetComponent<ChartGenerator>();
        if (chartGen != null)
            chartGen.GenerateAndSave(musicSource);

        string path = Application.persistentDataPath + "/chart.json";
        if (File.Exists(path))
            chart = JsonUtility.FromJson<ChartWrapper>(File.ReadAllText(path)).notes;

        Invoke(nameof(StartMusic), 1.0f);
        spawnDelay = beatInterval * 4.0f;
        _baseSpawnDelay = spawnDelay;
        _baseScrollSpeed = ButtonController.globalScrollSpeed;
    }

    void ApplySessionSettings()
    {
        if (GameSession.Instance == null || !GameSession.Instance.HasSelectedTrack) return;

        var selected = GameSession.Instance.SelectedTrack;
        musicSource.clip = selected.clip;
        musicSource.playOnAwake = false;

        if (LaneManager.Instance != null)
            LaneManager.Instance.laneCount = GameSession.Instance.laneCount;

        var bg = FindAnyObjectByType<GameBackground>();
        if (bg != null)
            bg.laneCount = GameSession.Instance.laneCount;
    }

    float ResolveBpm(AudioClip clip)
    {
        if (GameSession.Instance != null && GameSession.Instance.HasSelectedTrack)
            return GameSession.Instance.SelectedTrack.bpm;

        foreach (var track in trackLibrary)
        {
            if (track.clip == clip)
                return track.bpm;
        }
        return 120f;
    }

    void StartMusic()
    {
        musicSource.Play();
        _isPlaying = true;
    }

    void Update()
    {
        if (!_isPlaying || _runEnded || musicSource == null) return;

        if (musicSource.isPlaying)
        {
            if (musicSource.time >= 60.0f)
            {
                difficultyFactor += growthRate * Time.deltaTime;
                ApplyDifficultySmoothly();
            }

            float currentTime = musicSource.time;
            while (_nextNoteIndex < chart.Count)
            {
                float targetTime = chart[_nextNoteIndex].time;

                if (currentTime > 0.05f && targetTime >= currentTime && currentTime + spawnDelay >= targetTime)
                {
                    spawner.SpawnSpecificButton(chart[_nextNoteIndex]);
                    _nextNoteIndex++;
                }
                else if (targetTime < currentTime)
                {
                    _nextNoteIndex++;
                }
                else
                {
                    break;
                }
            }
        }

        if (_nextNoteIndex >= chart.Count &&
            FindObjectsByType<ButtonController>(FindObjectsSortMode.None).Length == 0)
        {
            CompleteSong();
        }
    }

    void ApplyDifficultySmoothly()
    {
        musicSource.pitch = Mathf.Clamp(difficultyFactor, 1.0f, 2.0f);
        spawnDelay = _baseSpawnDelay / difficultyFactor;
        ButtonController.globalScrollSpeed = _baseScrollSpeed * difficultyFactor;
        ButtonController.globalShrinkSpeed = _baseShrinkSpeed * difficultyFactor;
    }

    void OnEnable()
    {
        ButtonController.OnNoteMissed += HandleGameOver;
    }

    void OnDisable()
    {
        ButtonController.OnNoteMissed -= HandleGameOver;
    }

    void OnApplicationQuit() => GameplayScoreSaver.TrySave(inputHandler.TotalScore);

    void OnDestroy() => GameplayScoreSaver.TrySave(inputHandler.TotalScore);

    void HandleGameOver()
    {
        EndGame("Промах!");
    }

    public void TriggerGameOver(string reason)
    {
        EndGame(reason);
    }

    void CompleteSong()
    {
        if (!_isPlaying || _runEnded) return;

        _runEnded = true;
        _isPlaying = false;
        if (musicSource.isPlaying)
            musicSource.Stop();
        Time.timeScale = 0;
        Debug.Log("SONG COMPLETE. Score: " + inputHandler.TotalScore);
        OnSongComplete?.Invoke(inputHandler.TotalScore);
    }

    void EndGame(string reason)
    {
        if (!_isPlaying || _runEnded) return;

        _runEnded = true;
        _isPlaying = false;
        musicSource.Stop();
        Time.timeScale = 0;
        Debug.Log("GAME OVER: " + reason);
        OnGameOver?.Invoke(inputHandler.TotalScore);
    }
}
