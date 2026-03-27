using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class VehicleSwitch : MonoBehaviour
{
    public GameObject player;
    public GameObject car;
    public SimpleCarController carController;
    public Camera carCamera;
    public GameObject uiPanel;

    private GameObject enterCarText;
    private GameObject exitCarText;

    private bool playerInRange = false;
    private bool isDriving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enterCarText = uiPanel.transform.Find("EnterCarTM").gameObject;
        exitCarText = uiPanel.transform.Find("ExitCarTM").gameObject;

        enterCarText.SetActive(false);
        exitCarText.SetActive(false);
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
        player.SetActive(false);
        carCamera.gameObject.SetActive(true);
        player.transform.position = car.transform.position;

        carController.enabled = true;
        carController.canDrive = true;

        enterCarText.SetActive(false);
        exitCarText.SetActive(true);

        isDriving = true;
    }

    void ExitCar()
    {
        player.transform.position = car.transform.position + Vector3.up * 4f;

        player.SetActive(true);
        carCamera.gameObject.SetActive(false);

        carController.enabled = false;
        carController.canDrive = false;

        uiPanel.SetActive(false);
        exitCarText.SetActive(false);

        isDriving = false;
        playerInRange = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<ThirdPersonController>() != null)
        {
            playerInRange = true;
            uiPanel.SetActive(true);
            enterCarText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<ThirdPersonController>() != null)
        {
            playerInRange = false;
            uiPanel.SetActive(false);
            enterCarText.SetActive(false);
        }
    }
}
