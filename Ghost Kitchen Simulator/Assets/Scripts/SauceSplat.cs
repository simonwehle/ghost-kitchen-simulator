using UnityEngine;
using System.Collections;

public class SauceSplat : MonoBehaviour
{
    [Header("Scaling")]
    public float horizontalSpread = 3.5f; // How wide it gets
    public float verticalFlatness = 0.02f; // How thin it gets
    public float spreadSpeed = 7f;

    private bool hasLanded = false;
    private Rigidbody rb;
    private Collider myCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        
        // Random rotation for a natural look
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded)
        {
            // 1. Check if we hit the dough or the table (not another drop)
            // Note: If you want to be strict, check: if(collision.gameObject.CompareTag("FlatDough"))
            
            hasLanded = true;
            
            // 2. Freeze movement
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;

            // 3. THE TRICK: Disable the collider!
            // This way, the NEXT drop will fall right through this one 
            // and hit the pizza dough instead.
            myCollider.enabled = false;

            // 4. Align to surface
            transform.up = collision.contacts[0].normal;

            StartCoroutine(FlattenRoutine());
        }
    }

    IEnumerator FlattenRoutine()
    {
        // We want it big on X and Z, but tiny on Y
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(startScale.x * horizontalSpread, verticalFlatness, startScale.z * horizontalSpread);
        
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * spreadSpeed;
            // Smoothly change the size
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            
            // Move slightly up during scaling to prevent clipping into the floor
            transform.position += new Vector3(0, 0.001f, 0); 
            
            yield return null;
        }
    }
}