using UnityEngine;
using Unity.Cinemachine; 

public class RollingStationInteract : MonoBehaviour
{
    [Header("Scripts & Cameras")]
    public DoughRollingMinigame minigameScript;
    public CinemachineCamera stationCam; 

    public bool TryStartMinigame(PlayerInteraction player)
    {
        if (player.CurrentItem != null && player.CurrentItem.gameObject.CompareTag("Dough"))
        {
            player.DestroyHeldItem();

            if (stationCam != null) stationCam.Priority = 20; 

            // NEU: Wir übergeben den Spieler und diese Station an das Minigame
            minigameScript.StartMinigame(player, this);

            return true; 
        }
        
        return false; 
    }

    // NEU: Wird vom Minigame aufgerufen, wenn es fertig ist
    public void EndMinigame()
    {
        // Priorität wieder auf 0 setzen -> Die Kamera schwenkt automatisch zum Spieler zurück!
        if (stationCam != null) stationCam.Priority = 0;
    }
}