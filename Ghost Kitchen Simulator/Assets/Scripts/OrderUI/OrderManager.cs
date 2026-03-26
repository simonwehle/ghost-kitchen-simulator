using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<Dish> dishPool;
    public int maxOrders = 3;

    public List<Order> activeOrders = new List<Order>();

    void Start()
    {
        GenerateOrders();
    }

    public void GenerateOrders()
    {
        activeOrders.Clear();

        for (int i = 0; i < maxOrders; i++)
        {
            Dish randomDish = dishPool[Random.Range(0, dishPool.Count)];
            int amount = Random.Range(randomDish.minAmount, randomDish.maxAmount + 1);

            activeOrders.Add(new Order(randomDish, amount));
        }
    }
}