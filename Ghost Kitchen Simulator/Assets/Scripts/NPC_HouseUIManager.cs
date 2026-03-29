using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class NPC_HouseUIManager : MonoBehaviour
{
    public static NPC_HouseUIManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject selectionPanel;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    private Coroutine blickCoroutine;
    private NPC_StadtLeben aktuellerGesprächsPartner;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (selectionPanel.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseMenu();
        }
        else if (aktuellerGesprächsPartner != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            BeendeGespraech();
        }
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
        npc.aktuellerStatus = NPC_StadtLeben.NPCStatus.ImGespraech;
        
        if (npc.meinHaus != null) npc.meinHaus.SpawnNPCForTalk(npc);

        selectionPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // BESSER: Nutzt jetzt den Singleton anstelle des langsamen FindGameObjectWithTag
        if (PlayerInteractionNPC.Instance != null) 
        {
            if (blickCoroutine != null) StopCoroutine(blickCoroutine);
            blickCoroutine = StartCoroutine(NPCBlicktZumSpieler(npc, PlayerInteractionNPC.Instance.transform));
        }
    }

    public void CloseMenu()
    {
        selectionPanel.SetActive(false);
        SetPlayerControl(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        if (PlayerInteractionNPC.Instance != null)
        {
            var input = PlayerInteractionNPC.Instance.GetComponent<PlayerInput>();
            if (input != null) 
            { 
                if (state) input.ActivateInput();
                else input.DeactivateInput();
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