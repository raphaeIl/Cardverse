using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractablePanel : InventoryInteractable {

    public bool IsVisible { get; private set; }

    [HideInInspector] // can be shown in inspector if needed
    public KeyCode ToggleKey { get; set; }

    private Animator animator;

    protected override void Awake() {
        base.Awake();
        animator = GetComponentInChildren<Animator>();

        IsActive = false;
        
        InteractableActionEvents[(int)InteractableEventType.OnPointerBeginDrag].Event += BeginDrag;
        InteractableActionEvents[(int)InteractableEventType.OnPointerDrag].Event      += Drag;
        InteractableActionEvents[(int)InteractableEventType.OnPointerEndDrag].Event   += EndDrag;
    }

    void Update() {
        if (Input.GetKeyDown(ToggleKey))
            OnToggleVisibility();

        animator.SetBool("isVisible", IsVisible);
    }

    public void BeginDrag(PointerEventData eventData) { // panel follow mouse according to pivot, bad drawing -- https://imgur.com/a/pnFuqLz
        IsActive = true;

        RectTransform rt = GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float normalizedMouseX = (eventData.position.x - corners[1].x) / (corners[2].x - corners[1].x);
        float normalizedMouseY = (eventData.position.y - corners[1].y) / (corners[0].y - corners[1].y);

        rt.pivot = new Vector2(normalizedMouseX, normalizedMouseY);
    }

    public void Drag(PointerEventData eventData) {
        GetComponent<RectTransform>().position = eventData.position;
    }

    public void EndDrag(PointerEventData eventData) {
        IsActive = false;
    }

    public void OnToggleVisibility() {
        IsVisible = !IsVisible;
    }

    public void Hide() {
        IsVisible = false;
    }

    public void Show() {
        IsVisible = true;
    }

    public static InteractablePanel GetCurrentPanel_Static(GameObject source) {
        return source.GetComponentInParent<InteractablePanel>();
    }
}
