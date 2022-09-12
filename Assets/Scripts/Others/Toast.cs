using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour {
    
    public static readonly Vector2 LOCATION_TOP = new Vector2(0.5f, 0.9f); // offset included
    public static readonly Vector2 LOCATION_MIDDLE = new Vector2(0.5f, 0.5f);
    public static readonly Vector2 LOCATION_BOTTOM = new Vector2(0.5f, 0.1f); // offset included
    
    private TextMeshProUGUI textMeshPro;
    private CanvasGroup canvasGroup;

    public void InitToast(string text, Color? color = null, float fadeIn = 0.5f, float stay = 1.0f, float fadeOut = 0.5f) {
        color ??= Color.white;
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();

        textMeshPro.text = text;
        textMeshPro.color = color.Value;

        StartCoroutine(ToastAnimation());

        IEnumerator ToastAnimation() {
            canvasGroup.alpha = 0;
            transform.localScale = new Vector2(0.9f, 0.9f);

            float speed = 1 / fadeIn;
            float percent = 0;

            while (percent < 1) { // fade in
                float scale = Mathf.Lerp(0.9f, 1f, percent);
                float alpha = Mathf.Lerp(0f, 1f, percent);

                transform.localScale = new Vector2(scale, scale);
                canvasGroup.alpha = alpha;

                percent += Time.deltaTime * speed;
                yield return null;
            }

            yield return new WaitForSeconds(stay); // stay

            speed = 1 / fadeOut;
            percent = 0;

            while (percent < 1) { // fade out
                float scale = Mathf.Lerp(1f, 0.9f, percent);
                float alpha = Mathf.Lerp(1f, 0f, percent);

                transform.localScale = new Vector2(scale, scale);
                canvasGroup.alpha = alpha;

                percent += Time.deltaTime * speed;
                yield return null;
            }

        }
    }

    private void SetLocation(ToastLocation toastLocation, Vector2? customLocation = null) {
        RectTransform rt = GetComponent<RectTransform>();
        
        switch (toastLocation) {
            case ToastLocation.Top:
                rt.anchorMin = LOCATION_TOP;
                rt.anchorMax = LOCATION_TOP;
                rt.pivot = LOCATION_TOP;
                break;

            case ToastLocation.Middle:
                rt.anchorMin = LOCATION_MIDDLE;
                rt.anchorMax = LOCATION_MIDDLE;
                rt.pivot = LOCATION_MIDDLE;            
                break;

            case ToastLocation.Bottom:
                rt.anchorMin = LOCATION_BOTTOM;
                rt.anchorMax = LOCATION_BOTTOM;
                rt.pivot = LOCATION_BOTTOM;
                break;

            case ToastLocation.Custom:
                rt.anchorMin = customLocation.Value;
                rt.anchorMax = customLocation.Value;
                rt.pivot = customLocation.Value;
                break;
        }
    }

    public static void DisplayMessage(string message, ToastLocation toastLocation = ToastLocation.Bottom) {
        Vector2 displayLocation = toastLocation == ToastLocation.Top ? Toast.LOCATION_TOP : toastLocation == ToastLocation.Middle ? Toast.LOCATION_MIDDLE : LOCATION_BOTTOM;

        Toast toast = Instantiate(GameMaster.Instance.GameAssets.pfToast, displayLocation, Quaternion.identity).GetComponentInChildren<Toast>();
        toast.InitToast(message);
        toast.SetLocation(toastLocation);

        Debug.Log(message);
    }

    public static void DisplayError(string message, ToastLocation toastLocation = ToastLocation.Bottom, Exception exception = null) {
        DisplayMessage(message, toastLocation);

        Debug.LogError(message);

        if (exception != null)
            throw new Exception(message, exception);
    }
}

public enum ToastLocation {
    Top,
    Middle,
    Bottom,
    Custom
}
