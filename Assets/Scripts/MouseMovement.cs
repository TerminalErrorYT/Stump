using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 1. Add the Input System namespace
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    // NOTE: You may need to lower this value in the Inspector (e.g., 0.1f to 1f) 
    // because the New Input System returns raw pixel delta movements.
    public float mouseSensitivity = 0.5f;
 
    float xRotation = 0f;
    float YRotation = 0f;
 
    void Start()
    {
        // Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }
 
    void Update()
    {
        float mouseX = 0f;
        float mouseY = 0f;

        // 2. Read mouse delta using the New Input System
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseX = mouseDelta.x * mouseSensitivity;
            mouseY = mouseDelta.y * mouseSensitivity;
        }

        // Control rotation around x axis (Look up and down)
        xRotation -= mouseY;
 
        // We clamp the rotation so we cant Over-rotate
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
 
        // Control rotation around y axis (Look left and right)
        YRotation += mouseX;
 
        // Applying both rotations
        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);
    }
}