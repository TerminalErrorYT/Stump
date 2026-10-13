using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipScript : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameObject Hammer;
    public Camera Camera;
    public float range = 2f;
    public float open = 100f;
    private ItemFloat itemFloatScript;
    public float throwForwardForce = 5f;
    public float throwUpwardForce = 2f;
    public float hammerGravityMultiplier = 9.81f;

    // State tracking variable to prevent equipping and unequipping in the same frame
    private bool isEquipped = false;

    // Start is called before the first frame update
    void Start()
    {
        Hammer.GetComponent<Rigidbody>().isKinematic = true;
        itemFloatScript = Hammer.GetComponent<ItemFloat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            // If we are holding it, press F to throw it
            if (isEquipped)
            {
                UnequipObject();
            }
            // If hands are empty, press F to try and pick it up
            else
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                EquipObject();
            }
        }
    }

    void UnequipObject()
    {
        isEquipped = false; // Update state

        if (itemFloatScript != null)
        {
            itemFloatScript.isHeld = false;
            itemFloatScript.ResetLandingState();
        }

        Hammer.transform.SetParent(null);
        
        Rigidbody hammerRb = Hammer.GetComponent<Rigidbody>();
        hammerRb.isKinematic = false;
        hammerRb.useGravity = true;
        
        // Reset old speeds
        hammerRb.linearVelocity = Vector3.zero; 
        hammerRb.angularVelocity = Vector3.zero;
        
        Vector3 tossDirection = (Camera.transform.forward * throwForwardForce) + (Vector3.up * throwUpwardForce);
        hammerRb.AddForce(tossDirection, ForceMode.Impulse);
	hammerRb.AddTorque(Camera.transform.right * 1f, ForceMode.Impulse);

	Vector3 extraGravityForce = Vector3.down * hammerGravityMultiplier;
        hammerRb.AddForce(extraGravityForce, ForceMode.VelocityChange);
    }

    void EquipObject()
    {
        isEquipped = true; // Update state

        if (itemFloatScript != null)
        {
            itemFloatScript.isHeld = true;
	    itemFloatScript.ResetLandingState();
            itemFloatScript.enabled = true; // Make sure it's active when held if needed
        }

        Hammer.GetComponent<Rigidbody>().isKinematic = true;
        Hammer.transform.position = PlayerTransform.transform.position;
        Quaternion rotationOffset = Quaternion.Euler(-110f, -10f, 0f); 
        Hammer.transform.rotation = PlayerTransform.transform.rotation * rotationOffset;
        Hammer.transform.SetParent(PlayerTransform);
    }
}
