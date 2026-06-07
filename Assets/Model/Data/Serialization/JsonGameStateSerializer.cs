using System.IO;
using UnityEngine;

public class JsonGameStateSerializer : GameStateSerializer
{
    public override SerializationFormat Format => SerializationFormat.Json;
    public override string FileName => "leaderboard.json";

    public override LeaderboardCollection Load(string path)
    {
        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<LeaderboardCollection>(json) ?? new LeaderboardCollection();
        data.NormalizeLegacyDifficultyBoards();
        return data;
    }

    public override void Save(string path, LeaderboardCollection data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }
}
