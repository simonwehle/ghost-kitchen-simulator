using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI; // WICHTIG: Brauchen wir für UI-Elemente wie das Fadenkreuz!

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public Transform holdPoint;
    public LayerMask interactableLayer;
    public float throwForce = 20f; 

    [Header("Crosshair UI")]
    public RectTransform crosshair; // Zieh dein Crosshair-Image hier rein
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);
    public Vector3 interactScale = new Vector3(2f, 2f, 2f); // Fadenkreuz wird doppelt so dick/groß
    public float animationSpeed = 15f; // Wie schnell es wächst/schrumpft

    private Item currentItem;
    private bool isLookingAtInteractable = false; // Merkt sich, ob wir etwas anschaubar haben

    public Item CurrentItem => currentItem; 

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * interactionDistance, Color.red);
        
        // 1. Permanent prüfen, ob wir etwas anschaubar haben
        CheckForInteractable();
        AnimateCrosshair();

        // 2. Eingaben abfragen (wie vorher)
        if (Keyboard.current.eKey.wasPressedThisFrame) 
        {
            // ERSTENS: Versuchen, mit einer Station (Ofen, Tisch, etc.) zu interagieren
            // Wir übergeben das currentItem (egal ob es null ist oder nicht)
            if (TryUseStation()) 
            {
                // Wenn TryUseStation 'true' liefert, hat der Ofen oder eine Station 
                // die Interaktion geschluckt. Wir sind hier fertig.
                return; 
            }

            // ZWEITENS: Wenn keine Station vor uns ist, normale Item-Logik
            if (currentItem == null)
            {
                TryPickUp();
            }
            else
            {
                DropItem(); // Wenn wir was halten und ins Leere klicken -> Fallen lassen
            }
        }
        
        if (Keyboard.current.qKey.wasPressedThisFrame) 
        {
            if (currentItem != null)
            {
                ThrowItem();
            }
        }
    }

    // --- FADENKREUZ LOGIK ---
    private void CheckForInteractable()
    {
        RaycastHit hit;
        // Schießt jeden Frame einen Strahl. Wenn er den Interactable Layer trifft...
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactableLayer))
        {
            isLookingAtInteractable = true; 
        }
        else
        {
            isLookingAtInteractable = false; 
        }
    }

    private void AnimateCrosshair()
    {
        if (crosshair != null)
        {
            // Lerp sorgt für einen butterweichen Übergang zwischen den Größen
            Vector3 targetScale = isLookingAtInteractable ? interactScale : normalScale;
            crosshair.localScale = Vector3.Lerp(crosshair.localScale, targetScale, Time.deltaTime * animationSpeed);
        }
    }

    // --- STATION LOGIK ---
    private bool TryUseStation()
{
    RaycastHit hit;
    if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactableLayer))
    {
        OvenController oven = hit.collider.GetComponentInParent<OvenController>();
        if (oven != null)
        {
            oven.OnInteract(hit.collider.gameObject, this);
            return true; 
        }

        RollingStationInteract station = hit.collider.GetComponent<RollingStationInteract>();
        if (station != null) return station.TryStartMinigame(this);

        SauceStationInteract sauceStation = hit.collider.GetComponentInParent<SauceStationInteract>();
        if (sauceStation != null) return sauceStation.TryStartMinigame(this);

    }
    return false;
}

    // --- ITEM LOGIK ---
    public void DestroyHeldItem()
    {
        if (currentItem != null)
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }

    public void ForceEquipItem(Item newItem)
    {
        currentItem = newItem;
        currentItem.PickUp(holdPoint);
    }

    void TryPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance, interactableLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                currentItem = item;
                currentItem.PickUp(holdPoint);
            }
        }
    }

    void DropItem()
    {
        if (currentItem != null)
        {
            currentItem.Drop(transform.forward, 1f);
            currentItem = null;
        }
    }

    void ThrowItem()
    {
        if (currentItem != null)
        {
            currentItem.Drop(transform.forward, throwForce);
            currentItem = null;
        }
    }
    public void ClearHeldItem()
    {
        currentItem = null;
    }
}