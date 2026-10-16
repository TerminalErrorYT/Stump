using UnityEngine;
using System.Collections;

public class ItemFloat : MonoBehaviour
{
    private Rigidbody rb;

    public float floatSpeed = 2.0f;     
    public float floatAmplitude = 0.25f; 
    public float rotationSpeed = 90.0f;

    [Header("Landing Settings")]
    [Tooltip("Time in seconds to wait after a complete stop before floating begins")]
    public float floatDelay = 0.5f;

    public bool isHeld = true; 
    private bool hasLanded = false;
    private float groundY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
	if (!isHeld && !hasLanded && rb != null)
        {
            rb.AddForce(Vector3.down, ForceMode.Acceleration);
        }
        
        if (isHeld) return;

        
        if (hasLanded)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            float newY = groundY + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    // Unity automatically calls this the exact frame the hammer hits a surface
    private void OnCollisionEnter(Collision collision)
    {
        // If it's already landed or currently held, ignore the collision
        if (hasLanded || isHeld) return;

        // Start the sequence to wait for it to rest, pause, and float
        StartCoroutine(WaitAndFloatRoutine());
    }

    private IEnumerator WaitAndFloatRoutine()
    {
        // A standard fallback threshold: we wait until it mostly stops moving
        // (Handles Unity 6 linearVelocity and older velocity variables safely)
        Vector3 currentVelocity = rb != null ? rb.linearVelocity : Vector3.zero;

        // Loop until the hammer stops bouncing/rolling
        while (rb != null && !rb.IsSleeping() && currentVelocity.magnitude > 0.05f)
        {
            currentVelocity = rb.linearVelocity;
            yield return new WaitForFixedUpdate(); // Check during physics frames
        }

        // --- STEP 1: The hammer has physically come to a stop. Wait the requested delay time. ---
        yield return new WaitForSeconds(floatDelay);

        // Double check the player didn't pick it up during the delay window
        if (!isHeld)
        {
            // --- STEP 2: Transition into the magical hover state ---
            hasLanded = true;
            
            if (rb != null)
            {
                rb.isKinematic = true; // Turn off physics entirely
            }
            
            groundY = transform.position.y + 0.5f; // Set base height for hovering
            transform.rotation = Quaternion.Euler(-45f, 0f, 45f); // Snap upright cleanly
        }
    }

    public void ResetLandingState()
    {
        StopAllCoroutines(); // Stop any pending float routines
        hasLanded = false;
    }
}
