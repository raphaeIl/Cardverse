using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlobalLightController : MonoBehaviour {

    [Range(0, 1)] public float time;

    public Gradient timeGradients;

    private Light2D globalLight;

    // Start is called before the first frame update
    void Start() {
        globalLight = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update() {
        globalLight.color = timeGradients.Evaluate(time);
    }
}
