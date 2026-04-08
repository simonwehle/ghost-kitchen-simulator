using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public MoneyManager moneyManager;
    public TextMeshProUGUI moneyText;

    void Update()
    {
        moneyText.text = "Money: "
            + moneyManager.currentMoney
            + "$";
    }
}