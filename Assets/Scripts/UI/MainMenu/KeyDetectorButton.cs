using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KeyDetectorButton : MonoBehaviour {

    private Button button;
    public Text text;

    public System.Action<KeyCode> onKeyChanged;

    private bool isSelected;

    void Awake() {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
    }

    void Update() {

        if (isSelected)
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKey(keyCode)) {
                    text.text = keyCode.ToString();

                    if (onKeyChanged != null)
                        onKeyChanged(keyCode);

                    isSelected = false;
                }
            }
    }

    public void OnButtonSelected() {
        if (EventSystem.current.currentSelectedGameObject == button.gameObject)
            isSelected = true;
    }
}
