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
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(float throwForce = 2f)
    {
        rb.isKinematic = false;
        col.enabled = true;
        transform.SetParent(null);
        
        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }
}