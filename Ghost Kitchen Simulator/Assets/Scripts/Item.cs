using UnityEngine;

public class Item : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    [SerializeField] private bool canBePickedUp = true;
    public bool isBurnt = false; // Neue Variable, um den Zustand des Items zu verfolgen
    public bool CanBePickedUp => canBePickedUp;

    public void SetPickupEnabled(bool enabled)
    {
        canBePickedUp = enabled;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        if (!canBePickedUp) return;

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;
        transform.SetParent(holdPoint);

        Vector3 visualCenterOffset = Vector3.zero;
        if (GetComponent<Renderer>() != null)
        {
            visualCenterOffset = GetComponent<Renderer>().localBounds.center;
        }
        
        transform.localPosition = -visualCenterOffset;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 forceDirection, float force)
    {
        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;
        transform.SetParent(null);
        
        if (rb != null) rb.AddForce(forceDirection * force, ForceMode.Impulse);
    }
}