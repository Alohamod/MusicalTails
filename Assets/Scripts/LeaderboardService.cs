using System.Collections.Generic;
using UnityEngine;

public class LeaderboardService : MonoBehaviour
{
    public static LeaderboardService Instance { get; private set; }

    private readonly Dictionary<SerializationFormat, GameStateSerializer> _serializers = new()
    {
        { SerializationFormat.Json, new JsonGameStateSerializer() },
        { SerializationFormat.Xml, new XmlGameStateSerializer() },
        { SerializationFormat.Txt, new TxtGameStateSerializer() }
    };

    private LeaderboardCollection _data;
    private SerializationFormat _currentFormat;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(SerializationFormat format)
    {
        _currentFormat = format;
        EnsureAllFormatsExist();
        _data = GetSerializer(format).LoadOrCreate();
        _data.NormalizeLegacyDifficultyBoards();
        Save();
    }

    public SerializationFormat CurrentFormat => _currentFormat;

    public void MigrateFormat(SerializationFormat newFormat)
    {
        if (newFormat == _currentFormat) return;

        var oldSerializer = GetSerializer(_currentFormat);
        var newSerializer = GetSerializer(newFormat);

        LeaderboardCollection data;
        if (System.IO.File.Exists(oldSerializer.GetFullPath()))
            data = oldSerializer.Load(oldSerializer.GetFullPath());
        else
            data = new LeaderboardCollection();

        data.NormalizeLegacyDifficultyBoards();
        newSerializer.Save(newSerializer.GetFullPath(), data);

        _currentFormat = newFormat;
        _data = data;

        if (GameSession.Instance != null)
            GameSession.Instance.saveFormat = newFormat;
    }

    public void AddScore(string trackId, int score)
    {
        if (_data == null) Initialize(_currentFormat);
        _data.AddScore(trackId, score);
        Save();
    }

    public List<ScoreEntry> GetTopScores(string trackId)
    {
        if (_data == null) Initialize(_currentFormat);
        return _data.GetTopScores(trackId);
    }

    public void Save()
    {
        GetSerializer(_currentFormat).Save(GetSerializer(_currentFormat).GetFullPath(), _data);
    }

    private GameStateSerializer GetSerializer(SerializationFormat format) => _serializers[format];

    private void EnsureAllFormatsExist()
    {
        foreach (var serializer in _serializers.Values)
            serializer.EnsureFileExists();
    }
}
