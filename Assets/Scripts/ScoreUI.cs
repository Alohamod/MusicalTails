using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public MusicManager musicManager;

    void Start()
    {
        musicManager.inputHandler.OnScoreChanged += UpdateScoreText;
    }

    void UpdateScoreText(int newScore)
    {
        int lanes = GameSession.Instance != null ? GameSession.Instance.laneCount : 4;
        scoreText.text = $"Score: {newScore}  (x{lanes})";
    }

    void OnDestroy()
    {
        musicManager.inputHandler.OnScoreChanged -= UpdateScoreText;
    }
}