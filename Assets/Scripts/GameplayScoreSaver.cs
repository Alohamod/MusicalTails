using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameplayScoreSaver
{
    static bool _savedThisRun;

    public static void ResetForNewRun() => _savedThisRun = false;

    public static void TrySave(int score)
    {
        if (_savedThisRun || score <= 0) return;
        if (GameSession.Instance == null || !GameSession.Instance.HasSelectedTrack) return;
        if (LeaderboardService.Instance == null) return;

        LeaderboardService.Instance.AddScore(GameSession.Instance.SelectedTrackId, score);
        _savedThisRun = true;
    }

    public static void TrySaveActiveGameplay()
    {
        if (!IsGameplayScene()) return;

        var musicManager = Object.FindAnyObjectByType<MusicManager>();
        if (musicManager == null) return;

        TrySave(musicManager.inputHandler.TotalScore);
    }

    static bool IsGameplayScene() =>
        SceneManager.GetActiveScene().name == SceneNames.Gameplay;
}
