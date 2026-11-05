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

    public float slideDuration = .75f;
    public Vector2 startPosition = new Vector2(1000, -135);
    public Vector2 targetPosition = new Vector2(400, -135);

    private GameObject currentHoveredButton;
    private GameObject buttonClicked;
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
            if (currentHoveredButton != null && currentHoveredButton != buttonClicked)
            {
                currentHoveredButton.transform.localScale = cachedOriginalScale;
            }

            currentHoveredButton = detectedButton;

            if (currentHoveredButton != null)
            {
                if (currentHoveredButton != buttonClicked)
                {
                    cachedOriginalScale = currentHoveredButton.transform.localScale;
                }
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

        if (buttonClicked != null)
    {
        Vector3 targetScale = cachedOriginalScale * hoverScaleMultiplier;
        buttonClicked.transform.localScale = Vector3.Lerp(
            buttonClicked.transform.localScale,
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

    public void ShowPlaySubMenu(RectTransform subMenuRect)
    {
        //remove other menu items other than the one clicked
        //move the one clicked to the top if not already there
        if (currentHoveredButton != null)
        {
            buttonClicked = currentHoveredButton;
        }
        else if (detectedButton != null)
        {
            buttonClicked = detectedButton;
        }

        if (subMenuRect != null)
        {
            subMenuRect.gameObject.SetActive(true);
            subMenuRect.anchoredPosition = startPosition;
            subMenuRect.DOAnchorPos(targetPosition, slideDuration)
                .SetEase(Ease.OutQuad);
        }
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
