using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private float hoverScaleMultiplier = 1.15f;
    [SerializeField] private float animationSpeed = 8f;

    // [Header("Sub Menu Transition Layouts")]
    // [SerializeField] private RectTransform subMenuPanel;
    // [SerializeField] private Transform menuButtonsContainer;
    // [SerializeField] private float slideDuration = 0.4f;
    public float slideDuration = 0.5f;
    public Vector2 startPosition = new Vector2(2000, 0);
    public Vector2 targetPosition = Vector2.zero;

    private GameObject currentHoveredButton;
    private Vector3 cachedOriginalScale = Vector3.one;
    private GameObject currentHammerObj; // Safely tracks the current hammer
    GameObject overObject;
    GameObject detectedButton;
    private bool isMenuTransitionLocked = false;

    void Update()
    {
        if (isMenuTransitionLocked) return;

        overObject = GetHoveredUIObject();
        detectedButton = FindButtonInHierarchy(overObject);

        if (detectedButton != currentHoveredButton)
        {
            ResetPreviousButtonScale();

            currentHoveredButton = detectedButton;

            if (currentHoveredButton != null)
            {
                cachedOriginalScale = currentHoveredButton.transform.localScale;
                currentHammerObj = FindHammerChild(currentHoveredButton);
            }
            else
            {
                currentHammerObj = null;
            }
        }

        if (currentHammerObj != null)
        {
            currentHammerObj.transform.Rotate(Vector3.forward, animationSpeed * 20f * Time.deltaTime);
        }

        HandleButtonAnimations();
    }

    private void HandleButtonAnimations()
    {
        if (currentHoveredButton != null)
        {
            Vector3 targetScale = cachedOriginalScale * hoverScaleMultiplier;
            currentHoveredButton.transform.localScale = Vector3.Lerp(
                currentHoveredButton.transform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );
        }
    }

    private void ResetPreviousButtonScale()
    {
        if (currentHoveredButton != null)
        {
            currentHoveredButton.transform.localScale = cachedOriginalScale;
        }
    }

    private GameObject GetHoveredUIObject()
    {
        if (EventSystem.current == null || Mouse.current == null) return null;

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0 ? results[0].gameObject : null;
    }

    private GameObject FindButtonInHierarchy(GameObject obj)
    {
        if (obj == null) return null;

        Button btn = obj.GetComponentInParent<Button>();
        return btn != null ? btn.gameObject : null;
    }

    private GameObject FindHammerChild(GameObject buttonObj)
    {
        if (buttonObj == null) return null;

        Transform hammerTransform = buttonObj.transform.Find("Hammer");

        return hammerTransform != null ? hammerTransform.gameObject : null;
    }
    //remove other menu items other than the one clicked
    //move the one clicked to the top if not already there
    //have the sub options show up (on nails rather than hammers)
    public void ShowPlaySubMenu()
    {
        // For security, look for a GameObject with the script or passed reference 
        // Assuming 'detectedButton' is already defined in your class
        Canvas subMenu = detectedButton.GetComponentInParent<Canvas>();

        if (subMenu != null)
        {
            RectTransform rectTransform = subMenu.GetComponent<RectTransform>();

            // Ensure the canvas is active before moving it
            subMenu.gameObject.SetActive(true);

            // Start the smooth slide movement
            StartCoroutine(SlideIn(rectTransform));
        }
    }

    private IEnumerator SlideIn(RectTransform rectToMove)
    {
        float elapsedTime = 0;

        // Set initial starting position off-screen
        rectToMove.anchoredPosition = startPosition;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / slideDuration;

            // SmoothStep creates an "ease-in-ease-out" effect instead of a rigid linear movement
            t = Mathf.SmoothStep(0, 1, t);

            rectToMove.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Snap to exact target position at the end
        rectToMove.anchoredPosition = targetPosition;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
