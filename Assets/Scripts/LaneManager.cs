using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public int laneCount = 4; 
    public GameObject lanePrefab;
    public SpriteRenderer backgroundRenderer;
    public static LaneManager Instance;
    public GameObject hitZonePrefab; 
    public KeyCode[] keyBindings = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateLanes();
    }

    void GenerateLanes()
    {
        float spacing = 2.0f;
        float totalWidth = laneCount * spacing;
        float startX = -totalWidth / 2 + (spacing / 2);
        for (int i = 0; i < laneCount; i++)
        {
            float xPos = startX + (i * spacing);
            Instantiate(lanePrefab, new Vector3(xPos, 0, 0), Quaternion.identity, transform);

            GameObject hz = Instantiate(hitZonePrefab, new Vector3(xPos, -3, 0), Quaternion.identity);
            HitZone zoneScript = hz.GetComponent<HitZone>();
            zoneScript.laneIndex = i;
            zoneScript.triggerKey = keyBindings[i];
        }

    }
}
