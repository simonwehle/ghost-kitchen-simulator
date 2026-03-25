using UnityEngine;
using System.Collections.Generic;

public class PizzaToppingManager : MonoBehaviour
{
    [Header("Pizza Data")]
    public List<string> toppingsOnPizza = new List<string>(); // Speichert die Namen der Beläge

    // Wird von der ToppingZone aufgerufen
    public void AddTopping(GameObject topping)
    {
        // 1. Speichern: Welches Topping ist das? (Nutzt den Namen des Objekts oder ein Item-Script)
        string toppingName = topping.name.Replace("(Clone)", "").Trim();
        toppingsOnPizza.Add(toppingName);

        // 2. Parenting: Die Pizza wird der neue "Papa"
        topping.transform.SetParent(this.transform);

        // 3. Physik deaktivieren
        // Hitbox (Collider) ausmachen
        if (topping.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }

        // Rigidbody ausschalten, damit es nicht mehr fällt oder rollt
        if (topping.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        Debug.Log($"Topping hinzugefügt: {toppingName}. Anzahl insgesamt: {toppingsOnPizza.Count}");
    }
}