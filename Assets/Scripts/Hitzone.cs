using UnityEngine;

public class HitZone : MonoBehaviour
{
    public int laneIndex;
    public KeyCode triggerKey;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
            TryHit();
        if (Input.GetKeyUp(triggerKey))
            TryRelease();
    }

    private void TryHit()
    {
        var buttons = FindObjectsByType<ButtonController>(FindObjectsSortMode.None);
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
        var buttons = FindObjectsByType<ButtonController>(FindObjectsSortMode.None);
        foreach (var btn in buttons)
        {
            if (btn.GetLane() == laneIndex) btn.ReleaseClick();
        }
    }
}