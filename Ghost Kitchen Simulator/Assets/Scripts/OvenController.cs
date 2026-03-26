using UnityEngine;
using System.Collections;

public class OvenController : MonoBehaviour
{
    [Header("Hitboxes")]
    public GameObject topHitbox;
    public GameObject bottomHitbox;

    [Header("Door References")]
    public Transform topDoor;
    public Transform bottomDoor;

    [Header("Pizza Slots")]
    public Transform topSlotTransform;
    public Transform bottomSlotTransform;

    [Header("Indicator Lights")]
    public Light topLight;
    public Light bottomLight;

    [Header("Settings")]
    public float openAngle = -90f;
    public float speed = 5f;
    public float bakeTime = 60f;
    public float burnTime = 30f;
    public float ejectDistance = 0.2f;

    [Header("Indicator Colors")]
    public Color bakingColor = new Color(1f, 0.5f, 0f);
    public Color readyColor = Color.green;
    public Color burntLightColor = Color.red;
    public Color offColor = Color.black;

    [Header("Visual Effects")]
    public Color bakedPizzaColor = new Color(0.6f, 0.4f, 0.2f);
    public Color burntPizzaColor = Color.black;
    public GameObject steamParticlesPrefab;

    [Header("State")]
    public bool isTopOpen;
    public bool isBottomOpen;
    private GameObject pizzaInTop, pizzaInBottom;
    private bool isTopReady, isBottomReady;
    private bool isTopBurnt, isBottomBurnt;

    void Update()
    {
        RotateDoor(topDoor, isTopOpen ? openAngle : 0);
        RotateDoor(bottomDoor, isBottomOpen ? openAngle : 0);
    }

    private void RotateDoor(Transform door, float targetAngle)
    {
        if (door == null) return;
        Quaternion targetRotation = Quaternion.Euler(targetAngle, 0, 0);
        door.localRotation = Quaternion.Slerp(door.localRotation, targetRotation, Time.deltaTime * speed);
    }

    public void OnInteract(GameObject hitObject, PlayerInteraction player)
    {
        bool isTop = (hitObject == topHitbox || hitObject == topDoor.gameObject);
        
        if (isTop)
            HandleSlotLogic(ref isTopOpen, ref pizzaInTop, ref isTopReady, ref isTopBurnt, topSlotTransform, topLight, player, true);
        else
            HandleSlotLogic(ref isBottomOpen, ref pizzaInBottom, ref isBottomReady, ref isBottomBurnt, bottomSlotTransform, bottomLight, player, false);
    }

    private void HandleSlotLogic(ref bool isOpen, ref GameObject pizzaInSlot, ref bool isReady, ref bool isBurnt, Transform slot, Light indicator, PlayerInteraction player, bool isTop)
    {
        if (!isOpen)
        {
            isOpen = true; 
            if (pizzaInSlot != null && (isReady || isBurnt))
            {
                StartCoroutine(SlidePizza(pizzaInSlot.transform, new Vector3(0, 0, ejectDistance)));
            }
            return;
        }

        if (pizzaInSlot != null)
        {
            if (player.CurrentItem != null) return; 

            TakePizzaFromOven(ref pizzaInSlot, ref isReady, ref isBurnt, indicator, player);
            return;
        }

        Item heldItem = player.CurrentItem;
        if (heldItem != null && (heldItem.CompareTag("SaucedDough") || heldItem.name.Contains("Sauced Dough")))
        {
            PlacePizzaInOven(heldItem, slot, ref pizzaInSlot);
            player.ClearHeldItem(); 
            isOpen = false;
            StartCoroutine(BakingProcess(indicator, isTop, pizzaInSlot));
        }
        else 
        { 
            isOpen = false; 
        }
    }

    private IEnumerator BakingProcess(Light indicator, bool isTop, GameObject pizza)
    {
        if (isTop) { isTopReady = false; isTopBurnt = false; }
        else { isBottomReady = false; isBottomBurnt = false; }

        if (indicator) indicator.color = bakingColor;
        yield return new WaitForSeconds(bakeTime);

        // PIZZA IST FERTIG GEBACKEN
        if (isTop) isTopReady = true; else isBottomReady = true;
        if (indicator) indicator.color = readyColor;
        ApplyBakeEffect(pizza); 
        SpawnSteam(pizza);

        // --- NEU: Tag auf "Pizza" setzen ---
        if (pizza != null)
        {
            pizza.tag = "Pizza";
        }

        // WARTEN AUFS VERBRENNEN
        yield return new WaitForSeconds(burnTime);

        // PIZZA IST VERBRANNT
        if (pizza != null && ((isTop && pizzaInTop == pizza) || (!isTop && pizzaInBottom == pizza)))
        {
            if (isTop) { isTopReady = false; isTopBurnt = true; }
            else { isBottomReady = false; isBottomBurnt = true; }

            if (indicator) indicator.color = burntLightColor;
            ApplyBurntEffect(pizza); 
            
            // Info: Der Tag bleibt hier auf "Pizza". Wenn du verbrannte Pizzen 
            // vom System anders behandeln willst (z.B. Müll), könntest du hier 
            // pizza.tag = "BurntPizza"; setzen!
        }
    }

    private void TakePizzaFromOven(ref GameObject pizzaInSlot, ref bool isReady, ref bool isBurnt, Light indicator, PlayerInteraction player)
    {
        Item pizzaItem = pizzaInSlot.GetComponent<Item>();
        if (pizzaItem != null)
        {
            if (pizzaInSlot.TryGetComponent<Rigidbody>(out Rigidbody rb)) 
            { 
                rb.isKinematic = false; 
                rb.useGravity = true; 
            }

            player.ForceEquipItem(pizzaItem);
            
            pizzaInSlot = null;
            isReady = false;
            isBurnt = false; 
            if (indicator) indicator.color = offColor;
        }
    }

    private void ApplyBakeEffect(GameObject pizza)
    {
        if (pizza == null) return;
        
        Renderer[] allRenderers = pizza.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in allRenderers)
        {
            if (rend != null && rend.material != null)
            {
                rend.material.color = Color.Lerp(rend.material.color, bakedPizzaColor, 0.5f);
            }
        }
    }

    private void ApplyBurntEffect(GameObject pizza)
    {
        if (pizza == null) return;
        
        Renderer[] allRenderers = pizza.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in allRenderers)
        {
            if (rend != null && rend.material != null)
            {
                rend.material.color = burntPizzaColor;
            }
        }
    }

    private void SpawnSteam(GameObject pizza)
    {
        if (pizza == null || steamParticlesPrefab == null) return;
        GameObject steam = Instantiate(steamParticlesPrefab, pizza.transform.position + Vector3.up * 0.05f, pizza.transform.rotation);
        steam.transform.SetParent(pizza.transform);
        Destroy(steam, 5f);
    }

    private void PlacePizzaInOven(Item item, Transform slot, ref GameObject slotRef)
    {
        GameObject pizzaObj = item.gameObject;
        pizzaObj.transform.SetParent(slot);
        pizzaObj.transform.localPosition = Vector3.zero;
        pizzaObj.transform.localRotation = Quaternion.identity;

        if (pizzaObj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        slotRef = pizzaObj;
    }

    private IEnumerator SlidePizza(Transform pTransform, Vector3 targetLocalPos)
    {
        float elapsed = 0f;
        Vector3 startPos = pTransform.localPosition;
        while (elapsed < 0.5f)
        {
            pTransform.localPosition = Vector3.Lerp(startPos, targetLocalPos, elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        pTransform.localPosition = targetLocalPos;
    }
}