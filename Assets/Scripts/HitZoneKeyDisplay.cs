using TMPro;
using UnityEngine;

[RequireComponent(typeof(HitZone))]
public class HitZoneKeyDisplay : MonoBehaviour
{
    static TMP_FontAsset _font;
    static Sprite _boxSprite;

    HitZone _zone;
    TextMeshPro _label;
    SpriteRenderer _background;

    public void Setup(KeyCode key)
    {
        _zone = GetComponent<HitZone>();
        if (_label != null)
        {
            _label.text = FormatKey(key);
            return;
        }

        BuildVisuals(key);
    }

    void Update()
    {
        if (_label == null || _zone == null || _zone.triggerKey == KeyCode.None) return;

        bool pressed = Input.GetKey(_zone.triggerKey);
        _label.color = pressed ? OsuStyleUI.AccentPink : OsuStyleUI.AccentCyan;
        _label.transform.localScale = pressed ? Vector3.one * 1.12f : Vector3.one;

        if (_background != null)
            _background.color = pressed
                ? new Color(0.35f, 0.15f, 0.28f, 0.95f)
                : new Color(0.12f, 0.09f, 0.2f, 0.85f);
    }

    void BuildVisuals(KeyCode key)
    {
        if (_font == null)
            _font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        var hitRenderer = GetComponent<SpriteRenderer>();
        Vector2 hitSize = Vector2.one;
        int hitSortingOrder = 0;
        if (hitRenderer != null && hitRenderer.sprite != null)
        {
            hitSize = hitRenderer.sprite.bounds.size;
            hitSortingOrder = hitRenderer.sortingOrder;
        }

        var bgGo = new GameObject("KeyBackground");
        bgGo.transform.SetParent(transform, false);
        bgGo.transform.localPosition = Vector3.zero;
        bgGo.transform.localScale = new Vector3(hitSize.x, hitSize.y, 1f);

        _background = bgGo.AddComponent<SpriteRenderer>();
        _background.sprite = GetBoxSprite();
        _background.color = new Color(0.12f, 0.09f, 0.2f, 0.85f);
        _background.sortingOrder = hitSortingOrder + 1;

        var labelGo = new GameObject("KeyLabel");
        labelGo.transform.SetParent(transform, false);
        labelGo.transform.localPosition = new Vector3(0f, 0f, -0.1f);

        _label = labelGo.AddComponent<TextMeshPro>();
        if (_font != null)
            _label.font = _font;
        _label.text = FormatKey(key);
        _label.fontSize = 4f;
        _label.fontStyle = FontStyles.Bold;
        _label.alignment = TextAlignmentOptions.Center;
        _label.color = OsuStyleUI.AccentCyan;
        _label.sortingOrder = hitSortingOrder + 2;
        _label.outlineWidth = 0.15f;
        _label.outlineColor = new Color(0f, 0f, 0f, 0.8f);
    }

    static string FormatKey(KeyCode key)
    {
        if (key == KeyCode.None) return "?";

        string name = key.ToString();
        if (name.StartsWith("Alpha"))
            return name[5..];
        return name;
    }

    static Sprite GetBoxSprite()
    {
        if (_boxSprite != null) return _boxSprite;

        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        _boxSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return _boxSprite;
    }
}
