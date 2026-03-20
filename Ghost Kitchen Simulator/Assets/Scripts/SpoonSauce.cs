using UnityEngine;

public class SpoonSauce : MonoBehaviour
{
    public GameObject saucePrefab; // Drag your Sauce Sphere (with the Splat script) here
    public Transform spawnPoint;   // The tip of the spoon
    public float dropForce = 2f;    // Small push downwards

    void Update()
    {
        // Check for Left Mouse Click
        if (Input.GetMouseButtonDown(0)) 
        {
            DropSauce();
        }
    }

    void DropSauce()
    {
        // Create the drop
        GameObject newDrop = Instantiate(saucePrefab, spawnPoint.position, Quaternion.identity);
        
        // Give it a tiny nudge down so it doesn't just hang there
        Rigidbody rb = newDrop.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
        }
    }
}