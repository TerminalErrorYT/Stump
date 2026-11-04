using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    [Header("Sub Menu Transition Layouts")]
    [SerializeField] private RectTransform subMenuPanel;
    [SerializeField] private Transform menuButtonsContainer;
    [SerializeField] private float slideDuration = 0.4f;

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

    public void ShowPlaySubMenu(Button clickedButton)
    {
        if (clickedButton == null || isMenuTransitionLocked) return;

        // 1. Find the sliding SubMenu panel nested inside this button's hierarchy
        RectTransform foundSubMenu = FindSubMenuChild(clickedButton.gameObject);
        if (foundSubMenu == null)
        {
            Debug.LogError($"Could not find a 'SubMenu' child object inside {clickedButton.name}!");
            return;
        }

        isMenuTransitionLocked = true; // Freeze main hover systems
        ResetPreviousButtonScale();     // Clean up any mid-lerp hovering scales

        // 2. Turn off the spinning hammer on the clicked button 
        GameObject clickedHammer = FindHammerChild(clickedButton.gameObject);
        if (clickedHammer != null) clickedHammer.SetActive(false);

        // 3. Clear out all other sibling main menu choices
        // menuButtonsContainer can just be clickedButton.transform.parent
        Transform buttonParentContainer = clickedButton.transform.parent;
        foreach (Transform child in buttonParentContainer)
        {
            if (child.gameObject != clickedButton.gameObject)
            {
                child.gameObject.SetActive(false);
            }
        }

        // 4. Force the selected option to the top layer slot if using a Layout Group
        clickedButton.transform.SetAsFirstSibling();

        // 5. Slide the nested sub-menu UI smoothly onto the canvas
        StartCoroutine(SlideMenuRoutine(foundSubMenu, Vector2.zero, slideDuration));
    }

    private RectTransform FindSubMenuChild(GameObject buttonObj)
    {
        if (buttonObj == null) return null;

        // Searches for a child named "SubMenu" (Adjust this string to match your exact hierarchy name!)
        Transform subMenuTransform = buttonObj.transform.Find("SubMenu");

        if (subMenuTransform != null)
        {
            return subMenuTransform.GetComponent<RectTransform>();
        }

        return null;
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
