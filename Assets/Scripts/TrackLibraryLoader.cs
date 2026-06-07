using System.Collections.Generic;
using UnityEngine;

public static class TrackLibraryLoader
{
    static readonly Dictionary<string, float> KnownBpm = new()
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

    public static List<TrackSettings> MergeTracks(List<TrackSettings> primary, List<TrackSettings> fallback)
    {
        if (primary != null && primary.Count > 0)
            return new List<TrackSettings>(primary);

        if (fallback != null && fallback.Count > 0)
            return new List<TrackSettings>(fallback);

        return LoadFromResources();
    }

    static List<TrackSettings> LoadFromResources()
    {
        var tracks = new List<TrackSettings>();
        var clips = Resources.LoadAll<AudioClip>("Audio");
        foreach (var clip in clips)
        {
            if (clip == null) continue;
            tracks.Add(new TrackSettings { clip = clip, bpm = GuessBpm(clip.name) });
        }
        return tracks;
    }

    static float GuessBpm(string clipName)
    {
        foreach (var kv in KnownBpm)
        {
            if (clipName.Contains(kv.Key))
                return kv.Value;
        }
        return 120f;
    }
}
