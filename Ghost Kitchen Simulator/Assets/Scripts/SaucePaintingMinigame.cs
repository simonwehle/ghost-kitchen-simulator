using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; 

public class SaucePaintingMinigame : MonoBehaviour
{
    [Header("Station Visuals (Dummies)")]
    public GameObject spoonObject;        // The spoon model
    public GameObject flatDoughDummy;    // The "clean" dough on the table
    public Transform spawnPoint;         // Where sauce drops come out

    [Header("Player Items (Prefabs)")]
    public GameObject sauceDropPrefab;   // The splatting sauce drop
    public Item saucedDoughPrefab;       // THE REAL ITEM (Dough + Sauce) for the player

    [Header("Movement Bounds")]
    public Transform stationCenter; // Drag your Station or Pizza Dough here
    public float xLimit = 0.5f;     // Max distance left/right
    public float zLimit = 0.5f;     // Max distance front/back  
    
    [Header("Spoon Sensitivity")]
    public float sensitivity = 0.05f;

    [Header("Auto-Drop Settings")]
    public float dropFrequency = 0.2f; // Time between drops (0.2s)
    private float nextDropTime = 0f;    // Timer variable

    [Header("Settings")]
    public float spoonHeight = 2f;
    public int dropsNeeded = 25;

    private bool isPlaying = false;
    private int currentDrops = 0;
    
    // References to sync with your system
    private PlayerInteraction currentPlayer;
    private SauceStationInteract currentStation;
    private PlayerInput _playerInput;

    private List<GameObject> spawnedDrops = new List<GameObject>();

    void Start()
    {
        // Make sure everything is hidden at the start
        //if (spoonObject != null) spoonObject.SetActive(false);
        if (flatDoughDummy != null) flatDoughDummy.SetActive(false);
    }

    public void StartMinigame(PlayerInteraction player, SauceStationInteract station)
    {
        currentPlayer = player;
        currentStation = station;

        // Find and disable Player Input (matching your Rolling script logic)
        _playerInput = player.transform.parent.GetComponentInChildren<PlayerInput>();
        if (_playerInput != null) _playerInput.enabled = false;

        isPlaying = true;
        currentDrops = 0;


        // Show the table visuals
        spoonObject.SetActive(true);
        flatDoughDummy.SetActive(true);
        nextDropTime = 0f;

        
    }

    void Update()
    {
        if (!isPlaying) return;

        MoveSpoonWithMouse();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            DropSauce();
            nextDropTime = Time.time + dropFrequency;
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            if (Time.time >= nextDropTime)
            {
                DropSauce();
                nextDropTime = Time.time + dropFrequency;
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            AbortMinigame();
        }
    }

    void MoveSpoonWithMouse()
    {
        // 1. Get how much the mouse moved since the last frame
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 2. Calculate the "Desired" new position based on movement
        // Note: mouseDelta.y is screen UP/DOWN, which maps to world Z (front/back)
        Vector3 newPosition = spoonObject.transform.position + new Vector3(mouseDelta.x * sensitivity, 0, mouseDelta.y * sensitivity);

        // 3. Apply Clamping based on the station center
        float centerX = stationCenter.position.x;
        float centerZ = stationCenter.position.z;

        float clampedX = Mathf.Clamp(newPosition.x, centerX - xLimit, centerX + xLimit);
        float clampedZ = Mathf.Clamp(newPosition.z, centerZ - zLimit, centerZ + zLimit);

        // 4. Update position
        spoonObject.transform.position = new Vector3(clampedX, spoonHeight, clampedZ);
    }

    void DropSauce()
    {
        GameObject newDrop = Instantiate(sauceDropPrefab, spawnPoint.position, Quaternion.identity);
        
        // Add the new drop to our list
        spawnedDrops.Add(newDrop);
        
        currentDrops++;

        if (currentDrops >= dropsNeeded)
        {
            FinishMinigame();
        }
    }

    private void FinishMinigame()
    {
        isPlaying = false;
        ClearAllSauce();

        // 1. Cleanup table visuals
        //spoonObject.SetActive(false);
        flatDoughDummy.SetActive(false);
        
        // Find all sauce drops on the table and destroy them 
        // (so they don't float in the air when the pizza is gone)
        GameObject[] drops = GameObject.FindGameObjectsWithTag("SauceDrop"); 
        foreach (GameObject d in drops) Destroy(d);

        // 2. Reset Input/Cursor
        if (_playerInput != null) _playerInput.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 3. Spawn the SAUCED DOUGH item for the player
        if (saucedDoughPrefab != null && currentPlayer != null)
        {
            Item newPizza = Instantiate(saucedDoughPrefab, currentPlayer.transform.position, Quaternion.identity);
            currentPlayer.ForceEquipItem(newPizza);
        }

        // 4. Reset Camera
        if (currentStation != null) currentStation.EndMinigame();
    }

    private void AbortMinigame()
    {
        isPlaying = false;
        if (_playerInput != null) _playerInput.enabled = true;
        if (currentStation != null) currentStation.EndMinigame();
        
        ClearAllSauce();
        //spoonObject.SetActive(false);
        flatDoughDummy.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void OnDrawGizmosSelected()
    {
        if (stationCenter != null)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(stationCenter.position.x, spoonHeight, stationCenter.position.z);
            Gizmos.DrawWireCube(center, new Vector3(xLimit * 2, 0.1f, zLimit * 2));
        }
    }
    private void ClearAllSauce()
    {
        foreach (GameObject drop in spawnedDrops)
        {
            if (drop != null)
            {
                Destroy(drop);
            }
        }
        
        spawnedDrops.Clear();
    }

}