#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MusicalTailsSceneSetup
{
    const string MainMenuPath = "Assets/Scenes/MainMenu.unity";
    const string LeaderboardPath = "Assets/Scenes/Leaderboard.unity";
    const string GameplayPath = "Assets/Scenes/SampleScene.unity";

    [MenuItem("MusicalTails/Setup All Scenes")]
    public static void SetupAllScenes()
    {
        var tracks = LoadTracksFromGameplayOrAudio();
        CreateMainMenuScene(tracks);
        CreateLeaderboardScene();
        EnsureGameplayUI();
        UpdateBuildSettings();
        AssetDatabase.SaveAssets();
        Debug.Log("MusicalTails: сцены настроены. MainMenu — стартовая сцена.");
    }

    [MenuItem("MusicalTails/Setup All Scenes", true)]
    public static bool SetupAllScenesValidate() => !Application.isPlaying;

    static List<TrackSettings> LoadTracksFromGameplayOrAudio()
    {
        var tracks = new List<TrackSettings>();

        if (File.Exists(GameplayPath))
        {
            var gameplayScene = EditorSceneManager.OpenScene(GameplayPath, OpenSceneMode.Single);
            var musicManager = Object.FindAnyObjectByType<MusicManager>();
            if (musicManager != null && musicManager.trackLibrary.Count > 0)
            {
                tracks.AddRange(musicManager.trackLibrary);
                return tracks;
            }
        }

        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null) continue;
            tracks.Add(new TrackSettings { clip = clip, bpm = GuessBpm(clip.name) });
        }

        return tracks;
    }

    static float GuessBpm(string clipName)
    {
        var known = new Dictionary<string, float>
        {
            { "Kai_Angel_-_KHOLOD", 146f },
            { "Face_-_FOREVER_YOUNG", 230f },
            { "MORGENSHTERN - ICE", 152f },
            { "CODE80_ALLME_-_ADRENACHROME", 160f },
            { "THRILL_PILL_-_Milliony", 150f },
            { "Egor_Krid_-_Serdceedka", 120f },
            { "Code10 - Love Smoke", 100f },
            { "Boulevard_Depo_-_JUNKfood", 102f }
        };

        foreach (var kv in known)
        {
            if (clipName.Contains(kv.Key) || clipName.StartsWith(kv.Key))
                return kv.Value;
        }
        return 120f;
    }

    static void CreateMainMenuScene(List<TrackSettings> tracks)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var camera = Camera.main;
        if (camera != null)
        {
            camera.backgroundColor = new Color(0.08f, 0.06f, 0.14f);
            camera.orthographic = true;
        }

        var bootstrap = new GameObject("MainMenu");
        var controller = bootstrap.AddComponent<MainMenuController>();

        var so = new SerializedObject(controller);
        var tracksProp = so.FindProperty("defaultTracks");
        tracksProp.ClearArray();
        for (int i = 0; i < tracks.Count; i++)
        {
            tracksProp.InsertArrayElementAtIndex(i);
            var element = tracksProp.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("clip").objectReferenceValue = tracks[i].clip;
            element.FindPropertyRelative("bpm").floatValue = tracks[i].bpm;
        }
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, MainMenuPath);
    }

    static void CreateLeaderboardScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var camera = Camera.main;
        if (camera != null)
            camera.backgroundColor = new Color(0.08f, 0.06f, 0.14f);

        var bootstrap = new GameObject("Leaderboard");
        bootstrap.AddComponent<LeaderboardUIController>();

        EditorSceneManager.SaveScene(scene, LeaderboardPath);
    }

    static void EnsureGameplayUI()
    {
        var scene = EditorSceneManager.OpenScene(GameplayPath, OpenSceneMode.Single);

        if (Object.FindAnyObjectByType<GameplayUIController>() == null)
        {
            var uiGo = new GameObject("GameplayUI");
            uiGo.AddComponent<GameplayUIController>();
        }

        var musicManager = Object.FindAnyObjectByType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.musicSource.playOnAwake = false;
            EditorUtility.SetDirty(musicManager.musicSource);
        }

        EditorSceneManager.SaveScene(scene);
    }

    static void UpdateBuildSettings()
    {
        var scenes = new[]
        {
            new EditorBuildSettingsScene(MainMenuPath, true),
            new EditorBuildSettingsScene(LeaderboardPath, true),
            new EditorBuildSettingsScene(GameplayPath, true)
        };
        EditorBuildSettings.scenes = scenes;
    }
}
#endif
