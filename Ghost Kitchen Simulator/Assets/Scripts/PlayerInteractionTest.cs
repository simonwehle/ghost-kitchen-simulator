using UnityEngine;
using UnityEngine.InputSystem; // WICHTIG: Das neue System einbinden

public class PlayerInteractionTest : MonoBehaviour
{
    public float interaktionsReichweite = 5f;
    

    void Update()
    {
        // Im neuen System prüft man Tasten über Keyboard.current
        if (Cursor.lockState == CursorLockMode.Locked && Keyboard.current.eKey.wasPressedThisFrame)
        {
            VersucheZuKlopfen();
        }
    }


    void VersucheZuKlopfen()
    {
        // 1. Wir nehmen die Position des Charakters (nicht der Kamera!)
        // Wir addieren 1.5m zur Y-Achse, damit der Strahl etwa auf Augenhöhe startet
        Vector3 startPunkt = transform.position + Vector3.up * 1.5f;
        
        // 2. Wir nehmen die Richtung, in die der Charakter schaut
        Vector3 richtung = transform.forward;

        RaycastHit hit;

        // Zeichne eine Linie im Scene-Fenster zur Kontrolle
        Debug.DrawRay(startPunkt, richtung * interaktionsReichweite, Color.blue, 2f);

        if (Physics.Raycast(startPunkt, richtung, out hit, interaktionsReichweite))
        {
            // Wir haben etwas getroffen!
            Debug.Log("Getroffen: " + hit.collider.name);

            HouseManager haus = hit.collider.GetComponentInParent<HouseManager>();
            Transform tuerPunkt = haus.tuerPunkt; 

            if (haus != null)
            {
                Debug.Log("Haus erkannt! Klopfen...");
                if(haus.IstJemandZuhause()) GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAutoStepBack>().WeicheZurueck(tuerPunkt);
                haus.KnockOnDoor();
            }
        }
        else
        {
            Debug.Log("In Blickrichtung des Charakters nichts gefunden.");
        }
    }
}