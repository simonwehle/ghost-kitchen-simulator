using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHouse : MonoBehaviour
{
    // Singleton für leichten Zugriff von anderen Scripts (z.B. UIManager)
    public static PlayerInteractionHouse Instance { get; private set; }

    [Header("Interaction")]
    public float interaktionsReichweite = 5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Interaktion nur möglich, wenn der Cursor gesperrt ist (Spielmodus)
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                VersucheInteraktion();
            }
        }
    }

    void VersucheInteraktion()
    {
        // Raycast startet wieder exakt am Spieler (1.5 Meter über dem Boden) und geht in Blickrichtung der Figur
        Vector3 startPunkt = transform.position + Vector3.up * 1.5f;
        Vector3 richtung = transform.forward;
        RaycastHit hit;

        //Debug.DrawRay(startPunkt, richtung * interaktionsReichweite, Color.green, 2f);

        if (Physics.Raycast(startPunkt, richtung, out hit, interaktionsReichweite))
        {
            HouseManager haus = hit.collider.GetComponentInParent<HouseManager>();
            if (haus != null)
            {
                Debug.Log("Interaction: House/Door detected.");
                InteragiereMitHaus(haus);
                return; 
            }

            NPC_StadtLeben npc = hit.collider.GetComponentInParent<NPC_StadtLeben>();
            if (npc != null)
            {
                Debug.Log("Interaction: NPC detected.");
                InteragiereMitNPC(npc);
                return; 
            }
        }
        else
        {
            Debug.Log("Nothing in range to interact with.");
        }
    }

    private void InteragiereMitHaus(HouseManager haus)
    {
        if (haus.IstJemandZuhause())
        {
            var autoStep = GetComponent<PlayerAutoStepBack>();
            if (autoStep != null && haus.tuerPunkt != null) 
            {
                autoStep.WeicheZurueck(haus.tuerPunkt);
            }
        }
        haus.KnockOnDoor();
    }

    private void InteragiereMitNPC(NPC_StadtLeben npc)
    {
        if (npc.KannInteragieren())
        {
            if (NPCShoppingManager.Instance != null && NPCShoppingManager.Instance.GetAktuellenKunden() == npc)
            {
                Debug.Log($"Serving customer at the counter: {npc.name}");
                npc.SchaueSpielerAn(transform);
                npc.BedienungErfolgt();
            }
            else
            {
                npc.SchaueSpielerAn(transform);
                Debug.Log("Conversation with NPC started (outside the store).");
            }
        }
        else
        {
            Debug.Log("NPC ignores you (is busy or wandering).");
        }
    }
}