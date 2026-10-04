using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldInputHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float interactDistanceLimit = 100f;
    private readonly List<RaycastResult> uiRaycastResults = new();
    private PointerEventData pointerEventData;
    // Update is called once per frame
    void Update()
    {
#if !UNITY_EDITOR && ( UNITY_ANDROID || UNITY_IOS)
        RaycastCheckWithTouch();
#else
        RaycastCheckWithMouse();
#endif
    }
    void RaycastCheckWithTouch()
    {
        if (Input.touchCount > 0)
        {
            if (Input.touchCount != 1)
                return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Began)
                return;

            if (IsPointerOverUI(touch.position))
                return;

            Ray ray = mainCamera.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistanceLimit, interactableMask))
            {
                HandleHit(hit);
            }
        }
    }

    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return false;

        pointerEventData ??= new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;
        uiRaycastResults.Clear();
        eventSystem.RaycastAll(pointerEventData, uiRaycastResults);

        for (int i = 0; i < uiRaycastResults.Count; i++)
        {
            if (uiRaycastResults[i].module is GraphicRaycaster)
                return true;
        }

        return false;
    }

    void RaycastCheckWithMouse()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (IsPointerOverUI(Input.mousePosition))
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistanceLimit, interactableMask))
        {
            HandleHit(hit);
        }
    }
    void HandleHit(RaycastHit hit)
    {
        if (GameUIOverlay.IsGameUIOverlay) return;
        var interactableObject = hit.collider.GetComponentInParent<IInteractable>();
        if (interactableObject != null)
            interactableObject.OnTouch();
    }
}
