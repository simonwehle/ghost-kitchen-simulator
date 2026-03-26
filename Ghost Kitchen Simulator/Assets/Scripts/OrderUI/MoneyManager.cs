using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public int currentMoney = 0;
    public int targetMoney = 100;

    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }
}