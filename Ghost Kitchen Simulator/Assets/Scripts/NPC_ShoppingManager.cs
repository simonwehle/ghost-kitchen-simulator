using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCShoppingManager : MonoBehaviour
{
    public static NPCShoppingManager Instance { get; private set; }

    [Header("Store Entry")]
    [Tooltip("Drag the GameObject located at the shop door here.")]
    public Transform ladenEingang;

    [Header("Waypoints for the queue")]
    [Tooltip("Drag the 3 Child-Objects (Waypoints) to the counter here.")]
    public Transform[] schlangePunkte; 

    [Header("Shop-Settings")]
    [Tooltip("How high is the chance that a wandering NPC wants to shop?")]
    [Range(0, 100)] public float einkaufsWahrscheinlichkeit = 20f;
    
    [Tooltip("How long does a customer wait at the counter?")]
    public float minKundenGeduld = 90f;
    public float maxKundenGeduld = 120f;

    private List<NPC_StadtLeben> wartendeNPCs = new List<NPC_StadtLeben>();

    private void Awake()
    {
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);
    }

    public float GetRandomGeduld()
    {
        return Random.Range(minKundenGeduld, maxKundenGeduld);
    }

    public bool IstPlatzFrei()
    {
        return wartendeNPCs.Count < schlangePunkte.Length;
    }

    public Transform Anstellen(NPC_StadtLeben npc)
    {
        if (!IstPlatzFrei()) return null;

        if (!wartendeNPCs.Contains(npc)) wartendeNPCs.Add(npc);

        int index = wartendeNPCs.IndexOf(npc);
        
        // --- NEUES LOG: Zeigt an, welcher Platz neu belegt wurde ---
        // (index + 1), damit in der Konsole "Platz 1" statt "Platz 0" steht, liest sich besser!
        Debug.Log($"[OrderManager] Warteplatz {index + 1} belegt von: {npc.gameObject.name}");
        
        return schlangePunkte[index];
    }

    public void EntferneKunde(NPC_StadtLeben npc)
    {
        if (wartendeNPCs.Contains(npc))
        {
            wartendeNPCs.Remove(npc);
            RückeNPCsNach(); 
        }
    }

    private void RückeNPCsNach()
    {
        for (int i = 0; i < wartendeNPCs.Count; i++)
        {
            if (wartendeNPCs[i] != null)
            {
                Transform neuerPunkt = schlangePunkte[i];
                
                // --- NEUES LOG: Zeigt an, wer auf welchen Platz vorrückt ---
                Debug.Log($"[OrderManager] Nachrücken: Warteplatz {i + 1} ist nun belegt von: {wartendeNPCs[i].gameObject.name}");
                
                StartCoroutine(VerzoegertesNachruecken(wartendeNPCs[i], neuerPunkt));
            }
        }
    }

    private IEnumerator VerzoegertesNachruecken(NPC_StadtLeben npc, Transform punkt)
    {
        yield return new WaitForSeconds(1.0f); 
        
        if (npc != null && wartendeNPCs.Contains(npc))
        {
            npc.GeheZuWartepunkt(punkt);
        }
    }

    public NPC_StadtLeben GetAktuellenKunden()
    {
        if (wartendeNPCs.Count > 0) return wartendeNPCs[0];
        return null;
    }

    [ContextMenu("Serve First Customer")]
    public void BedienteErstenKunden()
    {
        NPC_StadtLeben kunde = GetAktuellenKunden();
        if (kunde != null) kunde.BedienungErfolgt();
        else Debug.Log("No customer in the queue!");
    }
}