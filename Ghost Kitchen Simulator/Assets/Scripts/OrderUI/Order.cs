[System.Serializable]
public class Order
{
    public Dish dish;
    public int amount;

    public Order(Dish dish, int amount)
    {
        this.dish = dish;
        this.amount = amount;
    }

    public string GetDisplayText()
    {
        return amount + "x " + dish.dishName;
    }

    public int GetReward()
    {
        return amount * dish.rewardPerItem;
    }
}