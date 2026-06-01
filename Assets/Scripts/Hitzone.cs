using UnityEngine;

public class HitZone : MonoBehaviour
{
    public int laneIndex; 
    public KeyCode triggerKey; 
    private GameObject _currentButton;
    private bool _isPressed = false;



    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            _isPressed = true;
            TryHit();
        }
        if (Input.GetKeyUp(triggerKey))
        {
            _isPressed = false;
            TryRelease();
        }
    }

    private void TryHit()
    {
        ButtonController[] buttons = FindObjectsOfType<ButtonController>();
        foreach (var btn in buttons)
        {
            if (btn.GetLane() == laneIndex && Mathf.Abs(btn.transform.position.y - transform.position.y) < 1.0f)
            {
                btn.HandleClick();
                return;
            }
        }
    }

    private void TryRelease()
    {
        ButtonController[] buttons = FindObjectsOfType<ButtonController>();
        foreach (var btn in buttons)
        {
            if (btn.GetLane() == laneIndex) btn.ReleaseClick();
        }
    }
}