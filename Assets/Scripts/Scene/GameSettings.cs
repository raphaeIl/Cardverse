using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour {

    public GameSetting CurrentGameSetting { get { return gameSetting; } }
    [SerializeField] private GameSetting gameSetting;

    private int previousResolutionIndex;
    private FullScreenMode previousFullScreenMode;

    void Start() {
        UpdateAllSettings();
    }

    void Update() {
        if (gameSetting.AutoHideCursor) {
            if (Input.GetKey(gameSetting.Controls[ControlType.Show_Cursor]) || 
                (InventoryInteractableHandler.Instance != null && InventoryInteractableHandler.Instance.GetVisibleInteractablePanels().Length > 0) || 
                (PartyManager.Instance != null && PartyManager.Instance.IsTavernVisible) || SceneManager.GetActiveScene().name == "MenuScene") { // hard coded fix later with a state in GameMaster
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        UpdateAllSettings();  
    }

    public void UpdateAllSettings() {
        // Controls
        //if (gameSetting.AutoHideCursor) {
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.Locked;
        //}

        if (InventoryInteractableHandler.Instance != null) // slight problem where when GameSettings is used outside of the game scene so InventoryInteractableHandler wouldn't exist
            InventoryInteractableHandler.Instance.SetInteractablePanelToggleKeys(new KeyCode[] { gameSetting.Controls[ControlType.Character_Menu], gameSetting.Controls[ControlType.Equipment_Panel], gameSetting.Controls[ControlType.Inventory] });

        // Video
        int resolutionIndex = gameSetting.ResolutionIndex;
        if (previousResolutionIndex != resolutionIndex || previousFullScreenMode != gameSetting.FullScreenMode) {
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, gameSetting.FullScreenMode);
            previousResolutionIndex = resolutionIndex;
            previousFullScreenMode = gameSetting.FullScreenMode;
        }

        // Audio
        GameMaster.Instance.AudioManager.SetVolume(gameSetting.MasterVolume, AudioManager.AudioChannel.Master);
        GameMaster.Instance.AudioManager.SetVolume(gameSetting.MusicVolume, AudioManager.AudioChannel.Music);
        GameMaster.Instance.AudioManager.SetVolume(gameSetting.SFXVolume, AudioManager.AudioChannel.SFX);
    }

    public KeyCode GetControlKey(ControlType controlType) {
        return gameSetting.Controls[controlType];
    }
}
