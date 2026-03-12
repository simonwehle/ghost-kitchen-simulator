using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HouseManager : MonoBehaviour
{
    public Transform tuerPunkt;
    public List<NPC_StadtLeben> alleBewohner;
    //public UnityEvent<List<NPC_StadtLeben>> OnMultipleResidentsUI;
    private List<NPC_StadtLeben> _bewohnerAktuellZuhause = new List<NPC_StadtLeben>();

    void Start()
    {
        foreach (var npc in alleBewohner)
        {
            if (npc != null)
            {
                npc.meinHaus = this;
                npc.EntersHouse();
            }
        }
    }

    public void RegisterResidentEntry(NPC_StadtLeben npc)
    {
        if (!_bewohnerAktuellZuhause.Contains(npc)) _bewohnerAktuellZuhause.Add(npc);
    }

    public void RegisterResidentExit(NPC_StadtLeben npc)
    {
        if (_bewohnerAktuellZuhause.Contains(npc)) _bewohnerAktuellZuhause.Remove(npc);
    }

    public void KnockOnDoor()
    {
        if (_bewohnerAktuellZuhause.Count == 0)
        {
            Debug.Log("Niemand zu Hause.");
            return;
        }
        
        // Auch bei nur einer Person rufen wir jetzt die UI auf, 
        // damit die Interaktions-Logik (Sperren, Drehen) startet.
        //OnMultipleResidentsUI.Invoke(_bewohnerAktuellZuhause);
        NPC_UIManager.Instance.ShowResidentSelection(_bewohnerAktuellZuhause);
    }

    public void SpawnNPCForTalk(NPC_StadtLeben npc)
    {
        npc.StopAllCoroutines(); 
        npc.aktuellerStatus = NPC_StadtLeben.NPCStatus.ImGespraech;
        npc.ExitsHouse(); // Macht ihn sichtbar und setzt ihn an die Tür
        
        if (npc.agent != null && npc.agent.isOnNavMesh) 
        {
            // Den Agenten sauber stoppen statt nur die Werte auf 0 zu setzen
            npc.agent.isStopped = true;
            npc.agent.velocity = Vector3.zero;
        }
    }

    public bool IstJemandZuhause()
    {
        return _bewohnerAktuellZuhause.Count > 0;
    }
}