using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    const string TrackPlaceholder = "Выберите трек";

    [SerializeField] List<TrackSettings> defaultTracks = new();
    [SerializeField] InputActionAsset uiInputActions;

    TextMeshProUGUI _trackInfoLabel;
    Button _startButton;
    TMP_Dropdown _trackDropdown;
    TMP_Dropdown _difficultyDropdown;
    TMP_Dropdown _formatDropdown;

    void Start()
    {
        Time.timeScale = 1f;
        if (uiInputActions != null)
            UIInputActions.Asset = uiInputActions;
        OsuStyleUI.EnsureEventSystem();
        EnsureServices();
        BuildUI();
    }

    void EnsureServices()
    {
        if (GameSession.Instance == null)
        {
            var sessionGo = new GameObject("GameSession");
            sessionGo.AddComponent<GameSession>();
        }

        GameSession.Instance.trackLibrary = TrackLibraryLoader.MergeTracks(defaultTracks, GameSession.Instance.trackLibrary);

        if (LeaderboardService.Instance == null)
        {
            var lbGo = new GameObject("LeaderboardService");
            lbGo.AddComponent<LeaderboardService>();
        }

        LeaderboardService.Instance.Initialize(GameSession.Instance.saveFormat);
    }

    void BuildUI()
    {
        var canvas = OsuStyleUI.CreateCanvas();
        OsuStyleUI.CreateBackground(canvas.transform);
        OsuStyleUI.CreateTitle("MusicalTails", canvas.transform);

        var panel = OsuStyleUI.CreateMenuPanel(canvas.transform);

        _trackDropdown = OsuStyleUI.CreateDropdownRow("Трек", panel);
        PopulateTrackDropdown();

        _trackInfoLabel = OsuStyleUI.CreateInfoLabel(GetTrackInfoText(), panel);

        _difficultyDropdown = OsuStyleUI.CreateDropdownRow("Сложность (дорожки 2–8)", panel);
        PopulateDifficultyDropdown();

        _formatDropdown = OsuStyleUI.CreateDropdownRow("Формат сохранения", panel);
        PopulateFormatDropdown();

        var leaderboardBtn = OsuStyleUI.CreateMenuButton("Таблица результатов", panel);
        leaderboardBtn.onClick.AddListener(OpenLeaderboard);

        _startButton = OsuStyleUI.CreateMenuButton("Начать игру", panel);
        _startButton.onClick.AddListener(StartGame);

        var quitBtn = OsuStyleUI.CreateMenuButton("Выход", panel, 52f);
        quitBtn.onClick.AddListener(GameQuit.Quit);

        _trackDropdown.onValueChanged.AddListener(OnTrackChanged);
        _difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        _formatDropdown.onValueChanged.AddListener(OnFormatChanged);

        UpdateStartButtonState();
    }

    void PopulateTrackDropdown()
    {
        _trackDropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData> { new(TrackPlaceholder) };

        foreach (var track in GameSession.Instance.trackLibrary)
        {
            string name = track.clip != null ? track.clip.name : "Трек";
            if (name.Length > 40) name = name[..37] + "...";
            options.Add(new TMP_Dropdown.OptionData(name));
        }

        _trackDropdown.AddOptions(options);
        ResizeDropdownTemplate(_trackDropdown, options.Count);
        _trackDropdown.SetValueWithoutNotify(0);
        _trackDropdown.RefreshShownValue();
    }

    void PopulateDifficultyDropdown()
    {
        _difficultyDropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>();
        for (int i = 2; i <= 8; i++)
            options.Add(new TMP_Dropdown.OptionData(i.ToString()));

        _difficultyDropdown.AddOptions(options);
        ResizeDropdownTemplate(_difficultyDropdown, options.Count);
        _difficultyDropdown.SetValueWithoutNotify(Mathf.Clamp(GameSession.Instance.laneCount - 2, 0, options.Count - 1));
        _difficultyDropdown.RefreshShownValue();
    }

    void PopulateFormatDropdown()
    {
        _formatDropdown.ClearOptions();
        _formatDropdown.AddOptions(new List<TMP_Dropdown.OptionData>
        {
            new("JSON"),
            new("XML"),
            new("TXT")
        });
        ResizeDropdownTemplate(_formatDropdown, 3);
        _formatDropdown.SetValueWithoutNotify(FormatToIndex(GameSession.Instance.saveFormat));
        _formatDropdown.RefreshShownValue();
    }

    static void ResizeDropdownTemplate(TMP_Dropdown dropdown, int itemCount)
    {
        if (dropdown.template == null) return;
        float height = Mathf.Clamp(itemCount * 42f + 16f, 140f, 360f);
        dropdown.template.sizeDelta = new Vector2(0, height);
    }

    string GetTrackInfoText()
    {
        if (!GameSession.Instance.HasSelectedTrack)
            return "Музыка начнёт играть после выбора трека";

        var track = GameSession.Instance.SelectedTrack;
        return $"BPM: {track.bpm}";
    }

    void OnTrackChanged(int index)
    {
        if (index <= 0)
            GameSession.Instance.ClearTrackSelection();
        else
            GameSession.Instance.SelectTrack(index - 1);

        _trackInfoLabel.text = GetTrackInfoText();
        UpdateStartButtonState();
    }

    void OnDifficultyChanged(int index) => GameSession.Instance.laneCount = index + 2;

    void OnFormatChanged(int index)
    {
        var newFormat = IndexToFormat(index);
        LeaderboardService.Instance.MigrateFormat(newFormat);
        GameSession.Instance.saveFormat = newFormat;
    }

    static int FormatToIndex(SerializationFormat format) => format switch
    {
        SerializationFormat.Json => 0,
        SerializationFormat.Xml => 1,
        SerializationFormat.Txt => 2,
        _ => 0
    };

    static SerializationFormat IndexToFormat(int index) => index switch
    {
        0 => SerializationFormat.Json,
        1 => SerializationFormat.Xml,
        2 => SerializationFormat.Txt,
        _ => SerializationFormat.Json
    };

    void UpdateStartButtonState()
    {
        _startButton.interactable = true;
        var text = _startButton.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null) return;

        text.text = GameSession.Instance.HasSelectedTrack
            ? "Начать игру"
            : "Начать игру";
    }

    void OpenLeaderboard()
    {
        if (!GameSession.Instance.HasSelectedTrack)
        {
            _trackInfoLabel.text = "Сначала выберите трек!";
            return;
        }

        SceneManager.LoadScene(SceneNames.Leaderboard);
    }

    void StartGame()
    {
        if (!GameSession.Instance.HasSelectedTrack)
        {
            _trackInfoLabel.text = "Сначала выберите трек!";
            return;
        }

        GameSession.Instance.StopPreviewMusic();
        GameSession.Instance.ResetGameplayState();
        SceneManager.LoadScene(SceneNames.Gameplay);
    }
}
