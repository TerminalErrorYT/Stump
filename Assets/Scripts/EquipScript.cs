using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipScript : MonoBehaviour
{
    public Transform PlayerTransform;
    public GameObject Hammer;
    public Camera Camera;
    private ItemFloat itemFloatScript;
    public float throwForwardForce = 5f;
    public float throwUpwardForce = 2f;

    // State tracking variable to prevent equipping and unequipping in the same frame
    public bool isEquipped = false;

    // Start is called before the first frame update
    void Start()
    {
		isEquipped = false;
        Hammer.GetComponent<Rigidbody>().isKinematic = false;
        itemFloatScript = Hammer.GetComponent<ItemFloat>();
		itemFloatScript.isHeld = false;
        itemFloatScript.ResetLandingState();

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // If we are holding it, press E to throw it
            if (isEquipped)
            {
                DropObject();
            }
            // If hands are empty, press E to try and pick it up
            else
            {
                EquipObject();
            }
        }
    }

    void DropObject()
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
        Quaternion rotationOffset = Quaternion.Euler(-110f, -10f, 20f); 
        Hammer.transform.rotation = PlayerTransform.transform.rotation * rotationOffset;
        Hammer.transform.SetParent(PlayerTransform);
    }
}
