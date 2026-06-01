using UnityEngine;

public class GameBackground : MonoBehaviour
{
    public int laneCount; 
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        float totalWidth = laneCount * 2.0f;
        spriteRenderer.size = new Vector2(totalWidth, 10.0f); 
    }
}