using UnityEngine;
using Unity.Cinemachine;


public class RollingStationInteract : MonoBehaviour
{
    [Header("Scripts & Cameras")]
    public DoughRollingMinigame minigameScript;
    public CinemachineCamera stationCam; 

    public bool TryStartMinigame(PlayerInteraction player)
    {
        Debug.Log("Player is trying to start the Dough Rolling Minigame...");
        if (player.CurrentItem != null && player.CurrentItem.gameObject.CompareTag("Dough"))
        {
            Debug.Log("Starting Dough Rolling Minigame!");
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