using UnityEngine;

// Attach this to every topping prefab.
// Links the physical topping GameObject to its ToppingType data asset.
public class ToppingItem : MonoBehaviour
{
    public ToppingType toppingType;
}
