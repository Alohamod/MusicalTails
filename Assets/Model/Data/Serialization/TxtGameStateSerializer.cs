using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

public class TxtGameStateSerializer : GameStateSerializer
{
    public override SerializationFormat Format => SerializationFormat.Txt;
    public override string FileName => "leaderboard.txt";

    public override LeaderboardCollection Load(string path)
    {
        var collection = new LeaderboardCollection();
        if (!File.Exists(path)) return collection;

        LeaderboardBoard currentBoard = null;
        foreach (string rawLine in File.ReadAllLines(path))
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line.StartsWith("BOARD\t"))
            {
                string[] parts = line.Split('\t');
                if (parts.Length < 2 || string.IsNullOrEmpty(parts[1])) continue;

                currentBoard = new LeaderboardBoard { trackId = parts[1] };
                if (parts.Length >= 3 && int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int legacyDifficulty))
                    currentBoard.difficulty = legacyDifficulty;

                collection.boards.Add(currentBoard);
                continue;
            }

            if (line.StartsWith("ENTRY\t") && currentBoard != null)
            {
                string[] parts = line.Split('\t');
                if (parts.Length < 3) continue;

                currentBoard.entries.Add(new ScoreEntry
                {
                    score = int.Parse(parts[1], CultureInfo.InvariantCulture),
                    date = parts[2]
                });
            }
        }

        collection.NormalizeLegacyDifficultyBoards();
        return collection;
    }

    public override void Save(string path, LeaderboardCollection data)
    {
        var sb = new StringBuilder();
        foreach (var board in data.boards)
        {
            sb.Append("BOARD\t").Append(board.trackId).AppendLine();
            foreach (var entry in board.entries)
                sb.Append("ENTRY\t").Append(entry.score).Append('\t').Append(entry.date).AppendLine();
            sb.AppendLine();
        }

        File.WriteAllText(path, sb.ToString());
    }
}
