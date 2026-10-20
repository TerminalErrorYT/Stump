using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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

	[Header("References")]
  	public Transform cam;
  	public Transform attackPoint;
  	public GameObject objectToThrow;

  	[Header("Settings")]
  	public int totalThrows;
  	public float throwCooldown;

  	[Header("Throwing")]
  	public float throwForce;
  	public float throwUpwardForce;

  	bool readyToThrow;

    // State tracking variable to prevent equipping and unequipping in the same frame
    private bool isEquipped = false;

    // Start is called before the first frame update
    void Start()
    {
		isEquipped = false; //might be able to remove
        Hammer.GetComponent<Rigidbody>().isKinematic = false;
        itemFloatScript = Hammer.GetComponent<ItemFloat>();
		itemFloatScript.isHeld = false;
        itemFloatScript.ResetLandingState();
		readyToThrow = false;
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
                Shoot(); //might be able to skip shoot and go straight to equip 
				readyToThrow = true;
            }
        }

		if (Keyboard.current.eKey.wasPressedThisFrame)
	    {
	      readyToThrow = true;
	    }
	    
	    if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
	    {
	      Throw();
	    }
    }

    void Shoot() //might be able to remove
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

	private void Throw() //update so it throws up not forawrd, hammer shouldn't really move around, should just go up and down for throw
  	{
    	readyToThrow = false;

    	GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
    	Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

    	Vector3 forceDirection = cam.transform.forward; //change this to upward
    	RaycastHit hit; //this part might not be neccesary but would be good to keep for funzies
    	if(Physics.Raycast(cam.position, cam.forward, out hit, 500f)) //basically the throw happens from the "hand" so this adjusts it so that it hits the center of the screen from any distance
    	{
      	forceDirection = (hit.point -attackPoint.position).normalized;
    	}
    
    	Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce; //might be able to just change it so that it takes upward force and thats it
    	projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
    	totalThrows--;
    	Invoke(nameof(ResetThrow), throwCooldown);
  	}

  	private void ResetThrow() //update so this is catch instead reset throw, shouldn't let the user throw a second time until hammer is caught again
  	{ //realistically there wouldn't be a second throw ever but hey, you never know
    	readyToThrow = true;
  	}
}
