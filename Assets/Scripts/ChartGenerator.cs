using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

[Serializable]
public class NoteEvent { public float time; public int lane; public bool isLong; public int typeID; }

[Serializable]
public class ChartWrapper { public List<NoteEvent> notes; }

public class ChartGenerator : MonoBehaviour
{
    public List<TrackSettings> trackLibrary;

    public void GenerateAndSave(AudioSource musicSource)
    {
        if (musicSource.clip == null) return;

        float bpm = 120f;
        if (GameSession.Instance != null && GameSession.Instance.HasSelectedTrack)
            bpm = GameSession.Instance.SelectedTrack.bpm;
        else
        {
            foreach (var t in trackLibrary)
            {
                if (t.clip == musicSource.clip) { bpm = t.bpm; break; }
            }
        }

        List<NoteEvent> notes = new List<NoteEvent>();
        float beatInterval = 60f / bpm;
        int totalBeats = Mathf.FloorToInt(musicSource.clip.length / beatInterval);

        int lastLane = -1;

        for (int i = 0; i < totalBeats; i++)
        {
            float time = i * beatInterval;
            bool isIntro = i < 16;
            if (isIntro && notes.Count > 0 && (time - notes[notes.Count - 1].time) < beatInterval * 4)
                continue;
            if (time < 1.0f) continue;

            int laneCount = LaneManager.Instance != null ? LaneManager.Instance.laneCount : 4;
            int newLane = GetRandomLane(lastLane, laneCount);
            lastLane = newLane;

            int type = 0;
            float rand = UnityEngine.Random.value;
            float progress = (float)i / totalBeats;

            if (rand < progress * 0.1f) type = 3;
            else if (rand < progress * 0.2f + 0.1f) type = 2;
            else if (UnityEngine.Random.value < 0.2f) type = 1;

            notes.Add(new NoteEvent
            {
                time = time,
                lane = newLane,
                isLong = (type == 1),
                typeID = type
            });
        }

        string json = JsonUtility.ToJson(new ChartWrapper { notes = notes }, true);
        File.WriteAllText(Application.persistentDataPath + "/chart.json", json);
        Debug.Log("???? ????????????: " + notes.Count + " ???.");
    }

    private int GetRandomLane(int lastLane, int laneCount)
    {
        if (laneCount <= 1) return 0;
        int newLane;
        do
        {
            newLane = UnityEngine.Random.Range(0, laneCount);
        } while (newLane == lastLane);
        return newLane;
    }
}
