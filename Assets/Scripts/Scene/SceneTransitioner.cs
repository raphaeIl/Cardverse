using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour {

    private Image background;

    void Start() {
        background = GetComponentInChildren<Image>();

        StartCoroutine(FadeToColor(Color.black, Color.clear, 1f));
    }

    public void TransitionToScene(string sceneName, float fadeOutTime = 1f) {
        StartCoroutine(StartTransitionToScene());

        IEnumerator StartTransitionToScene() {
            yield return StartCoroutine(FadeToColor(Color.clear, Color.black, fadeOutTime));

            SceneManager.LoadScene(sceneName);
            Start();
        }
    }

    public void BattleEndTransition() {
        StandardTransition(Color.clear, Color.black, 0.5f, 0.7f, Color.black, Color.clear, 1.0f);
    }

    public void BattleEntranceTransition() {
        StandardTransition(Color.clear, Color.black, 0.5f, 0f, Color.black, Color.clear, 1.0f);
    }

    // a standard transition, for example: fading from clear to black for 0.5s and waiting for 1s before fading from black to clear again
    public void StandardTransition(Color fadeInFromColor, Color fadeInToColor, float fadeInTime, float stayDelay, Color fadeOutFromColor, Color fadeOutToColor, float fadeOutTime) {
        StartCoroutine(StartStandardTransition());

        IEnumerator StartStandardTransition() {
            yield return StartCoroutine(FadeToColor(fadeInFromColor, fadeInToColor, fadeInTime)); 
            yield return new WaitForSeconds(stayDelay);
            yield return StartCoroutine(FadeToColor(fadeOutFromColor, fadeOutToColor, fadeOutTime));
        }
    }

    // graph demo https://www.desmos.com/calculator/emwiw3olme
    private IEnumerator FadeBetweenTwoColors(Color color1, Color color2, float duration, int frequency) {

        float speed = 1 / duration;
        float percent = 0;

        while (percent < 1) {
            float evaluate = -0.5f * Mathf.Cos(frequency * percent) + 0.5f;

            percent += Time.deltaTime * speed;
            background.color = Color.Lerp(color1, color2, evaluate);

            yield return null;
        }

    }

    private IEnumerator FadeToColor(Color fromColor, Color toColor, float duration) {

        float speed = 1 / duration;
        float percent = 0;

        while (percent < 1) {

            percent += Time.deltaTime * speed;
            background.color = Color.Lerp(fromColor, toColor, percent);

            yield return null;
        }

    }
}
