using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipScript : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public Transform PlayerTransform;
    public GameObject Hammer;
    public Camera Camera;
    private ItemFloat itemFloatScript;
    public float throwForwardForce = 5f;
    public float throwUpwardForce = 2f;
    public float range = 2.5f;

    public bool isEquipped = false; // State tracking variable to prevent equipping and unequipping in the same frame
    public bool readyToThrow; //looking to see when the player is holding, if holding before throw then true, if holding after catch, then false
    public float strengthModifier = 0; //strength modifier will add a fraction of the original force ontop of the original throw to allow it to be thrown even higher
    private bool isInCatchZone = false;
    private bool isInPerfectZone = false;

    void Start() // Start is called before the first frame update
    {
	    isEquipped = false;
        Hammer.GetComponent<Rigidbody>().isKinematic = false;
        itemFloatScript = Hammer.GetComponent<ItemFloat>();
	    itemFloatScript.isHeld = false;
        itemFloatScript.ResetLandingState();
	    readyToThrow = true;
    }

    void Update() { // Update is called once per frame
        if (Keyboard.current.eKey.wasPressedThisFrame) {
            if (isEquipped) { // If we are holding it, press E to throw it
                DropObject();
            } else { // If hands are empty, press E to try and pick it up
                EquipObject();
            }
        }

		if (isEquipped && readyToThrow) {
	    	if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
	  			Throw();
	    	}
    	} else if (!isEquipped) { //add a check for if the hammer has hit the ground
	    	if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
	  			Catch();
	    	}
    	}
    }

    private void OnTriggerEnter(Collider hammer)
    {
        if (hammer.CompareTag("CatchZone"))
        {
            isInCatchZone = true;
            Debug.Log("Is Catch = " + isInCatchZone);
        }
        else if (hammer.CompareTag("PerfectZone"))
        {
            isInPerfectZone = true;
            Debug.Log("Is Perfect = " + isInPerfectZone);
        }
    }

    private void OnTriggerExit(Collider hammer)
    {
        if (hammer.CompareTag("CatchZone")) {
            isInCatchZone = false;
            Debug.Log("Is Catch = " + isInCatchZone);
        }
        else if (hammer.CompareTag("PerfectZone"))
        {
            isInPerfectZone = false;
            Debug.Log("Is Perfect = " + isInPerfectZone);
        }
    }

    void DropObject() { //player drops the hammer on the ground
        isEquipped = false; // Update state

        if (itemFloatScript != null) {
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

    void EquipObject() { //player can grab the item from the ground
		RaycastHit hit;
		if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, range)) {
            isEquipped = true; // Update state

            if (itemFloatScript != null) {
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

    private void Throw() { //player throws the hammer up in the air to be caught
    	readyToThrow = false;
    	isEquipped = false;
    	float throwUpwardForce = 25f; //change this to change the height the hammer will be thrown

    	if (itemFloatScript != null) {
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
        
    	Vector3 tossDirection = (Camera.transform.forward) + (Vector3.up * throwUpwardForce); //hammer throw direction
    	hammerRb.AddForce(tossDirection, ForceMode.Impulse); //hammer throw power
    	hammerRb.AddTorque(Camera.transform.right * 1f, ForceMode.Impulse); //hammer spin
    }

    private void Catch() { //player catches the item once it reaches their catch range
        if (isInCatchZone) {
            isEquipped = true; // Update state

            if (itemFloatScript != null) {
                itemFloatScript.isHeld = true;
                itemFloatScript.ResetLandingState();
                itemFloatScript.enabled = true; // Make sure it's active when held if needed
            }

            Hammer.GetComponent<Rigidbody>().isKinematic = true;
            Hammer.transform.position = PlayerTransform.transform.position;
            Quaternion rotationOffset = Quaternion.Euler(-110f, -10f, 20f);
            Hammer.transform.rotation = PlayerTransform.transform.rotation * rotationOffset;
            Hammer.transform.SetParent(PlayerTransform);

            if (isInPerfectZone) {
                //give 100% (and maybe a small bonus) for catching the hammer perfectly
                Debug.Log("Perfect Catch!");
            } else {
                // calculate how far the hammer was from the perfect zone and give a percentage of the power for the swing
                Debug.Log("Git Gud");
            }

            // swingHammer(); the function allowing the user to swing the hammer after catching and NOT throw a second time

            readyToThrow = true; //should only activate after swinging the hammer
        }
    }

    private void MissedCatch() {
		//create another class for when the hammer hits the ground and needs reset/ passed to the next player
		
		readyToThrow = true;
    }
	
    private void ProximityDetector() {
		//create another class that will warn the player that they are getting close/leaving the area where the hammer is going to fall, maybe puts an indicator on the ground on where its going to fall

    }

    //create another class or file that calculates the velocity of the hammer as it falls
}
