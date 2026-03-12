using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPC_StadtLeben : MonoBehaviour
{
    public enum NPCStatus { ImHaus, Unterwegs, ImGespraech }

    [Header("Referenzen")]
    public NavMeshAgent agent;
    public Animator animator;
    [HideInInspector] public HouseManager meinHaus;
    public List<Transform> wegpunkte;

    [Header("Einstellungen")]
    public float minWartezeitHaus = 10f;
    public float maxWartezeitHaus = 20f;
    public NPCStatus aktuellerStatus = NPCStatus.ImHaus;

    [Header("Wander-Einstellungen")]
public float minWanderZeit = 30f; // Mindestens 30 Sekunden wandern
public float maxWanderZeit = 60f; // Maximal 60 Sekunden wandern

    private Renderer[] allRenderers;
    private Collider[] allColliders;
    private float _animationBlend;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        allRenderers = GetComponentsInChildren<Renderer>();
        allColliders = GetComponentsInChildren<Collider>();
    }

    void Start() { StartCoroutine(NPCRoutine()); }

    void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null || !animator.enabled) return;

        float targetSpeed = 0f;
        if (aktuellerStatus == NPCStatus.Unterwegs && agent != null && agent.isOnNavMesh)
        {
            targetSpeed = agent.velocity.magnitude;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * 10f);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        animator.SetFloat("Speed", _animationBlend);
        animator.SetFloat("MotionSpeed", 1f); 
        animator.SetBool("Grounded", true);
        animator.SetBool("Jump", false);
        animator.SetBool("FreeFall", false);
    }

    IEnumerator NPCRoutine()
    {
        while (true)
        {
            if (aktuellerStatus == NPCStatus.ImHaus)
            {
                yield return new WaitForSeconds(Random.Range(minWartezeitHaus, maxWartezeitHaus));
                if (aktuellerStatus == NPCStatus.ImHaus) ExitsHouse();
            }

            if (aktuellerStatus == NPCStatus.Unterwegs)
            {
                // 1. Bestimme zufällige Dauer für diesen Ausflug
                float geplanteWanderDauer = Random.Range(minWanderZeit, maxWanderZeit);
                float startZeit = Time.time;

                // 2. Wandere solange die Zeit nicht abgelaufen ist
                while (Time.time - startZeit < geplanteWanderDauer && aktuellerStatus == NPCStatus.Unterwegs)
                {
                    if (wegpunkte.Count > 0 && agent != null && agent.isOnNavMesh)
                    {
                        Transform ziel = wegpunkte[Random.Range(0, wegpunkte.Count)];
                        
                        yield return StartCoroutine(DreheZuPunkt(ziel.position));
                        
                        agent.isStopped = false;
                        agent.SetDestination(ziel.position);
                        
                        // Warten bis Ziel erreicht oder Status sich ändert
                        while (agent.pathPending || agent.remainingDistance > 0.5f)
                        {
                            if (aktuellerStatus != NPCStatus.Unterwegs) yield break;
                            yield return null;
                        }

                        // Kurze Pause am Wegpunkt
                        yield return new WaitForSeconds(Random.Range(2f, 5f));
                    }
                    else 
                    {
                        yield return null; 
                    }
                }

                // 3. Zeit ist um -> Gehe zum Haus
                if (aktuellerStatus == NPCStatus.Unterwegs && meinHaus != null)
                {
                    yield return StartCoroutine(GeheNachHauseRoutine());
                }
            }
            yield return null;
        }
    }

    IEnumerator GeheNachHauseRoutine()
    {
        if (meinHaus == null || meinHaus.tuerPunkt == null) yield break;

        Vector3 hausPos = meinHaus.tuerPunkt.position;
        yield return StartCoroutine(DreheZuPunkt(hausPos));
        
        agent.isStopped = false;
        agent.SetDestination(hausPos);

        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            if (aktuellerStatus != NPCStatus.Unterwegs) yield break;
            yield return null;
        }

        EntersHouse();
    }

    public void EntersHouse()
    {
        aktuellerStatus = NPCStatus.ImHaus;
        if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
        if (meinHaus != null) meinHaus.RegisterResidentEntry(this);
        SetNPCPhysicalState(false);
    }

    public void ExitsHouse()
    {
        if (meinHaus != null && meinHaus.tuerPunkt != null)
        {
            transform.position = meinHaus.tuerPunkt.position;
            transform.rotation = meinHaus.tuerPunkt.rotation;
            meinHaus.RegisterResidentExit(this);
        }
        
        SetNPCPhysicalState(true);
        
        if (agent != null && agent.isOnNavMesh)
        {


            agent.updateRotation = true;
            if (aktuellerStatus != NPCStatus.ImGespraech)
            {
                agent.isStopped = false;
                aktuellerStatus = NPCStatus.Unterwegs;
            }
        }
    }

    public void BeendeGespraechUndGeheWeiter()
    {
        if (aktuellerStatus == NPCStatus.ImGespraech)
        {
            StartCoroutine(GespraechsEndeRoutine());
        }
    }

    // --- NEU: Kurze Pause nach dem Gespräch ---
    IEnumerator GespraechsEndeRoutine()
    {
        // 1 Sekunde warten, damit es natürlicher wirkt
        yield return new WaitForSeconds(1.0f);

        int wahl = Random.Range(0, 2); 
        if (wahl == 0) 
        {
            EntersHouse();
        }
        else
        {
            aktuellerStatus = NPCStatus.Unterwegs;
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.updateRotation = true;

            }
            StopAllCoroutines();
            StartCoroutine(NPCRoutine());
        }
    }

    // --- NEU: Hilfs-Funktion für sauberes Drehen ---
    IEnumerator DreheZuPunkt(Vector3 zielPunkt)
    {
        Vector3 richtung = (zielPunkt - transform.position);
        richtung.y = 0;
        
        if (richtung.sqrMagnitude > 0.1f)
        {
            Quaternion zielRotation = Quaternion.LookRotation(richtung);
            float timeout = 0f;
            
            // Drehe den NPC max. 1 Sekunde lang in die neue Richtung
            while (Quaternion.Angle(transform.rotation, zielRotation) > 5f && timeout < 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, zielRotation, Time.deltaTime * 10f);
                timeout += Time.deltaTime;
                yield return null;
            }
        }
    }

    public void SetNPCPhysicalState(bool isActive)
    {
        foreach (var r in allRenderers) r.enabled = isActive;
        foreach (var c in allColliders) c.enabled = isActive;
        if (animator != null) 
        { 
            animator.enabled = isActive; 
            if (isActive) 
            {
                animator.SetBool("Grounded", true);
                animator.SetFloat("MotionSpeed", 1f);
            }
        }
    }

    public void SchaueSpielerAn(Transform spieler)
    {
        Vector3 richtung = spieler.position - transform.position;
        richtung.y = 0;
        if (richtung.sqrMagnitude > 0.01f)
        {
            Quaternion zielRotation = Quaternion.LookRotation(richtung);
            transform.rotation = Quaternion.Slerp(transform.rotation, zielRotation, Time.deltaTime * 5f);
        }
    }
}