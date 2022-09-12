using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {

    public static CameraShaker Instance;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);
    }

    public void Shake(float duration, float magnitude) {
        StartCoroutine(ShakeCamera(duration, magnitude));
    }

    IEnumerator ShakeCamera(float duration, float magnitude) {

        Vector3 originalPos = transform.localPosition;
        float elapsed = 0;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;

            float x = Random.Range(-1f, 1f) * magnitude + originalPos.x;
            float y = Random.Range(-1f, 1f) * magnitude + originalPos.y;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            yield return null;
        }

        transform.localPosition = originalPos;
    }
   
}

