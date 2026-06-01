using UnityEngine;
using System.Collections.Generic;
using System.IO;

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
    private float _startTime;
    private bool _isPlaying = false;
    public float difficultyFactor = 1.0f;
    private float _baseSpawnDelay;
    private float _baseScrollSpeed;
    public float growthRate = 0.005f;
    private float _baseShrinkSpeed = 5.0f;
    public MusicalTails.Model.Core.InputHandler inputHandler = new();
    public void RegisterHit(int points)
    {
        inputHandler.AddScore(points);
    }

    void Start()
    {
        difficultyFactor = 1.0f;
        _baseSpawnDelay = 3.0f;
        _baseScrollSpeed = 5.0f;
        ButtonController.globalScrollSpeed = _baseScrollSpeed;
        ButtonController.globalShrinkSpeed = 5.0f;
        if (musicSource.clip == null) return;
        _originalBpm = 120f;
        foreach (var track in trackLibrary)
        {
            if (track.clip == musicSource.clip)
            {
                _originalBpm = track.bpm;
                break;
            }
        }
        _currentBpm = _originalBpm;
        float beatInterval = 60f / _originalBpm;
        GetComponent<ChartGenerator>().GenerateAndSave(musicSource);

        string path = Application.persistentDataPath + "/chart.json";
        if (File.Exists(path))
        {
            chart = JsonUtility.FromJson<ChartWrapper>(File.ReadAllText(path)).notes;

        }
        Invoke("StartMusic", 1.0f);
        spawnDelay = beatInterval * 4.0f;
        _baseSpawnDelay = spawnDelay;
        _baseScrollSpeed = ButtonController.globalScrollSpeed;

    }
    void StartMusic()
    {
        musicSource.Play();
        _startTime = Time.time;
        _isPlaying = true;
    }

    void Update()
    {
        if (!_isPlaying || musicSource == null || !musicSource.isPlaying) return;

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

    void HandleGameOver()
    {
        if (!_isPlaying) return;

        _isPlaying = false;
        musicSource.Stop();
        Debug.Log("Игра окончена: нота пропущена!");
    }
    public void TriggerGameOver(string reason)
    {
        if (!_isPlaying) return;

        _isPlaying = false;
        musicSource.Stop();
        Debug.Log("GAME OVER: " + reason);
        Time.timeScale = 0;
    }

}