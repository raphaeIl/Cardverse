using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    #region Singleton

    public static GameMaster Instance;

    void Awake() {

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else 
            Destroy(this);

    }

    #endregion
    // scene state here, update in SceneTransitioner
    public AudioManager AudioManager { get; private set; }
    public SceneTransitioner SceneTransitioner { get; private set; }
    public GameSettings GameSettings { get; private set; }

    public GameAssets GameAssets { get; private set; }
    public Utils Utils { get; private set; }
    

    // Start is called before the first frame update
    void Start() {
        AudioManager = GetComponentInChildren<AudioManager>();
        SceneTransitioner = GetComponentInChildren<SceneTransitioner>();
        GameSettings = GetComponent<GameSettings>();
        GameAssets = GetComponent<GameAssets>();
        Utils = GetComponent<Utils>();
    }

    // Update is called once per frame
    void Update() {
    }
}

public enum GameState {
    MainMenu,
    PauseMenu,
    Game,
    CutScene
}
