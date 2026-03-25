using UnityEngine;
using Unity.Cinemachine;


public class SauceStationInteract : MonoBehaviour
{
    [Header("Scripts & Cameras")]
    public SaucePaintingMinigame minigameScript;
    public CinemachineCamera stationCam;

    public bool TryStartMinigame(PlayerInteraction player)
    {
        Debug.Log("Player is trying to start the Sauce Painting Minigame...");
        // Prüfen, ob der Spieler den ausgerollten Teig hält (Tag muss "FlatDough" sein)
        if (player.CurrentItem != null && player.CurrentItem.gameObject.CompareTag("FlatDough"))
        {
            Debug.Log("Starting Sauce Painting Minigame!");
            player.DestroyHeldItem();

            if (stationCam != null) stationCam.Priority = 20;

            minigameScript.StartMinigame(player, this);
            return true;
        }
        return false;
    }

    public void EndMinigame()
    {
        if (stationCam != null) stationCam.Priority = 0;
    }
}