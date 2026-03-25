using UnityEngine;

public class OvenController : MonoBehaviour
{
    [Header("Door References")]
    public Transform topDoor;
    public Transform bottomDoor;

    [Header("Settings")]
    public float openAngle = -90f; 
    public float closeAngle = 0f;
    public float speed = 2f;

    public bool isTopOpen = false;
    public bool isBottomOpen = false;

    void Update()
    {
        RotateDoor(topDoor, isTopOpen ? openAngle : closeAngle);
        RotateDoor(bottomDoor, isBottomOpen ? openAngle : closeAngle);
    }

    private void RotateDoor(Transform door, float targetAngle)
    {
        if (door == null) return;
        
        Quaternion targetRotation = Quaternion.Euler(targetAngle, 0, 0);
        door.localRotation = Quaternion.Slerp(door.localRotation, targetRotation, Time.deltaTime * speed);
    }

    public void ToggleTopDoor() => isTopOpen = !isTopOpen;
    public void ToggleBottomDoor() => isBottomOpen = !isBottomOpen;
    
    public void OpenBoth() { isTopOpen = true; isBottomOpen = true; }
    public void CloseBoth() { isTopOpen = false; isBottomOpen = false; }
}