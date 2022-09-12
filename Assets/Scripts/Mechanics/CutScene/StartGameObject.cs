using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameObject : MonoBehaviour {

    // Start is called before the first frame update
    void Awake() {
        GameMaster.Instance.SceneTransitioner.TransitionToScene("GameScene");
    }


}
