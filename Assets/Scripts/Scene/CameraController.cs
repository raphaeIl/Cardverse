using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform focus;
    public float smoothTime = 2;

    public Vector3 offset;

    private bool useOffset;

    void Awake() {
        offset = focus.position - transform.position;
    }

    void Update() {
        if (focus == null)
            return;

        transform.position = Vector3.Lerp(transform.position, focus.position + (useOffset ? -offset : Vector3.back), Time.deltaTime * smoothTime);
    }

    public void SetFocus(Transform transform, bool useOffset = false, float zoom = 2.7f) {
        focus = transform;

        this.useOffset = useOffset;
        StartCoroutine(Zoom(zoom));
    }

    public void RemoveFocus() {
        focus = null;

        this.useOffset = false;
        StartCoroutine(Zoom(3.14941f));
    }

    IEnumerator Zoom(float resultingZoom) {

        float startingZoom = GetComponent<Camera>().orthographicSize;
        float percent = 0;

        while (percent < 1) {

            GetComponent<Camera>().orthographicSize = Mathf.Lerp(startingZoom, resultingZoom, percent);
            percent += Time.deltaTime;

            yield return null;
        }

    }
}
