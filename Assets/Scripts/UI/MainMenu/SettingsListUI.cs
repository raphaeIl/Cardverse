using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsListUI : MonoBehaviour {

    [SerializeField] private SettingUI settingUIPrefab;

    public SettingUI CreateSettingUI(string name, object value, SettingType settingType, System.Action<object> OnSettingChanged, int selectedIndex = 0) {
        SettingUI settingUI = Instantiate(settingUIPrefab, transform).GetComponent<SettingUI>();                                    // only applicable for SettingType.List

        settingUI.InitSettingUI(name, value, settingType, OnSettingChanged, selectedIndex);

        return settingUI;
    }

}
