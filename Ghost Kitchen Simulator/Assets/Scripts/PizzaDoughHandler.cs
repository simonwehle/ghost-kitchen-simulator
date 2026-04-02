using UnityEngine;

public class PizzaDoughHandler : MonoBehaviour
{
    [Header("Referenzen")]
    public GameObject toppingZone;

    public void OnPickedUp()
    {
        if (toppingZone != null)
        {
            toppingZone.SetActive(false);
            Debug.Log("Pizza aufgehoben: Topping Zone deaktiviert.");
        }
    }

    public void OnDropped()
    {
        if (toppingZone != null)
        {
            toppingZone.SetActive(true);
            Debug.Log("Pizza abgelegt: Topping Zone aktiviert.");
        }
    }
}