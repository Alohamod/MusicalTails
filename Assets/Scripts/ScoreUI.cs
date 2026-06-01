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
        scoreText.text = $"Score: {newScore}";
    }

    void OnDestroy()
    {
        musicManager.inputHandler.OnScoreChanged -= UpdateScoreText;
    }
}