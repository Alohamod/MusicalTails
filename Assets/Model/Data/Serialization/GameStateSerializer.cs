using System.IO;
using UnityEngine;

public abstract class GameStateSerializer
{
    public abstract SerializationFormat Format { get; }
    public abstract string FileName { get; }

    public string GetFullPath() => Path.Combine(Application.persistentDataPath, FileName);

    public abstract LeaderboardCollection Load(string path);
    public abstract void Save(string path, LeaderboardCollection data);

    public void EnsureFileExists()
    {
        string path = GetFullPath();
        if (!File.Exists(path))
            Save(path, new LeaderboardCollection());
    }

    public LeaderboardCollection LoadOrCreate()
    {
        string path = GetFullPath();
        if (!File.Exists(path))
        {
            var empty = new LeaderboardCollection();
            Save(path, empty);
            return empty;
        }
        return Load(path);
    }
}
