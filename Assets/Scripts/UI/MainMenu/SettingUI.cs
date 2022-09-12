using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour {

    [SerializeField] private Text settingName;
    [SerializeField] private GameObject settingTypeParent;

    [SerializeField] private GameObject settingTypeValuePrefab;
    [SerializeField] private GameObject settingTypeBooleanPrefab;
    [SerializeField] private GameObject settingTypeEnumPrefab;
    [SerializeField] private GameObject settingTypeKeyCodePrefab;
    
    public void InitSettingUI(string settingName, object settingValue, SettingType settingType, System.Action<object> OnSettingChanged, int selectedIndex = 0) {
        this.settingName.text = settingName;                                                                                                // only applicable for list

        switch (settingType) { // really unsafe casts here, just relying on me putting in the correct value
            case SettingType.Value:
                Slider slider = Instantiate(settingTypeValuePrefab, settingTypeParent.transform).GetComponent<Slider>();

                slider.onValueChanged.AddListener((value) => why_is_unitys_sliders_so_trash(slider));
                slider.onValueChanged.AddListener((value) => OnSettingChanged(value));

                slider.value = (float)settingValue;
                slider.onValueChanged?.Invoke((float)settingValue);

                break;
            case SettingType.Boolean:
                Toggle toggle = Instantiate(settingTypeBooleanPrefab, settingTypeParent.transform).GetComponent<Toggle>();
                toggle.onValueChanged.AddListener((value) => OnSettingChanged(value));
                toggle.isOn = (bool)settingValue;

                break;
            case SettingType.Enum:
                Dropdown dropdown = Instantiate(settingTypeEnumPrefab, settingTypeParent.transform).GetComponent<Dropdown>();
                
                foreach (object obj in System.Enum.GetNames(settingValue.GetType()))
                    dropdown.options.Add(new Dropdown.OptionData(obj.ToString()));

                dropdown.onValueChanged.AddListener((selected) => OnSettingChanged(selected));
                dropdown.value = (int)settingValue;
                break;

            case SettingType.List: // similar to enums but passed in as a list
                Dropdown dropdownList = Instantiate(settingTypeEnumPrefab, settingTypeParent.transform).GetComponent<Dropdown>();
                
                foreach (object obj in (object[])settingValue)
                    dropdownList.options.Add(new Dropdown.OptionData(obj.ToString()));

                dropdownList.onValueChanged.AddListener((selected) => OnSettingChanged(selected));
                dropdownList.value = selectedIndex;
                break;

            case SettingType.KeyCode:
                KeyDetectorButton keyDetectorButton = Instantiate(settingTypeKeyCodePrefab, settingTypeParent.transform).GetComponent<KeyDetectorButton>();
                keyDetectorButton.text.text = settingValue.ToString();

                keyDetectorButton.onKeyChanged += (keyCode) => OnSettingChanged(keyCode);
                break;
        }

    }

    public void why_is_unitys_sliders_so_trash(Slider slider) {
        slider.transform.Find("Value").GetComponent<Text>().text = slider.value.ToString("0.0");
    }

}

public enum SettingType {
    Value, // clamped slider
    Boolean, // checkbox
    Enum, // drop down
    List, // drop down for a list
    KeyCode, // key press detection
}