using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUIController : MonoBehaviour
{
    GameObject _gameOverPanel;
    GameObject _completePanel;
    TextMeshProUGUI _gameOverText;
    TextMeshProUGUI _completeText;
    MusicManager _musicManager;

    void Start()
    {
        Time.timeScale = 1f;
        _musicManager = FindAnyObjectByType<MusicManager>();
        OsuStyleUI.EnsureEventSystem();
        BuildUI();

        if (_musicManager != null)
        {
            _musicManager.OnGameOver += ShowGameOver;
            _musicManager.OnSongComplete += ShowSongComplete;
        }
    }

    void OnDestroy()
    {
        if (_musicManager != null)
        {
            _musicManager.OnGameOver -= ShowGameOver;
            _musicManager.OnSongComplete -= ShowSongComplete;
        }
    }

    void BuildUI()
    {
        var existingCanvas = FindAnyObjectByType<Canvas>();
        Transform parent = existingCanvas != null
            ? existingCanvas.transform
            : OsuStyleUI.CreateCanvas().transform;

        OsuStyleUI.CreateCornerButtonStack(
            parent,
            new Vector2(1f, 1f),
            ("В меню", ReturnToMenu),
            ("Выход", GameQuit.Quit)
        );

        _gameOverPanel = OsuStyleUI.CreateDialogPanel("GameOverPanel", parent, new Vector2(560, 300));
        _gameOverPanel.SetActive(false);

        _gameOverText = OsuStyleUI.CreateDialogText("Вы проиграли!", _gameOverPanel.transform, 130f);

        OsuStyleUI.CreateDialogButtonRow(
            _gameOverPanel.transform,
            ("В меню", ReturnToMenu)
        );

        _completePanel = OsuStyleUI.CreateDialogPanel("CompletePanel", parent, new Vector2(560, 300));
        _completePanel.SetActive(false);

        _completeText = OsuStyleUI.CreateDialogText("Трек пройден!", _completePanel.transform, 130f);

        OsuStyleUI.CreateDialogButtonRow(
            _completePanel.transform,
            ("В меню", ReturnToMenu)
        );
    }

    void ShowGameOver(int finalScore)
    {
        _gameOverPanel.SetActive(true);
        _gameOverText.text = $"Вы проиграли!\n\nСчёт: {finalScore}";
        GameplayScoreSaver.TrySave(finalScore);
    }

    void ShowSongComplete(int finalScore)
    {
        _completePanel.SetActive(true);
        _completeText.text = $"Трек пройден!\n\nСчёт: {finalScore}";
        GameplayScoreSaver.TrySave(finalScore);
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        if (_musicManager != null)
            GameplayScoreSaver.TrySave(_musicManager.inputHandler.TotalScore);
        if (GameSession.Instance != null)
            GameSession.Instance.StopPreviewMusic();
        SceneManager.LoadScene(SceneNames.MainMenu);
    }
}
