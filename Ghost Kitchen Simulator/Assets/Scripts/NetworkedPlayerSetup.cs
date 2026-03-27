using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkedPlayerSetup : NetworkBehaviour
{
    public GameObject cameraRoot;
    public PlayerInput playerInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            if (playerInput) playerInput.enabled = false;
        }
    }
}