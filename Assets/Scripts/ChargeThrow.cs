using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChargeThrow : MonoBehaviour
{
    [Header("UI References")]
    public Slider qteSlider;
    public RectTransform targetZone;
    public RectTransform handle;

    [Header("Settings")]
    public float speed = 1.5f;
    public KeyCode actionKey = KeyCode.Space;
    public bool pingPong = true; // True = bounces back and forth; False = one-way

    [Header("Events")]
    public UnityEvent onSuccess;
    public UnityEvent onFail;

    private bool isActive = false;
    private float timer = 0f;

    public void StartQTE()
    {
        timer = 0f;
        qteSlider.value = 0f;
        isActive = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isActive) return;

        // 1. Move the slider value
        timer += Time.deltaTime * speed;

        if (pingPong)
        {
            // Moves smoothly between 0 and 1 back and forth
            qteSlider.value = Mathf.PingPong(timer, 1f);
        }
        else
        {
            // Moves from 0 to 1 once; fails if it reaches the end
            qteSlider.value = timer;
            if (qteSlider.value >= 1f)
            {
                Fail();
                return;
            }
        }

        // 2. Check for player input
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            CheckPower();
        }
    }

    void CheckPower()
    {
        isActive = false;

        // Convert UI positions to check if the handle is inside the target zone
        float handleX = qteSlider.value;
        Debug.Log(handleX);
    }

    void Fail()
    {
        isActive = false;
        Debug.Log("QTE Failed!");
        onFail?.Invoke();
    }
}
