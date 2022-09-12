using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [SerializeField] private SettingsMenu settingsMenu;

    private Button[] menuButtons;

    void Awake() {
        menuButtons = GetComponentsInChildren<Button>();
    }

    void Start() {
        menuButtons[1].interactable = false;

        GameMaster.Instance.AudioManager.Play2DMusic(AudioManager.MusicType.First_Village);
    }

    public void OnNewGameButton() {
        GameMaster.Instance.AudioManager.StopAllMusic();

        GameMaster.Instance.SceneTransitioner.TransitionToScene("CutScene", 0f);
    }

    public void OnContinueButton() {
        Toast.DisplayError("That feature isn't implemented yet!");
    }

    public void OnSettingsButton() {
        Hide();

        settingsMenu.Show();
    }

    public void OnQuitButton() {
        Application.Quit();
    }

    public void Show() {
        this.gameObject.SetActive(true);
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }
}
