using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public List<TrackSettings> trackLibrary = new();
    public int selectedTrackIndex = -1;
    public int laneCount = 4;
    public SerializationFormat saveFormat = SerializationFormat.Json;

    public AudioSource previewAudioSource;

    public bool HasSelectedTrack => selectedTrackIndex >= 0 && selectedTrackIndex < trackLibrary.Count;

    public TrackSettings SelectedTrack =>
        HasSelectedTrack ? trackLibrary[selectedTrackIndex] : null;

    public string SelectedTrackId =>
        HasSelectedTrack ? trackLibrary[selectedTrackIndex].clip.name : string.Empty;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (previewAudioSource == null)
        {
            previewAudioSource = gameObject.AddComponent<AudioSource>();
            previewAudioSource.playOnAwake = false;
            previewAudioSource.loop = true;
        }
    }

    public void ClearTrackSelection()
    {
        selectedTrackIndex = -1;
        StopPreviewMusic();
    }

    public void SelectTrack(int index)
    {
        if (index < 0 || index >= trackLibrary.Count) return;

        selectedTrackIndex = index;
        var clip = trackLibrary[index].clip;
        if (clip == null) return;

        previewAudioSource.Stop();
        previewAudioSource.clip = clip;
        previewAudioSource.Play();
    }

    public void StopPreviewMusic()
    {
        if (previewAudioSource != null && previewAudioSource.isPlaying)
            previewAudioSource.Stop();
    }

    public void ResetGameplayState()
    {
        Time.timeScale = 1f;
    }
}
