using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class MultiplayerButtonPress : MonoBehaviour
{
    Vector3 moveToPosition; // This is where the camera will move after the start
    Quaternion rotateCamera;
    float speed = 2f; // this is the speed at which the camera moves
    bool started = false; // stops the movement until we want it

    void Start()
    {
        // Since this object as an child the (0, 0, 0) position will be the same as the players. So we can just add to the zero vector and it will be position correctly. 

        moveToPosition = new Vector3(-559, 494, 364); // move camera to above log
                                                      // position: -558.9, 493.8, 364
                                                      // rotation: 76.9, -1.01, 6.5
                                                      // spin log with nail
                                                      // remove nail on spin
                                                      // possibly swap scenes with a crossfade or something
        rotateCamera = Quaternion.Euler(76.9f, -1.01f, 6.5f);
    }

    void Update()
    {
        // so we only want the movement to start when we decide
        if (!started)
            return;

        transform.position = Vector3.Lerp(transform.position, moveToPosition, speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateCamera ,speed);
    }

    public void StartCameraMove()
    {
        started = true;
    }
}
