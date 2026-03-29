using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;

public class NPC_Order : MonoBehaviour
{
    public enum OrderState { Idle, WantsToOrder, WaitingForFood, Done }
    public OrderState currentState = OrderState.Idle;

    [Header("UI Referenzen (Am NPC)")]
    [HideInInspector] public GameObject canvas; 
    [HideInInspector] public GameObject questionBubble; 
    [HideInInspector] public GameObject timerBubble;    
    [HideInInspector] public Image timerFillImage;      

    [Header("Bestellung")]
    public List<string> availableToppings = new List<string> { "Salami", "Pilze", "Schinken" };
    public List<string> wantedToppings = new List<string>();

    [Header("Wartezeit")]
    private float maxWaitTime;
    private float currentWaitTime;

    [Header("UI Skalierung (Distanz)")]
    public float minDistance = 2f;  // Wenn der Spieler näher als 2 Meter ist...
    public float maxDistance = 15f; // Wenn der Spieler weiter als 10 Meter entfernt ist...
    public float minScale = 0.5f;     // ...ist die Größe 1x
    public float maxScale = 2f;     // ...ist die Größe 2x

    private Transform mainCamera;

    void Awake()
    {
        // 1. Sucht den Canvas in den Unterobjekten dieses NPCs
        Canvas gefundenesCanvas = GetComponentInChildren<Canvas>();
        
        if (gefundenesCanvas != null)
        {
            canvas = gefundenesCanvas.gameObject;

            // 2. Sucht in der ersten Ebene unter dem Canvas nach exakten Namen
            Transform qBubble = canvas.transform.Find("QuestionBubble"); 
            if (qBubble != null) questionBubble = qBubble.gameObject;

            Transform tBubble = canvas.transform.Find("TimerBubble");
            if (tBubble != null) 
            {
                timerBubble = tBubble.gameObject;
                Transform tFill = timerBubble.transform.Find("TimerCircle");
                // 3. Findet das Image (den Ladebalken) direkt in der TimerBubble
                if (tFill != null) timerFillImage = tFill.GetComponent<Image>();
                
            }
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Kein Canvas gefunden! Bitte stell sicher, dass der NPC einen Canvas als Child hat.");
        }
    }

    void Start()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }

        questionBubble.SetActive(false);
        timerBubble.SetActive(false);
    }

    void Update()
    {
        if (mainCamera != null)
        {
            if (canvas.activeSelf)
            {
                // 1. Billboarding (UI dreht sich zum Spieler)
                canvas.transform.LookAt(canvas.transform.position + mainCamera.forward);

                // 2. Skalierung basierend auf der Distanz
                float distanz = Vector3.Distance(canvas.transform.position, mainCamera.position);
                
                // Mathf.InverseLerp berechnet prozentual (0.0 bis 1.0), wo wir uns zwischen minDistance und maxDistance befinden
                float t = Mathf.InverseLerp(minDistance, maxDistance, distanz);
                
                // Mathf.Lerp wandelt diesen Prozentwert in die tatsächliche Skalierung (1 bis 2) um
                float aktuellesScale = Mathf.Lerp(minScale, maxScale, t);
                
                // Wende die neue Skalierung auf den Canvas an
                canvas.transform.localScale = new Vector3(aktuellesScale, aktuellesScale, aktuellesScale);
            }
        }

        if (currentState == OrderState.WaitingForFood)
        {
            currentWaitTime -= Time.deltaTime;
            UpdateTimerVisuals();

            if (currentWaitTime <= 0)
            {
                Debug.Log($"{gameObject.name} hat zu lange aufs Essen gewartet und ist wütend gegangen (Timer 2)!");
                
                currentState = OrderState.Done; 
                timerBubble.SetActive(false);
            }
        }
    }

    public void GenerateOrder()
    {
        currentState = OrderState.WantsToOrder;
        questionBubble.SetActive(true);
        timerBubble.SetActive(false);

        wantedToppings.Clear();
        int amountOfToppings = Random.Range(1, 3); 

        for (int i = 0; i < amountOfToppings; i++)
        {
            string randomTopping = availableToppings[Random.Range(0, availableToppings.Count)];
            if (!wantedToppings.Contains(randomTopping)) 
            {
                wantedToppings.Add(randomTopping);
            }
        }
    }

    public string GetOrderText()
    {
        string toppingText = string.Join(" und ", wantedToppings);
        return $"Hallo! Ich möchte bitte eine Pizza mit {toppingText}.";
    }

    public void AcceptOrder()
    {
        currentState = OrderState.WaitingForFood;
        
        if (NPCShoppingManager.Instance != null)
        {
            maxWaitTime = NPCShoppingManager.Instance.GetRandomGeduld();
        }
        else
        {
            maxWaitTime = 30f; 
        }

        currentWaitTime = maxWaitTime;

        questionBubble.SetActive(false);
        timerBubble.SetActive(true);
    }

    private void UpdateTimerVisuals()
    {
        float fillAmount = currentWaitTime / maxWaitTime;
        timerFillImage.fillAmount = fillAmount;

        if (fillAmount > 0.6f) timerFillImage.color = Color.green;
        else if (fillAmount > 0.3f) timerFillImage.color = Color.yellow;
        else timerFillImage.color = Color.red;
    }

    public void ResetOrder()
    {
        currentState = OrderState.Idle;
        wantedToppings.Clear();
        questionBubble.SetActive(false);
        timerBubble.SetActive(false);
    }
}