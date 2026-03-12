using UnityEngine;

public class Item : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        rb.isKinematic = true; 
        col.enabled = false;   
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
        rb.isKinematic = false;
        col.enabled = true;
        transform.SetParent(null);
        
        rb.AddForce(forceDirection * force, ForceMode.Impulse);
    }
}