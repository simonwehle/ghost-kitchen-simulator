using UnityEngine;
using TMPro; // Wichtig für TextMeshPro

public class OrderUIManager : MonoBehaviour
{
    public static OrderUIManager Instance { get; private set; }

    [Header("UI Referenzen")]
    public GameObject orderPanel;       // Dein OrderMenu_Panel
    public TextMeshProUGUI orderText;   // Dein OrderText

    private NPC_Order currentActiveNPC; // Merkt sich, mit wem wir gerade reden

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        // Zu Beginn verstecken
        if (orderPanel != null) orderPanel.SetActive(false);
    }

    // Wird vom Spieler-Skript aufgerufen, wenn er 'E' drückt
    public void OpenOrderMenu(NPC_Order npc)
    {
        currentActiveNPC = npc;
        orderText.text = npc.GetOrderText();
        orderPanel.SetActive(true);

        // Mauszeiger freigeben, damit man den Button klicken kann
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Diese Methode musst du gleich im Inspector auf deinen "Bestellung annehmen" Button legen!
    public void OnAcceptOrderButtonClicked()
    {
        if (currentActiveNPC != null)
        {
            currentActiveNPC.AcceptOrder(); // Startet den Timer beim NPC
        }

        CloseMenu();
    }

    public void CloseMenu()
    {
        orderPanel.SetActive(false);
        currentActiveNPC = null;

        // Mauszeiger wieder sperren für das First-Person Gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}