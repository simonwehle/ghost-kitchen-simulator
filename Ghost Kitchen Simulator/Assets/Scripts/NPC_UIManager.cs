using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class NPC_UIManager : MonoBehaviour
{

    public static NPC_UIManager Instance { get; private set; }

    void Awake()
    {
        // Wenn noch kein Manager existiert, wird dieser zum globalen Chef ernannt
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Falls du aus Versehen einen zweiten UI-Manager in die Szene ziehst, zerstört er sich selbst
            Destroy(gameObject); 
        }
    }

    public GameObject selectionPanel;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    private Coroutine blickCoroutine;
    private NPC_StadtLeben aktuellerGesprächsPartner;

    void Update()
    {
        if (selectionPanel.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseMenu();
        else if (aktuellerGesprächsPartner != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            BeendeGespraech();
    }

    public void ShowResidentSelection(List<NPC_StadtLeben> bewohner)
    {
        foreach (Transform child in buttonContainer) Destroy(child.gameObject);
        selectionPanel.SetActive(true);
        
        SetPlayerControl(false); 
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        foreach (var npc in bewohner)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = npc.gameObject.name;
            newButton.GetComponent<Button>().onClick.AddListener(() => OnResidentSelected(npc));
        }
    }

    void OnResidentSelected(NPC_StadtLeben npc)
    {
        aktuellerGesprächsPartner = npc;
        npc.meinHaus.SpawnNPCForTalk(npc);
        selectionPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameObject spieler = GameObject.FindGameObjectWithTag("Player");
        if (spieler != null) 
        {
            if (blickCoroutine != null) StopCoroutine(blickCoroutine);
            blickCoroutine = StartCoroutine(NPCBlicktZumSpieler(npc, spieler.transform));
        }
    }

    public void CloseMenu()
    {
        selectionPanel.SetActive(false);
        SetPlayerControl(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        aktuellerGesprächsPartner = null;
    }

    public void BeendeGespraech()
    {
        if (aktuellerGesprächsPartner != null)
        {
            if (blickCoroutine != null) StopCoroutine(blickCoroutine);
            aktuellerGesprächsPartner.BeendeGespraechUndGeheWeiter();
            aktuellerGesprächsPartner = null;
        }
        CloseMenu();
    }

    private void SetPlayerControl(bool state)
    {
        GameObject spieler = GameObject.FindGameObjectWithTag("Player");
        

        if (spieler != null)
        {
            var input = spieler.GetComponent<PlayerInput>();
            
            if (input != null) 
            { 
                if (state) 
                {
                    input.ActivateInput();
                } 
                else 
                {
                    input.DeactivateInput();
                } 
            }
        }
    }

    System.Collections.IEnumerator NPCBlicktZumSpieler(NPC_StadtLeben npc, Transform ziel)
    {
        if (npc.agent != null) npc.agent.updateRotation = false;
        while (npc != null && npc.aktuellerStatus == NPC_StadtLeben.NPCStatus.ImGespraech)
        {
            npc.SchaueSpielerAn(ziel);
            yield return null;
        }
        if (npc != null && npc.agent != null) npc.agent.updateRotation = true;
    }
}