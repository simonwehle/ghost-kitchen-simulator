using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPC_StadtLeben : MonoBehaviour
{
    public enum NPCStatus { ImHaus, Unterwegs, ImGespraech, InWarteschlange }

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    [HideInInspector] public HouseManager meinHaus;

    [Header("Settings")]
    public float minWartezeitHaus = 10f;
    public float maxWartezeitHaus = 20f;
    public NPCStatus aktuellerStatus = NPCStatus.ImHaus;

    [Header("Wander-Settings")]
    public float minWanderZeit = 30f; 
    public float maxWanderZeit = 60f; 
    [Tooltip("Min/Max Pause at a Waypoint")]
    public Vector2 wegpunktPause = new Vector2(2f, 5f);

    [Header("Feintuning")]
    public float rotationsGeschwindigkeit = 720f;
    public float zielToleranz = 0.5f;
    public float ladenToleranz = 0.5f;
    public float warteToleranz = 0.2f;

    private Renderer[] allRenderers;
    private Collider[] allColliders;
    
    private float _animationBlend;
    private Coroutine hauptRoutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // GANZ WICHTIG: Wieder die alte, absolut sichere Methode für dein Setup!
        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();
    }

    void Start() { hauptRoutine = StartCoroutine(NPCHauptSchleife()); }

    void Update() { UpdateAnimation(); }

    private void UpdateAnimation()
    {
        if (animator == null || !animator.enabled) return;
        float targetSpeed = 0f;
        
        if ((aktuellerStatus == NPCStatus.Unterwegs || aktuellerStatus == NPCStatus.InWarteschlange) 
            && agent != null && agent.isOnNavMesh)
        {
            targetSpeed = agent.velocity.magnitude;
        }
        
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * 10f);
        if (_animationBlend < 0.01f) _animationBlend = 0f;
        
        animator.SetFloat("Speed", _animationBlend);
        animator.SetFloat("MotionSpeed", 1f); 
        animator.SetBool("Grounded", true);
    }

    public bool KannInteragieren()
    {
        if (aktuellerStatus == NPCStatus.ImHaus) return true;
        if (aktuellerStatus == NPCStatus.InWarteschlange)
        {
            return OrderManager.Instance != null && OrderManager.Instance.GetAktuellenKunden() == this;
        }
        return false;
    }

    // --- DIE SAUBERE STATE MACHINE ---
    IEnumerator NPCHauptSchleife()
    {
        // LÄUFT JETZT SICHER: Nur solange das GameObject aktiv und das Skript an ist
        while (isActiveAndEnabled)
        {
            if (aktuellerStatus == NPCStatus.ImHaus) yield return StartCoroutine(HausRoutine());
            else if (aktuellerStatus == NPCStatus.Unterwegs) yield return StartCoroutine(WanderRoutine());
            
            // Wenn ImGespraech oder InWarteschlange, warten wir einfach (wird extern gesteuert)
            yield return null; 
        }
    }

    IEnumerator HausRoutine()
    {
        yield return new WaitForSeconds(Random.Range(minWartezeitHaus, maxWartezeitHaus));
        if (aktuellerStatus == NPCStatus.ImHaus) ExitsHouse();
    }

    IEnumerator WanderRoutine()
    {
        Debug.Log($"[{gameObject.name}] Starting walking routine.");
        float geplanteWanderDauer = Random.Range(minWanderZeit, maxWanderZeit);
        float startZeit = Time.time;

        while (Time.time - startZeit < geplanteWanderDauer && aktuellerStatus == NPCStatus.Unterwegs)
        {
            if (OrderManager.Instance != null && Random.Range(0f, 100f) <= OrderManager.Instance.einkaufsWahrscheinlichkeit)
            {
                Debug.Log($"[{gameObject.name}] Impulse: Shopping!");
                yield return StartCoroutine(EinkaufsRoutine());
                yield break; // Bricht das Wandern ab, wenn er einkaufen geht
            }

            if (WaypointManager.Instance != null)
            {
                Transform ziel = WaypointManager.Instance.GetRandomWaypoint();
                if (ziel != null && agent != null && agent.isOnNavMesh)
                {
                    if (agent != null) agent.updateRotation = true;
                    yield return StartCoroutine(DreheZuPunkt(ziel.position));
                    
                    agent.isStopped = false;
                    agent.SetDestination(ziel.position);
                    
                    while (agent.pathPending || agent.remainingDistance > zielToleranz)
                    {
                        if (aktuellerStatus != NPCStatus.Unterwegs) yield break;
                        yield return null;
                    }
                    agent.velocity = Vector3.zero; 
                    yield return new WaitForSeconds(Random.Range(wegpunktPause.x, wegpunktPause.y));
                }
            }
            yield return null;
        }
        
        if (aktuellerStatus == NPCStatus.Unterwegs && meinHaus != null)
        {
            Debug.Log($"[{gameObject.name}] Walking routine finished. Going home.");
            yield return StartCoroutine(GeheNachHauseRoutine());
        }
    }

    public void BeendeGespraechUndGeheWeiter()
    {
        NPCStatus vorherigerStatus = aktuellerStatus;
        if (OrderManager.Instance != null) OrderManager.Instance.EntferneKunde(this);
        
        StopAllCoroutines(); 
        StartCoroutine(GespraechsEndeRoutine(vorherigerStatus));
    }

    IEnumerator GespraechsEndeRoutine(NPCStatus alterStatus)
    {
        yield return new WaitForSeconds(0.1f);
        
        if (agent != null) {
            agent.enabled = true;
            agent.updateRotation = true;
            agent.isStopped = false;
        }

        if (alterStatus == NPCStatus.InWarteschlange) aktuellerStatus = NPCStatus.Unterwegs;
        else
        {
            int wahl = Random.Range(0, 2); 
            if (wahl == 0 && meinHaus != null) EntersHouse();
            else aktuellerStatus = NPCStatus.Unterwegs;
        }
        
        hauptRoutine = StartCoroutine(NPCHauptSchleife());
    }

    public void SchaueSpielerAn(Transform spieler)
    {
        if (spieler == null) return;
        
        if (agent != null) { 
            agent.updateRotation = false; 
            agent.velocity = Vector3.zero; 
        }

        Vector3 richtung = (spieler.position - transform.position).normalized;
        richtung.y = 0;
        if (richtung.sqrMagnitude > 0.01f)
        {
            Quaternion zielRotation = Quaternion.LookRotation(richtung);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, zielRotation, rotationsGeschwindigkeit * Time.deltaTime);
        }
    }

    IEnumerator EinkaufsRoutine()
    {
        // 1. Zuerst nur das Ziel setzen, aber Status auf "Unterwegs" lassen!
        if (OrderManager.Instance.ladenEingang == null) 
        { 
            Debug.LogError($"[{gameObject.name}] No store entrance assigned!");
            yield break; 
        }

        Transform eingang = OrderManager.Instance.ladenEingang;
        if (agent != null && agent.isOnNavMesh) { 
            agent.updateRotation = true; 
            agent.isStopped = false; 
            agent.SetDestination(eingang.position); 
        }
        yield return null; 

        // 2. Warten, bis er WIRKLICH an der Ladentür angekommen ist - ELEGANT GELÖST
        yield return new WaitUntil(() => 
            agent != null && 
            !agent.pathPending && 
            agent.remainingDistance <= ladenToleranz && 
            Vector3.Distance(transform.position, eingang.position) <= ladenToleranz + 1.5f
        );

        agent.velocity = Vector3.zero; 

        // 3. Erst JETZT prüfen, ob im Laden Platz ist
        if (OrderManager.Instance.IstPlatzFrei())
        {
            Debug.Log($"[{gameObject.name}] Am Eingang angekommen: Platz frei.");
            
            // Erst wenn er sich anstellt, ändert sich der Status!
            Transform punkt = OrderManager.Instance.Anstellen(this);
            
            if (punkt != null)
            {
                aktuellerStatus = NPCStatus.InWarteschlange; // Status erst HIER setzen
                GeheZuWartepunkt(punkt);
                
                while (agent.pathPending || agent.remainingDistance > warteToleranz) yield return null;
                agent.velocity = Vector3.zero; 
                
                float meineGeduld = OrderManager.Instance.GetRandomGeduld();
                float warteBeginn = Time.time;
                
                while (aktuellerStatus == NPCStatus.InWarteschlange && (Time.time - warteBeginn) < meineGeduld)
                    yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            Debug.Log($"[{gameObject.name}] Laden voll. Gehe weiter.");
        }
        
        // 4. Aufräumen
        if (agent != null) agent.updateRotation = true; 
        OrderManager.Instance.EntferneKunde(this);
        aktuellerStatus = NPCStatus.Unterwegs;
    }

    public void BedienungErfolgt() 
    { 
        if (aktuellerStatus == NPCStatus.InWarteschlange) 
        {
            if (agent != null) agent.updateRotation = true; 
            aktuellerStatus = NPCStatus.Unterwegs; 
        }
    }

    public void GeheZuWartepunkt(Transform punkt)
    {
        if (agent != null && agent.isOnNavMesh) { 
            agent.updateRotation = true; 
            agent.isStopped = false; 
            agent.SetDestination(punkt.position); 
        }
    }

    IEnumerator GeheNachHauseRoutine()
    {
        if (meinHaus == null || meinHaus.tuerPunkt == null) yield break;
        
        if (agent != null) agent.updateRotation = true; 
        agent.SetDestination(meinHaus.tuerPunkt.position);
        
        while (agent.pathPending || agent.remainingDistance > zielToleranz) { 
            if (aktuellerStatus != NPCStatus.Unterwegs) yield break; 
            yield return null; 
        }
        EntersHouse();
    }

    public void EntersHouse()
    {
        aktuellerStatus = NPCStatus.ImHaus;
        if (agent != null && agent.isOnNavMesh) { agent.isStopped = true; agent.velocity = Vector3.zero; }
        if (meinHaus != null) meinHaus.RegisterResidentEntry(this);
        SetNPCPhysicalState(false);
    }

    public void ExitsHouse()
    {
        if (meinHaus != null && meinHaus.tuerPunkt != null) 
        {
            if (agent != null && agent.isOnNavMesh) { 
                agent.Warp(meinHaus.tuerPunkt.position); 
                agent.velocity = Vector3.zero; 
                agent.updateRotation = true; 
            }
            else transform.position = meinHaus.tuerPunkt.position;
            
            transform.rotation = meinHaus.tuerPunkt.rotation;
            meinHaus.RegisterResidentExit(this);
        }
        SetNPCPhysicalState(true);
        if (aktuellerStatus != NPCStatus.ImGespraech) aktuellerStatus = NPCStatus.Unterwegs;
    }

    IEnumerator DreheZuPunkt(Vector3 zielPunkt)
    {
        Vector3 richtung = (zielPunkt - transform.position).normalized; 
        richtung.y = 0;
        if (richtung.sqrMagnitude > 0.01f)
        {
            Quaternion zielRotation = Quaternion.LookRotation(richtung);
            float timeout = 0f;
            while (Quaternion.Angle(transform.rotation, zielRotation) > 5f && timeout < 1f)
            {
                if (agent != null) agent.updateRotation = false; 
                transform.rotation = Quaternion.Slerp(transform.rotation, zielRotation, Time.deltaTime * 10f);
                timeout += Time.deltaTime; 
                yield return null;
            }
            if (agent != null) agent.updateRotation = true; 
        }
    }

    public void SetNPCPhysicalState(bool isActive)
    {
        foreach (var r in allRenderers) if(r != null) r.enabled = isActive;
        foreach (var c in allColliders) if(c != null) c.enabled = isActive;
        
        if (animator != null) animator.enabled = isActive;
    }
}