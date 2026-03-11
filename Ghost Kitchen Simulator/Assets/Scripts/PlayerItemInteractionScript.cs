using UnityEngine;
// WICHTIG: Du musst diesen Namespace hinzufügen!
using UnityEngine.InputSystem; 

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;
    public Transform holdPoint;
    public LayerMask interactableLayer;
    public float throwForce = 20f; 

    private Item currentItem;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.red);
        // Die neue Art, eine Taste abzufragen:
        if (Keyboard.current.eKey.wasPressedThisFrame) 
        {
            if (currentItem == null)
            {
                TryPickUp();
                Debug.Log("E-Taste gedrückt, versuche aufzuheben...");
            }
            else
            {
                DropItem();
                Debug.Log("E-Taste gedrückt, versuche abzulegen...");
            }
        }
    }

    void TryPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactableLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            Debug.Log("Interagiert mit: " + hit.collider.name);
            if (item != null)
            {
                currentItem = item;
                currentItem.PickUp(holdPoint);
                Debug.Log("Gegenstand aufgehoben: " + hit.collider.name);
            }
        }
    }

    void DropItem()
    {
        if (currentItem != null)
        {
            currentItem.Drop(throwForce);
            currentItem = null;
            Debug.Log("Gegenstand abgelegt.");
        }
    }
}