using MusicalTails.Model.Core;
using UnityEngine;

public class ButtonSpawner : MonoBehaviour
{
    public GameObject buttonPrefab;

    public void SpawnSpecificButton(NoteEvent noteEvent)
    {
        float spacing = 2.0f;
        int count = LaneManager.Instance.laneCount;
        float totalWidth = count * spacing;
        float startX = -totalWidth / 2 + (spacing / 2);

        float xPos = startX + (noteEvent.lane * spacing);

        BaseButton model;
        if (noteEvent.typeID == 3) model = new TrapButton(noteEvent.lane, noteEvent.time);
        else if (noteEvent.typeID == 2) model = new DoubleButton(noteEvent.lane, noteEvent.time);
        else model = noteEvent.isLong ? (BaseButton)new Long(noteEvent.lane, noteEvent.time) : (BaseButton)new Short(noteEvent.lane, noteEvent.time);
        GameObject go = Instantiate(buttonPrefab, new Vector3(xPos, 5, 0), Quaternion.identity);
        go.GetComponent<ButtonController>().Initialize(model);
    }
}