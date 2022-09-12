using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentSize : MonoBehaviour { // unity's content size fitter is actually garbage

    void Update() {

        float totalChildHeight = 0;
        float maxChildWidth = 0;

        foreach (Transform transform in this.transform) {
            RectTransform childRectTransform = transform.GetComponent<RectTransform>();

            totalChildHeight += childRectTransform.rect.height;

            if (childRectTransform.rect.width > maxChildWidth)
                maxChildWidth = childRectTransform.rect.width;
        }

        RectTransform rt = GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(maxChildWidth, totalChildHeight);

    }
}
