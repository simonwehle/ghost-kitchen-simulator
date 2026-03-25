using UnityEngine;

public class ToppingZone : MonoBehaviour
{
    private PizzaToppingManager manager;

    void Start()
    {
        // Sucht den Manager auf dem Parent-Objekt
        manager = GetComponentInParent<PizzaToppingManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Nur reagieren, wenn das Objekt den Tag "Topping" hat
        if (other.CompareTag("Topping"))
        {
            // WICHTIG: Prüfen, ob das Item gerade noch vom Spieler gehalten wird
            // Wenn dein Item-Script eine "isHeld" Variable hat, nutze sie hier!
            
            // Wir rufen die Funktion am Manager auf
            manager.AddTopping(other.gameObject);
        }
    }
}