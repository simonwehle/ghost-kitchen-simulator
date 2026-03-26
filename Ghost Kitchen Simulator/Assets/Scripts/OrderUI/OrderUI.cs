using TMPro;
using UnityEngine;

public class OrderUI : MonoBehaviour
{
    public OrderManager orderManager;

    public TextMeshProUGUI[] orderTexts;

    void Start()
    {
        //UpdateUI();
    }

    public void Update()
    {
        for (int i = 0; i < orderTexts.Length; i++)
        {
            if (i < orderManager.activeOrders.Count)
            {
                orderTexts[i].text = orderManager.activeOrders[i].GetDisplayText();
            }
            else
            {
                orderTexts[i].text = "";
            }
        }
    }
}