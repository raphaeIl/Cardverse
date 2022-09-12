using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "Game/Cutscene")]
public class CutScene : ScriptableObject {

    [SerializeField] private float StartDelay;
    [SerializeField] private AudioManager.MusicType BackgroundMusic;
    
    [SerializeField] private DialogueData[] Dialogues;

    public void Start() {
        CutSceneManager.Instance.StartCoroutine(StartCutScene()); // borrowing CutSceneManager since this class isn't MonoBehaviour

        IEnumerator StartCutScene() {
            GameMaster.Instance.AudioManager.Play2DMusic(BackgroundMusic);

            yield return new WaitForSeconds(StartDelay);

            CutSceneManager.Instance.StartCoroutine(PlayDialogues());
            yield return new WaitForSeconds(26.0f); // get total dialogue time

            yield return CutSceneManager.Instance.StartCoroutine(PlayVideo());

            CutSceneManager.Instance.StartTimeline();
        }

    }

    public IEnumerator PlayDialogues() {
        for (int i = 0; i < Dialogues.Length; i++) {
            DialogueData currentDialogueData = Dialogues[i];

            yield return new WaitForSeconds(currentDialogueData.StartDelay);
            
            CutSceneDialogue dialogue = Instantiate(CutSceneManager.Instance.DialoguePrefab, CutSceneManager.Instance.CutSceneParent.transform).GetComponent<CutSceneDialogue>();
            float duration = dialogue.PlayDialogue(currentDialogueData);
            yield return new WaitForSeconds(duration);

            yield return new WaitForSeconds(currentDialogueData.EndDelay);

            Destroy(dialogue.gameObject);
        }
    }

    private IEnumerator PlayVideo() {
        VideoPlayer cutsceneVideoPlayer = CutSceneManager.Instance.CutsceneVideoPlayer;

        cutsceneVideoPlayer.targetTexture.Release();

        float percent = 0;

        cutsceneVideoPlayer.Play();

        while (percent < 1) {
            cutsceneVideoPlayer.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, percent);
            percent += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds((float)cutsceneVideoPlayer.length);

        percent = 0;

        while (percent < 1) {
            cutsceneVideoPlayer.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, percent);
            percent += Time.deltaTime;

            yield return null;
        }

        cutsceneVideoPlayer.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
