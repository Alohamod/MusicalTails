using UnityEngine;
using UnityEngine.InputSystem;

public static class UIInputActions
{
    static InputActionAsset _asset;

    public static InputActionAsset Asset
    {
        get => _asset;
        set => _asset = value;
    }
}
