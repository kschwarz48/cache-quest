using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private bool hasPlayerLeftSafeZone = false;

    void Start()
    {
        // Initially, the player is in the safe zone
        UpdateDoorInteractionSafeZoneStatus(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!hasPlayerLeftSafeZone && other.CompareTag("Player"))
        {
            hasPlayerLeftSafeZone = true;
            UpdateDoorInteractionSafeZoneStatus(false);
        }
    }

    private void UpdateDoorInteractionSafeZoneStatus(bool status)
    {
        DoorInteraction[] exits = FindObjectsOfType<DoorInteraction>();
        foreach (var exit in exits)
        {
            exit.SetSafeZoneStatus(status);
        }
    }
}
