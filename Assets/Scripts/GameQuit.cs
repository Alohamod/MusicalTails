using UnityEngine;

public static class GameQuit
{
    public static void Quit()
    {
        Time.timeScale = 1f;
        GameplayScoreSaver.TrySaveActiveGameplay();

        if (GameSession.Instance != null)
            GameSession.Instance.StopPreviewMusic();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
