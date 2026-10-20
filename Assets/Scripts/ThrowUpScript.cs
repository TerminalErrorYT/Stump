using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrowUpScript : MonoBehavior
{
  [Header("References")]
  public Transform cam;
  public Transform attackpoint;
  public GameObject objectToThrow;

  [Header("Settings")]
  public int totalThrows;
  public float throwCooldown;

  [Header("Throwing")]
  public KeyCode throwKey = KeyCode.Mouse0;
  public float throwForce;
  public float throwUpwardForce;

  bool readyToThrow;

  private void Start()
  {
    readyToThrow = true;
  }

  private void Update()
  {
    if(Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
    {
      Throw();
    }
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
      forceDirection = (hit.point -attackPoint.position).normalize
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
