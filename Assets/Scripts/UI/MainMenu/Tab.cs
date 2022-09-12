using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour {

    public Text tabTitle;

    private SettingsListUI settingsListUI;

    void Awake() {
        settingsListUI = GetComponentInChildren<SettingsListUI>();
    }

    public void InitTab(SettingTabType settingType) {
        GameSetting gameSetting = GameMaster.Instance.GameSettings.CurrentGameSetting;

        tabTitle.text = settingType.ToString();

        switch (settingType) { // rip had to hard code all this idk how to do it cleaner
            case SettingTabType.Controls:
                settingsListUI.CreateSettingUI("Auto Hide Cursor", gameSetting.AutoHideCursor, SettingType.Boolean, (value) => gameSetting.AutoHideCursor = (bool)value);

            foreach (var control in gameSetting.Controls.serializedictionary)
                settingsListUI.CreateSettingUI(control.Key.ToString().Replace('_', ' '), control.Value, SettingType.KeyCode, (keyCode) => control.Value = (KeyCode)keyCode);

                break;

            case SettingTabType.Video:
                settingsListUI.CreateSettingUI("Full Screen Mode", gameSetting.FullScreenMode, SettingType.Enum, (mode) => gameSetting.FullScreenMode = (FullScreenMode)mode);

                string[] resolutionDisplays = Screen.resolutions.Select(resolution => $"{resolution.width} x {resolution.height}").ToArray();
                settingsListUI.CreateSettingUI("Resolution", resolutionDisplays, SettingType.List, (index) => { gameSetting.ResolutionIndex = (int)index; gameSetting.OnValidate(); }, gameSetting.ResolutionIndex);

                break;

            case SettingTabType.Audio:
                settingsListUI.CreateSettingUI("Master Volume", gameSetting.MasterVolume, SettingType.Value, (value) => gameSetting.MasterVolume = (float)value);
                settingsListUI.CreateSettingUI("Music Volume", gameSetting.MusicVolume, SettingType.Value, (value) => gameSetting.MusicVolume = (float)value);
                settingsListUI.CreateSettingUI("SFX Volume", gameSetting.SFXVolume, SettingType.Value, (value) => gameSetting.SFXVolume = (float)value);
                break;

            case SettingTabType.Graphics:
                break;
        }

    }

    public void Enable() {
        gameObject.SetActive(true);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }

}
