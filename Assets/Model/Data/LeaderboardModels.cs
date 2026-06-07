using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ScoreEntry
{
    public int score;
    public string date;

    public ScoreEntry() { }

    public ScoreEntry(int score)
    {
        this.score = score;
        date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}

[Serializable]
public class LeaderboardBoard
{
    public string trackId;
    public int difficulty;
    public List<ScoreEntry> entries = new();
}

[Serializable]
public class LeaderboardCollection
{
    public List<LeaderboardBoard> boards = new();

    public void NormalizeLegacyDifficultyBoards()
    {
        var merged = new Dictionary<string, LeaderboardBoard>();

        foreach (var board in boards)
        {
            if (string.IsNullOrEmpty(board.trackId)) continue;

            if (!merged.TryGetValue(board.trackId, out var target))
            {
                target = new LeaderboardBoard { trackId = board.trackId };
                merged[board.trackId] = target;
            }

            if (board.entries != null)
                target.entries.AddRange(board.entries);
        }

        boards = merged.Values.ToList();
        foreach (var board in boards)
        {
            board.difficulty = 0;
            board.entries = board.entries
                .OrderByDescending(e => e.score)
                .Take(10)
                .ToList();
        }
    }

    public LeaderboardBoard GetOrCreateBoard(string trackId)
    {
        var board = boards.FirstOrDefault(b => b.trackId == trackId);
        if (board != null) return board;

        board = new LeaderboardBoard { trackId = trackId };
        boards.Add(board);
        return board;
    }

    public void AddScore(string trackId, int score, int maxEntries = 10)
    {
        var board = GetOrCreateBoard(trackId);
        board.entries.Add(new ScoreEntry(score));
        board.entries = board.entries
            .OrderByDescending(e => e.score)
            .Take(maxEntries)
            .ToList();
    }

    public List<ScoreEntry> GetTopScores(string trackId, int maxEntries = 10)
    {
        var board = boards.FirstOrDefault(b => b.trackId == trackId);
        if (board == null) return new List<ScoreEntry>();

        return board.entries
            .OrderByDescending(e => e.score)
            .Take(maxEntries)
            .ToList();
    }
}
