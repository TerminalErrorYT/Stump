using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ThrowUpScript : MonoBehaviour
{ 
  	[Header("References")]
  	public Camera Camera;
  	public Transform attackPoint;
  	public GameObject Hammer;

  	bool readyToThrow; //looking to see when the player is holding, if holding before throw then true, if holding after catch, then false
  	private ItemFloat itemFloatScript;
  	public EquipScript hammer;

	public strengthModifier = 0; //strength modifier will add a fraction of the original force ontop of the original throw to allow it to be thrown even higher

  	private void Start()
  	{
    	readyToThrow = true;
	}

	private void Update()
	{
		hammer = GetComponent<EquipScript>();
    	if (hammer != null && hammer.isEquipped && readyToThrow)
    	{
			if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && readyToThrow)
			{
	  			Throw();
			}
    	}else if (hammer != null && hammer.isEquipped == false){
			if(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
			{
	  			Catch();
			}
    	}
  	}

  	private void Throw()
  	{
    	readyToThrow = false;
    	hammer.isEquipped = false;
    	float throwUpwardForce = 25f; //change this to change the height the hammer will be thrown

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
        
    	Vector3 tossDirection = (Camera.transform.forward) + (Vector3.up * throwUpwardForce); //hammer throw direction
    	hammerRb.AddForce(tossDirection, (ForceMode.Impulse + (ForceMode.Impulse * strengthModifier))); //hammer throw power
    	hammerRb.AddTorque(Camera.transform.right * 1f, ForceMode.Impulse); //hammer spin
  	}

  	private void Catch()
  	{
    	// generate one sphere and one box/cylinder around the player for the range of catching the hammer
		// the sphere is the complete range the player can catch the hammer
		// the box/cylinder is the "perfect" range that conserves all momentum from the hammer falling in the swing
    	// closer to the player catches to the smaller sphere the better the catch
		// both areas should follow the players hand area but not rotate with it
		// might also think about putting a failed catch area behind the player since catching something behind you is impossible
    	// calcualte power of next hit and have it quickly lose power if user doesnt swing the hammer in time
    	// shouldn't be able to throw again until the user swings the hammer or the hammer hits the ground
    	readyToThrow = true;
  	}
  	private void MissedCatch(){
		//create another class for when the hammer hits the ground and needs reset/ passed to the next player
		
		readyToThrow = true;
	}
	
	private void ProximityDetector(){
		//create another class that will warn the player that they are getting close/leaving the area where the hammer is going to fall, maybe puts an indicator on the ground on where its going to fall

	}
	//create another class or file that calculates the velocity of the hammer as it falls
}
