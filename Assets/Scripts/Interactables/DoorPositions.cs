using UnityEngine;

public class DoorPositions : MonoBehaviour
{
    public enum DoorPosition
    {
        InActive,
        Left,
        Right,
        Vertical
    }

    public DoorPosition doorPosition;
    public DoorPosition doorTargetPosition;
}
