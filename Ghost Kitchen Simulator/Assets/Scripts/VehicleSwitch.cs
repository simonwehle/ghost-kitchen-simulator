using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleSwitch : MonoBehaviour
{
    public GameObject player;
    public GameObject car;
    public SimpleCarController carController;
    public ThirdPersonController playerController;
    public SkinnedMeshRenderer playerVisual;
    public Transform playerCamera;
    public GameObject playerCamRoot;

    private bool playerInRange = false;
    private bool isDriving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!isDriving && playerInRange)
            {
                EnterCar();
            }
            else if (isDriving)
            {
                ExitCar();
            }
        }
    }

    void EnterCar()
    {
        playerController.enabled = false;
        playerVisual.enabled = false;
        playerCamRoot.SetActive(false);

        carController.enabled = true;
        carController.canDrive = true;

        playerCamera.SetParent(car.transform);
        playerCamera.localPosition = new Vector3(0, 3, -6);
        playerCamera.localRotation = Quaternion.Euler(20, 0, 0);

        isDriving = true;
    }

    void ExitCar()
    {
        Vector3 exitOffset = car.transform.right * 2f;
        Vector3 exitPosition = car.transform.position + exitOffset + Vector3.up * 0.5f;

        player.transform.position = exitPosition;
        player.transform.rotation = Quaternion.Euler(0, car.transform.eulerAngles.y, 0);

        playerController.enabled = true;
        playerVisual.enabled = true;
        playerCamRoot.SetActive(true);
        playerCamera.SetParent(player.transform);
        //playerCamera.localPosition = Vector3.zero;
        //playerCamera.localRotation = Quaternion.identity;

        carController.enabled = false;
        carController.canDrive= false;

        isDriving = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<ThirdPersonController>() != null)
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<ThirdPersonController>() != null)
        {
            playerInRange = false;
        }
    }
}
