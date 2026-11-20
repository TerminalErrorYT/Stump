using UnityEngine;

public class CatchZoneTrigger : MonoBehaviour
{
    public EquipScript equipScript;
    public bool isPerfectZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer"))
        {
            if (isPerfectZone)
                equipScript.SetPerfectZone(true);
            else
                equipScript.SetCatchZone(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hammer"))
        {
            if (isPerfectZone)
                equipScript.SetPerfectZone(false);
            else
                equipScript.SetCatchZone(false);
        }
    }
}