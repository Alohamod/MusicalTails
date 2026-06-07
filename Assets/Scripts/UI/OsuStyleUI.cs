using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class OsuStyleUI
{
    public static readonly Color Background = new(0.08f, 0.06f, 0.14f, 1f);
    public static readonly Color Panel = new(0.12f, 0.09f, 0.2f, 0.95f);
    public static readonly Color AccentPink = new(1f, 0.4f, 0.67f, 1f);
    public static readonly Color AccentCyan = new(0.4f, 0.8f, 1f, 1f);
    public static readonly Color ButtonNormal = new(0.18f, 0.14f, 0.28f, 1f);
    public static readonly Color ButtonHighlight = new(0.28f, 0.2f, 0.42f, 1f);

    public static Canvas CreateCanvas(string name = "Canvas")
    {
        var canvasGo = new GameObject(name, typeof(RectTransform));
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    public static void EnsureEventSystem()
    {
        var eventSystem = Object.FindAnyObjectByType<EventSystem>();
        GameObject eventSystemGo;

        if (eventSystem == null)
        {
            eventSystemGo = new GameObject("EventSystem");
            eventSystem = eventSystemGo.AddComponent<EventSystem>();
        }
        else
        {
            eventSystemGo = eventSystem.gameObject;
        }

        var legacyModule = eventSystemGo.GetComponent<StandaloneInputModule>();
        if (legacyModule != null)
            Object.Destroy(legacyModule);

        var inputModule = eventSystemGo.GetComponent<InputSystemUIInputModule>();
        if (inputModule == null)
            inputModule = eventSystemGo.AddComponent<InputSystemUIInputModule>();

        if (inputModule.actionsAsset == null && UIInputActions.Asset != null)
            inputModule.actionsAsset = UIInputActions.Asset;
    }

    public static Image CreateBackground(Transform parent)
    {
        var go = CreateUIObject("Background", parent);
        StretchFull(go.GetComponent<RectTransform>());
        var image = go.AddComponent<Image>();
        image.color = Background;
        image.raycastTarget = false;
        return image;
    }

    public static TextMeshProUGUI CreateTitle(string text, Transform parent, float fontSize = 64f)
    {
        var go = CreateUIObject("Title", parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0, -24);
        rect.sizeDelta = new Vector2(1000, 90);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = AccentPink;
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 36f;
        tmp.fontSizeMax = fontSize;
        return tmp;
    }

    public static RectTransform CreateMenuPanel(Transform parent)
    {
        var go = CreateUIObject("MenuPanel", parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(720, 640);
        rect.anchoredPosition = new Vector2(0, -30);

        var image = go.AddComponent<Image>();
        image.color = new Color(Panel.r, Panel.g, Panel.b, 0.92f);
        image.raycastTarget = false;

        var layout = go.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 12;
        layout.padding = new RectOffset(28, 28, 24, 24);
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        return rect;
    }

    public static TMP_Dropdown CreateDropdownRow(string labelText, Transform parent, float dropdownHeight = 48f)
    {
        var row = CreateUIObject("Row_" + labelText, parent);
        var rowLayout = row.AddComponent<LayoutElement>();
        rowLayout.preferredHeight = dropdownHeight + 30;

        var vlg = row.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        var labelGo = CreateUIObject("Label", row.transform);
        labelGo.AddComponent<LayoutElement>().preferredHeight = 26;
        var label = labelGo.AddComponent<TextMeshProUGUI>();
        label.text = labelText;
        label.fontSize = 22f;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.color = AccentCyan;
        label.raycastTarget = false;

        var dropdownGo = CreateUIObject("DropdownHost", row.transform);
        var dropdownHost = dropdownGo.AddComponent<LayoutElement>();
        dropdownHost.preferredHeight = dropdownHeight;
        dropdownHost.minHeight = dropdownHeight;

        return CreateDropdown(dropdownGo.transform);
    }

    public static TextMeshProUGUI CreateInfoLabel(string text, Transform parent)
    {
        var go = CreateUIObject("InfoLabel", parent);
        go.AddComponent<LayoutElement>().preferredHeight = 44;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 20f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 1f, 1f, 0.8f);
        tmp.raycastTarget = false;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        return tmp;
    }

    public static Button CreateMenuButton(string text, Transform parent, float height = 58f)
    {
        var go = CreateUIObject("Button_" + text, parent);
        go.AddComponent<LayoutElement>().preferredHeight = height;

        var image = go.AddComponent<Image>();
        image.color = ButtonNormal;

        var button = go.AddComponent<Button>();
        ApplyButtonColors(button);

        var textGo = CreateUIObject("Text", go.transform);
        var textRect = textGo.GetComponent<RectTransform>();
        StretchWithPadding(textRect, 12, 8);

        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        ApplyButtonTextStyle(tmp, 26f);
        return button;
    }

    public static RectTransform CreateCornerButtonStack(Transform parent, Vector2 anchor, params (string text, UnityEngine.Events.UnityAction action)[] buttons)
    {
        var stack = CreateUIObject("CornerButtons", parent);
        var rect = stack.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-16, -16);
        rect.sizeDelta = new Vector2(200, buttons.Length * 52 + (buttons.Length - 1) * 8);

        var layout = stack.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.UpperRight;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        foreach (var (text, action) in buttons)
        {
            var btn = CreateStackedButton(text, stack.transform, 46f);
            btn.onClick.AddListener(action);
        }

        return rect;
    }

    public static RectTransform CreateBottomButtonBar(Transform parent, params (string text, UnityEngine.Events.UnityAction action)[] buttons)
    {
        var bar = CreateUIObject("BottomBar", parent);
        var rect = bar.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0, 32);
        rect.sizeDelta = new Vector2(720, 64);

        var layout = bar.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 16;
        layout.padding = new RectOffset(8, 8, 0, 0);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        foreach (var (text, action) in buttons)
        {
            var btn = CreateStackedButton(text, bar.transform, 58f);
            btn.onClick.AddListener(action);
        }

        return rect;
    }

    public static GameObject CreateDialogPanel(string name, Transform parent, Vector2 size)
    {
        var go = CreateUIObject(name, parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;

        var image = go.AddComponent<Image>();
        image.color = Panel;

        var layout = go.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 16;
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        return go;
    }

    public static TextMeshProUGUI CreateLabel(string text, Transform parent, Vector2 anchor, Vector2 size)
    {
        var go = CreateUIObject("Label", parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24f;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.color = AccentCyan;
        tmp.raycastTarget = false;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        return tmp;
    }

    public static TextMeshProUGUI CreateDialogText(string text, Transform parent, float height = 120f)
    {
        var go = CreateUIObject("DialogText", parent);
        go.AddComponent<LayoutElement>().preferredHeight = height;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 32f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = AccentPink;
        tmp.raycastTarget = false;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    public static RectTransform CreateDialogButtonRow(Transform parent, params (string text, UnityEngine.Events.UnityAction action)[] buttons)
    {
        var row = CreateUIObject("DialogButtons", parent);
        row.AddComponent<LayoutElement>().preferredHeight = 58;
        var layout = row.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 16;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        foreach (var (text, action) in buttons)
        {
            var btn = CreateStackedButton(text, row.transform, 54f);
            btn.onClick.AddListener(action);
        }

        return row.GetComponent<RectTransform>();
    }

    public static Button CreateButton(string text, Transform parent, Vector2 anchor, Vector2 size)
    {
        var go = CreateUIObject("Button_" + text, parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;

        var image = go.AddComponent<Image>();
        image.color = ButtonNormal;

        var button = go.AddComponent<Button>();
        ApplyButtonColors(button);

        var textGo = CreateUIObject("Text", go.transform);
        StretchWithPadding(textGo.GetComponent<RectTransform>(), 10, 6);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        ApplyButtonTextStyle(tmp, 24f);
        return button;
    }

    public static GameObject CreatePanel(string name, Transform parent)
    {
        var go = CreateUIObject(name, parent);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(700, 500);

        var image = go.AddComponent<Image>();
        image.color = Panel;
        return go;
    }

    static Button CreateStackedButton(string text, Transform parent, float height)
    {
        var go = CreateUIObject("Button_" + text, parent);
        go.AddComponent<LayoutElement>().preferredHeight = height;

        var image = go.AddComponent<Image>();
        image.color = ButtonNormal;

        var button = go.AddComponent<Button>();
        ApplyButtonColors(button);

        var textGo = CreateUIObject("Text", go.transform);
        StretchWithPadding(textGo.GetComponent<RectTransform>(), 10, 6);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        ApplyButtonTextStyle(tmp, 24f);
        return button;
    }

    static void ApplyButtonColors(Button button)
    {
        var colors = button.colors;
        colors.normalColor = ButtonNormal;
        colors.highlightedColor = ButtonHighlight;
        colors.pressedColor = AccentPink;
        colors.selectedColor = ButtonHighlight;
        button.colors = colors;
    }

    static void ApplyButtonTextStyle(TextMeshProUGUI tmp, float maxFontSize)
    {
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 16f;
        tmp.fontSizeMax = maxFontSize;
        tmp.fontSize = maxFontSize;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        tmp.enableWordWrapping = false;
    }

    public static TMP_Dropdown CreateDropdown(Transform parent)
    {
        var go = CreateUIObject("Dropdown", parent);
        StretchFull(go.GetComponent<RectTransform>());

        var image = go.AddComponent<Image>();
        image.color = ButtonNormal;

        var dropdown = go.AddComponent<TMP_Dropdown>();

        var labelGo = CreateUIObject("Label", go.transform);
        var labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12, 4);
        labelRect.offsetMax = new Vector2(-32, -4);
        var label = labelGo.AddComponent<TextMeshProUGUI>();
        label.fontSize = 20f;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.overflowMode = TextOverflowModes.Ellipsis;
        label.raycastTarget = false;
        dropdown.captionText = label;

        var arrowGo = CreateUIObject("Arrow", go.transform);
        var arrowRect = arrowGo.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(1, 0.5f);
        arrowRect.anchorMax = new Vector2(1, 0.5f);
        arrowRect.pivot = new Vector2(1, 0.5f);
        arrowRect.sizeDelta = new Vector2(24, 24);
        arrowRect.anchoredPosition = new Vector2(-8, 0);
        var arrowText = arrowGo.AddComponent<TextMeshProUGUI>();
        arrowText.text = "▼";
        arrowText.fontSize = 14f;
        arrowText.alignment = TextAlignmentOptions.Center;
        arrowText.raycastTarget = false;

        var templateGo = CreateUIObject("Template", go.transform);
        var templateRect = templateGo.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0, 0);
        templateRect.anchorMax = new Vector2(1, 0);
        templateRect.pivot = new Vector2(0.5f, 1);
        templateRect.anchoredPosition = Vector2.zero;
        templateRect.sizeDelta = new Vector2(0, 220);
        templateGo.AddComponent<Image>().color = Panel;

        var templateCanvas = templateGo.AddComponent<Canvas>();
        templateCanvas.overrideSorting = true;
        templateCanvas.sortingOrder = 30000;
        templateGo.AddComponent<GraphicRaycaster>();

        var scroll = templateGo.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;

        var viewportGo = CreateUIObject("Viewport", templateGo.transform);
        var viewportRect = viewportGo.GetComponent<RectTransform>();
        StretchFull(viewportRect);
        viewportRect.offsetMin = new Vector2(4, 4);
        viewportRect.offsetMax = new Vector2(-4, -4);
        viewportGo.AddComponent<Image>().color = Panel;
        viewportGo.AddComponent<Mask>().showMaskGraphic = true;

        var contentGo = CreateUIObject("Content", viewportGo.transform);
        var contentRect = contentGo.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 40);

        var contentLayout = contentGo.AddComponent<VerticalLayoutGroup>();
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;
        contentGo.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRect;

        var itemGo = CreateUIObject("Item", contentGo.transform);
        itemGo.AddComponent<LayoutElement>().preferredHeight = 40;
        var itemToggle = itemGo.AddComponent<Toggle>();
        itemToggle.toggleTransition = Toggle.ToggleTransition.Fade;

        var itemBgGo = CreateUIObject("Item Background", itemGo.transform);
        StretchFull(itemBgGo.GetComponent<RectTransform>());
        var itemBg = itemBgGo.AddComponent<Image>();
        itemBg.color = ButtonNormal;
        itemToggle.targetGraphic = itemBg;

        var checkGo = CreateUIObject("Item Checkmark", itemGo.transform);
        var checkRect = checkGo.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0, 0.5f);
        checkRect.anchorMax = new Vector2(0, 0.5f);
        checkRect.pivot = new Vector2(0, 0.5f);
        checkRect.sizeDelta = new Vector2(16, 16);
        checkRect.anchoredPosition = new Vector2(10, 0);
        var checkImage = checkGo.AddComponent<Image>();
        checkImage.color = AccentPink;
        itemToggle.graphic = checkImage;

        var itemLabelGo = CreateUIObject("Item Label", itemGo.transform);
        var itemLabelRect = itemLabelGo.GetComponent<RectTransform>();
        itemLabelRect.anchorMin = Vector2.zero;
        itemLabelRect.anchorMax = Vector2.one;
        itemLabelRect.offsetMin = new Vector2(32, 2);
        itemLabelRect.offsetMax = new Vector2(-8, -2);
        var itemLabel = itemLabelGo.AddComponent<TextMeshProUGUI>();
        itemLabel.fontSize = 18f;
        itemLabel.color = Color.white;
        itemLabel.alignment = TextAlignmentOptions.MidlineLeft;
        itemLabel.overflowMode = TextOverflowModes.Ellipsis;
        itemLabel.enableWordWrapping = false;
        itemLabel.raycastTarget = true;

        dropdown.template = templateRect;
        dropdown.itemText = itemLabel;
        dropdown.itemImage = checkImage;
        templateGo.SetActive(false);

        return dropdown;
    }

    static GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    static void StretchWithPadding(RectTransform rect, float horizontal, float vertical)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(horizontal, vertical);
        rect.offsetMax = new Vector2(-horizontal, -vertical);
    }
}
