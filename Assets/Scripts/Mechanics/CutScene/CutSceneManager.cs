using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour {

    #region Singleton

    public static CutSceneManager Instance;

    void Awake() {

        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

    }

    #endregion

    [field: SerializeField] public GameObject CutSceneParent { get; private set; }
    [field: SerializeField] public GameObject DialoguePrefab { get; private set; }
    [field: SerializeField] public VideoPlayer CutsceneVideoPlayer { get; private set; }

    [SerializeField] private CutScene currentCutScene;

    void Start() {
        currentCutScene.Start();
    }

    void Update() { // skipping cutscene 
        if (Input.GetKeyDown(KeyCode.Space))
            GetComponentInChildren<StartGameObject>(true).gameObject.SetActive(true);
    }

    public float StartTimeline() { // returns total duration
        GetComponent<PlayableDirector>().Play();

        return (float)GetComponent<PlayableDirector>().duration;
    }

}
