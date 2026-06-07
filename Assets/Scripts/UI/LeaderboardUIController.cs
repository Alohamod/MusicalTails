using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardUIController : MonoBehaviour
{
    Transform _entriesContainer;
    TextMeshProUGUI _headerLabel;

    void Start()
    {
        Time.timeScale = 1f;
        OsuStyleUI.EnsureEventSystem();
        BuildUI();
        RefreshTable();
    }

    void BuildUI()
    {
        var canvas = OsuStyleUI.CreateCanvas();
        OsuStyleUI.CreateBackground(canvas.transform);
        OsuStyleUI.CreateTitle("Таблица результатов", canvas.transform, 52f);

        _headerLabel = OsuStyleUI.CreateLabel(
            "",
            canvas.transform,
            new Vector2(0.5f, 0.8f),
            new Vector2(900, 48)
        );
        _headerLabel.alignment = TextAlignmentOptions.Center;
        _headerLabel.fontSize = 22f;
        _headerLabel.enableWordWrapping = true;
        _headerLabel.overflowMode = TextOverflowModes.Ellipsis;

        var panel = OsuStyleUI.CreatePanel("LeaderboardPanel", canvas.transform);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(760, 400);
        panelRect.anchoredPosition = new Vector2(0, 20);

        var headerRow = OsuStyleUI.CreateLabel(
            "#     Счёт              Дата",
            panel.transform,
            new Vector2(0.5f, 0.92f),
            new Vector2(700, 36)
        );
        headerRow.fontStyle = FontStyles.Bold;
        headerRow.color = OsuStyleUI.AccentPink;
        headerRow.fontSize = 22f;

        var containerGo = new GameObject("Entries", typeof(RectTransform));
        containerGo.transform.SetParent(panel.transform, false);
        _entriesContainer = containerGo.transform;
        var containerRect = containerGo.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(700, 320);
        containerRect.anchoredPosition = new Vector2(0, -24);

        var entriesLayout = containerGo.AddComponent<VerticalLayoutGroup>();
        entriesLayout.spacing = 4;
        entriesLayout.childAlignment = TextAnchor.UpperCenter;
        entriesLayout.childControlWidth = true;
        entriesLayout.childControlHeight = true;
        entriesLayout.childForceExpandWidth = true;
        entriesLayout.childForceExpandHeight = false;

        OsuStyleUI.CreateBottomButtonBar(
            canvas.transform,
            ("В меню", () => SceneManager.LoadScene(SceneNames.MainMenu)),
            ("Выход", GameQuit.Quit)
        );
    }

    void RefreshTable()
    {
        if (GameSession.Instance == null || !GameSession.Instance.HasSelectedTrack)
        {
            _headerLabel.text = "Трек не выбран";
            return;
        }

        var track = GameSession.Instance.SelectedTrack;
        string trackName = track.clip.name;
        if (trackName.Length > 50) trackName = trackName[..47] + "...";
        _headerLabel.text = trackName;

        foreach (Transform child in _entriesContainer)
            Destroy(child.gameObject);

        List<ScoreEntry> scores = LeaderboardService.Instance != null
            ? LeaderboardService.Instance.GetTopScores(track.clip.name)
            : new List<ScoreEntry>();

        for (int i = 0; i < 10; i++)
        {
            string rowText = i < scores.Count
                ? $"{i + 1,2}.  {scores[i].score,8}   {scores[i].date}"
                : $"{i + 1,2}.  —";

            var rowGo = new GameObject($"Row_{i}", typeof(RectTransform));
            rowGo.transform.SetParent(_entriesContainer, false);
            rowGo.AddComponent<LayoutElement>().preferredHeight = 30;

            var row = rowGo.AddComponent<TextMeshProUGUI>();
            row.text = rowText;
            row.fontSize = 20f;
            row.alignment = TextAlignmentOptions.MidlineLeft;
            row.color = i < 3 ? OsuStyleUI.AccentCyan : Color.white;
            row.raycastTarget = false;
        }
    }
}
