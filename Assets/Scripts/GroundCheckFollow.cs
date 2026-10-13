using UnityEngine;

public class GenericGroundCheck : MonoBehaviour
{
    public Transform target;
    public LayerMask groundLayer;
    public float checkRadius = 0.5f;

    public bool IsGrounded { get; private set; }

    void Update()
    {
        if (target != null)
        {
            // Follow the target's position
            transform.position = target.position;
        }

        // Actively check if overlapping with the ground layer
        IsGrounded = Physics.CheckSphere(transform.position, checkRadius, groundLayer);
    }

    // Optional: Draw a visual gizmo in the editor to help debug the check size
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
