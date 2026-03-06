using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPC_StadtLeben : MonoBehaviour
{
    public enum NPCStatus { ImHaus, AufGehweg, GehtHeim }

    [Header("Referenzen")]
    public NavMeshAgent agent;
    public Animator animator; // HIER NEU: Referenz zum Animator
    public Transform hausTuer; 
    public List<Transform> wegpunkte; 

    [Header("Zeiteinstellungen")]
    public float wartezeitImHaus = 5f;
    public float zeitDraussenBisHeimweg = 20f;

    private NPCStatus aktuellerStatus = NPCStatus.ImHaus;
    private float tagesTimer;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        // Falls der Animator nicht zugewiesen wurde, suchen wir ihn
        if (animator == null) animator = GetComponent<Animator>();
        
        StartCoroutine(NPCLogik());
    }

    void Update()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        // Wir berechnen die Geschwindigkeit basierend auf der Bewegung des Agents
        // magnitude gibt uns die reine Meter/Sekunde Zahl
        float speed = agent.velocity.magnitude;

        // Wir setzen die Parameter, die der Starter Asset Controller erwartet:
        animator.SetFloat("Speed", speed);
        
        // Da der NPC fest auf dem Boden steht, setzen wir Grounded auf true, 
        // damit er nicht in der "Fall"-Animation stecken bleibt.
        animator.SetBool("Grounded", true);

        // Optional: MotionSpeed wird oft für die Schritt-Animation-Geschwindigkeit genutzt
        animator.SetFloat("MotionSpeed", 1f); 
    }

    IEnumerator NPCLogik()
    {
        while (true)
        {
            switch (aktuellerStatus)
            {
                case NPCStatus.ImHaus:
                    // Wenn im Haus, Geschwindigkeit im Animator auf 0 erzwingen
                    yield return new WaitForSeconds(wartezeitImHaus);
                    
                    aktuellerStatus = NPCStatus.AufGehweg;
                    tagesTimer = 0;
                    WähleNeuenWegpunkt();
                    break;

                case NPCStatus.AufGehweg:
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        WähleNeuenWegpunkt();
                    }

                    tagesTimer += Time.deltaTime;
                    if (tagesTimer >= zeitDraussenBisHeimweg)
                    {
                        aktuellerStatus = NPCStatus.GehtHeim;
                        agent.SetDestination(hausTuer.position);
                    }
                    break;

                case NPCStatus.GehtHeim:
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        aktuellerStatus = NPCStatus.ImHaus;
                        Debug.Log("NPC ist wieder im Haus.");
                    }
                    break;
            }
            yield return null;
        }
    }

    void WähleNeuenWegpunkt()
    {
        if (wegpunkte.Count > 0)
        {
            int index = Random.Range(0, wegpunkte.Count);
            agent.SetDestination(wegpunkte[index].position);
        }
    }
}