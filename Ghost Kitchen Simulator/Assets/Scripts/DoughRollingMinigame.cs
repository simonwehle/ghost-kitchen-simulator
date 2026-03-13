using UnityEngine;
using UnityEngine.InputSystem; 

public class DoughRollingMinigame : MonoBehaviour
{
    [Header("Station Dummies (auf dem Tisch)")]
    public Transform rollingPin;
    public Transform doughBall; 
    public GameObject flatDoughModel; // Wird nur für den allerletzten Frame gebraucht, kann eigtl auch ganz weg, aber wir lassen es sicherheitshalber

    [Header("Player Item (aus dem Projekt-Ordner!)")]
    public Item flatDoughPrefab; // DAS ECHTE ITEM für den Spieler! Muss das "Item" Skript haben!

    [Header("Settings")]
    public float rollSpeed = 0.05f; 
    public float pinLimitZ = 0.5f; 
    public float requiredRolls = 5f; 

    private bool isMinigameActive = false;
    private float totalDistanceRolled = 0f;
    private Vector3 originalPinPosition;
    private Vector3 originalDoughScale;

    // Referenzen für das Ende
    private PlayerInteraction currentPlayer;
    private RollingStationInteract currentStation;

    void Start()
    {
        originalPinPosition = rollingPin.localPosition;
        if(doughBall != null) originalDoughScale = doughBall.localScale;
        
        if(flatDoughModel != null) flatDoughModel.SetActive(false);
    }

    void Update()
    {
        if (!isMinigameActive) return;

        float mouseY = 0f;
        if (Mouse.current != null)
        {
            mouseY = Mouse.current.delta.y.ReadValue();
        }
        
        float moveAmount = mouseY * rollSpeed * Time.deltaTime;
        Vector3 newPos = rollingPin.localPosition + new Vector3(0, 0, moveAmount);
        
        newPos.z = Mathf.Clamp(newPos.z, originalPinPosition.z - pinLimitZ, originalPinPosition.z + pinLimitZ);
        
        float distanceMoved = Mathf.Abs(rollingPin.localPosition.z - newPos.z);
        rollingPin.localPosition = newPos;

        if (distanceMoved > 0)
        {
            totalDistanceRolled += distanceMoved;
            UpdateDoughVisuals();
        }
    }

    private void UpdateDoughVisuals()
    {
        float progress = Mathf.Clamp01(totalDistanceRolled / requiredRolls);

        float currentY = Mathf.Lerp(originalDoughScale.y, 0.2f, progress);
        float currentXZ = Mathf.Lerp(originalDoughScale.x, 1.0f, progress);

        doughBall.localScale = new Vector3(currentXZ, currentY, currentXZ);

        if (progress >= 1f)
        {
            FinishMinigame();
        }
    }

    // NEU: Erwartet jetzt den Spieler und die Station als Info
    public void StartMinigame(PlayerInteraction player, RollingStationInteract station)
    {
        currentPlayer = player;
        currentStation = station;

        isMinigameActive = true;
        totalDistanceRolled = 0f;
        
        doughBall.gameObject.SetActive(true);
        flatDoughModel.SetActive(false);
        doughBall.localScale = originalDoughScale; 
        
        Cursor.lockState = CursorLockMode.Locked; 
    }

    private void FinishMinigame()
    {
        isMinigameActive = false;
        
        // 1. Station wieder komplett aufräumen für die nächste Nutzung!
        doughBall.gameObject.SetActive(false);
        flatDoughModel.SetActive(false); // Beide Dummys unsichtbar, Tisch ist wieder leer
        doughBall.localScale = originalDoughScale; // Reset der Kugel-Größe

        // 2. Maus wieder freigeben
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        
        // 3. Das ECHTE Item erschaffen und dem Spieler in die Hand drücken
        if (flatDoughPrefab != null && currentPlayer != null)
        {
            Item newPizzaBase = Instantiate(flatDoughPrefab, currentPlayer.transform.position, Quaternion.identity);
            currentPlayer.ForceEquipItem(newPizzaBase);
        }
        else
        {
            Debug.LogError("Fehler: Kein Flat Dough Prefab zugewiesen!");
        }

        // 4. Kamera wieder zum Spieler schwenken
        if (currentStation != null)
        {
            currentStation.EndMinigame();
        }
    }
}