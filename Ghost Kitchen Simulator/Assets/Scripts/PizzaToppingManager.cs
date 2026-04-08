using UnityEngine;
using System.Collections.Generic;

public class PizzaToppingManager : MonoBehaviour
{
    [Header("Pizza Data")]
    public List<ToppingType> toppingsOnPizza = new List<ToppingType>();

    // Wird von der ToppingZone aufgerufen
    public void AddTopping(GameObject topping)
    {
        ToppingItem item = topping.GetComponent<ToppingItem>();
        if (item == null || item.toppingType == null)
        {
            Debug.LogWarning($"Topping '{topping.name}' has no ToppingItem component or ToppingType assigned — skipping.");
            return;
        }

        toppingsOnPizza.Add(item.toppingType);

        topping.transform.SetParent(this.transform);

        if (topping.TryGetComponent<Collider>(out Collider col))
            col.enabled = false;

        if (topping.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        Debug.Log($"Topping hinzugefügt: {item.toppingType.displayName}. Gesamt: {toppingsOnPizza.Count}");
    }
}
