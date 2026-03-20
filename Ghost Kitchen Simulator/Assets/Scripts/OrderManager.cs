using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Laden-Eingang")]
    [Tooltip("Ziehe hier das GameObject rein, das an der Ladentür liegt.")]
    public Transform ladenEingang;

    [Header("Wartepositionen")]
    [Tooltip("Ziehe hier die 3 Child-Objekte (Wegpunkte) am Tresen rein.")]
    public Transform[] schlangePunkte; 

    [Header("Shop-Einstellungen")]
    [Tooltip("Wie hoch ist die Chance, dass ein wandernder NPC einkaufen will?")]
    [Range(0, 100)] public float einkaufsWahrscheinlichkeit = 20f;
    
    [Tooltip("Wie lange wartet ein Kunde am Tresen?")]
    public float minKundenGeduld = 20f;
    public float maxKundenGeduld = 40f;

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

    [ContextMenu("Ersten Kunden bedienen")]
    public void BedienteErstenKunden()
    {
        NPC_StadtLeben kunde = GetAktuellenKunden();
        if (kunde != null) kunde.BedienungErfolgt();
        else Debug.Log("Kein Kunde in der Schlange!");
    }
}