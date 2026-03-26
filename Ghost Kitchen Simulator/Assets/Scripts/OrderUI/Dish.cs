using UnityEngine;

[CreateAssetMenu(fileName = "Dish", menuName = "Game/Dish")]
public class Dish : ScriptableObject
{
    public string dishName;
    public int minAmount = 1;
    public int maxAmount = 3;
    public int rewardPerItem = 10;
}
