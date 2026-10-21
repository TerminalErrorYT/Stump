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

  private void Throw() //update so it throws up not forawrd, hammer shouldn't really move around, should just go up and down for throw
  {
    readyToThrow = false;
    hammer.isEquipped = false;
    float throwUpwardForce = 25f;

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
        
    Vector3 tossDirection = (Camera.transform.forward) + (Vector3.up * throwUpwardForce);
    hammerRb.AddForce(tossDirection, ForceMode.Impulse);
    hammerRb.AddTorque(Camera.transform.right * 1f, ForceMode.Impulse);
  }

  private void Catch()
  {
    // generate a sphere around the player for the range of catching the hammer
    // closer to the center of the sphere the better the catch
    // calcualte power of next hit and have it quickly lose power if user doesnt swing the hammer in time
    // shouldn't be able to throw again until the user swings the hammer or the hammer hits the ground
    readyToThrow = true;
  }
  //create another class for when the hammer hits the ground and needs reset/ passed to the next player
}
