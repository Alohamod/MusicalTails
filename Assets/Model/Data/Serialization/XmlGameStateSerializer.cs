using System.IO;
using System.Xml.Serialization;

public class XmlGameStateSerializer : GameStateSerializer
{
    public override SerializationFormat Format => SerializationFormat.Xml;
    public override string FileName => "leaderboard.xml";

    public override LeaderboardCollection Load(string path)
    {
        using var reader = new StreamReader(path);
        var serializer = new XmlSerializer(typeof(LeaderboardCollection));
        var data = (LeaderboardCollection)serializer.Deserialize(reader) ?? new LeaderboardCollection();
        data.NormalizeLegacyDifficultyBoards();
        return data;
    }

    public override void Save(string path, LeaderboardCollection data)
    {
        using var writer = new StreamWriter(path);
        var serializer = new XmlSerializer(typeof(LeaderboardCollection));
        serializer.Serialize(writer, data);
    }
}
