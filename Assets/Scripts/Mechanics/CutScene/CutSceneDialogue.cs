using System.Collections;
using UnityEngine;
using TMPro;
using MyBox;

public class CutSceneDialogue : MonoBehaviour {

    public readonly Vector2 LOCATION_TOP = new Vector2(0.5f, 1f);
    public readonly Vector2 LOCATION_MIDDLE = new Vector2(0.5f, 0.5f);
    public readonly Vector2 LOCATION_BOTTOM = new Vector2(0.5f, 0f);

    public TextMeshProUGUI dialogueText;

    public float PlayDialogue(DialogueData dialogueData) {
        return PlayDialogue(dialogueData.Dialogue, dialogueData.FadeInTime, dialogueData.StayTime, dialogueData.FadeOutTime, dialogueData.UseOutline, dialogueData.DialogueLocation, dialogueData.CustomLocation);
    }

    public float PlayDialogue(string text, float fadeIn, float stay, float fadeOut, bool useOutline, DialogueLocation dialogueLocation, Vector2? customLocation = null) {
        SetLocation(dialogueLocation, customLocation);
        dialogueText.text = text;
        dialogueText.outlineWidth = useOutline ? 0.2f : 0f;

        StartCoroutine(PlayDialogueTransitions());

        IEnumerator PlayDialogueTransitions() {

            float speed = 1 / fadeIn;
            float percent = 0;

            while (percent < 1) {
                percent += Time.deltaTime * speed;
                dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, Mathf.Lerp(0, 1, percent));

                yield return null;
            }

            yield return new WaitForSeconds(stay);

            speed = 1 / fadeOut;
            percent = 0;

            while (percent < 1) {
                percent += Time.deltaTime * speed;
                dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, Mathf.Lerp(1, 0, percent));

                yield return null;
            }
        }

        return fadeIn + stay + fadeOut;
    }

    private void SetLocation(DialogueLocation dialogueLocation, Vector2? customLocation = null) {
        RectTransform rt = GetComponent<RectTransform>();
        
        switch (dialogueLocation) {
            case DialogueLocation.Top:
                rt.anchorMin = LOCATION_TOP;
                rt.anchorMax = LOCATION_TOP;
                rt.pivot = LOCATION_TOP;
                break;

            case DialogueLocation.Middle:
                rt.anchorMin = LOCATION_MIDDLE;
                rt.anchorMax = LOCATION_MIDDLE;
                rt.pivot = LOCATION_MIDDLE;            
                break;

            case DialogueLocation.Bottom:
                rt.anchorMin = LOCATION_BOTTOM;
                rt.anchorMax = LOCATION_BOTTOM;
                rt.pivot = LOCATION_BOTTOM;
                break;

            case DialogueLocation.Custom:
                rt.anchorMin = customLocation.Value;
                rt.anchorMax = customLocation.Value;
                rt.pivot = customLocation.Value;
                break;
        }
    }
}

[System.Serializable]
public struct DialogueData {

    public string Dialogue;

    public float StartDelay;
    public float FadeInTime;
    public float StayTime;
    public float FadeOutTime;
    public float EndDelay;

    public bool UseOutline;
    
    public bool simultaneous;

    public DialogueLocation DialogueLocation;
	public bool _isCustomLocation;
    [ConditionalField("_isCustomLocation")] public Vector2 CustomLocation;
}

public enum DialogueLocation {
    Top,
    Middle,
    Bottom,
    Custom
}
