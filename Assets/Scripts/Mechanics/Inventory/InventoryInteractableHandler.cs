using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventoryInteractableHandler : MonoBehaviour {

    #region Singleton

    public static InventoryInteractableHandler Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

    }

    #endregion

    private IInventoryInteractable[] allInteractables;

    private List<InteractablePanel> previouslyVisiblePanels;

    void OnValidate() { 
        allInteractables = GetComponentsInChildren<IInventoryInteractable>();
    }

    public void Update() {
        allInteractables = GetComponentsInChildren<IInventoryInteractable>();

        OnHover(new PointerEventData(EventSystem.current) { position = Input.mousePosition });

        IInventoryInteractable currentlyActiveInteractable = GetActiveInteractable();

        //if (currentlyActiveInteractable != null)
        //    print(currentlyActiveInteractable.AttachedGameObject.name);
    }

    public void OnHover(PointerEventData eventData) {
        foreach (IInventoryInteractable inventoryInteractable in allInteractables)
            inventoryInteractable.OnHover(eventData);
    }

    public void SetInteractablePanelToggleKeys(KeyCode[] keycodes) { // in the scene hierarchy order
        for (int i = 0, j = 0; i < allInteractables.Length; i++)
            if (allInteractables[i] is InteractablePanel interactablePanel)
                interactablePanel.ToggleKey = keycodes[j++];
    }

    public T MouseOverInteractableIcon<T>() where T : InteractableIcon {
        foreach (IInventoryInteractable interactable in allInteractables)
            if (RectTransformUtility.RectangleContainsScreenPoint(interactable.AttachedGameObject.GetComponent<RectTransform>(), Input.mousePosition))
                if (interactable is InteractableIcon)
                    return (T)interactable;
        
        return default(T);
    }

    public IInventoryInteractable GetActiveInteractable() {
        return allInteractables.Where(i => i.IsActive).FirstOrDefault();
    }

    public IInventoryInteractable[] GetVisibleInteractablePanels() {
        List<IInventoryInteractable> visibleInteractables = new List<IInventoryInteractable>();

        foreach (IInventoryInteractable inventoryInteractable in allInteractables)
            if (inventoryInteractable is InteractablePanel interactablePanel && interactablePanel.IsVisible) {
                visibleInteractables.Add(interactablePanel);
            }

        return visibleInteractables.ToArray();
    }

    public void HideAllVisiblePanels() {
        previouslyVisiblePanels = new List<InteractablePanel>();

        foreach (IInventoryInteractable inventoryInteractable in allInteractables)
            if (inventoryInteractable is InteractablePanel interactablePanel && interactablePanel.IsVisible) {
                interactablePanel.Hide();
                previouslyVisiblePanels.Add(interactablePanel);
            }
    }

    public void ShowAllVisiblePanels() {
        foreach (InteractablePanel previousPanel in previouslyVisiblePanels)
            previousPanel.Show();

        previouslyVisiblePanels = null;
    }
}
