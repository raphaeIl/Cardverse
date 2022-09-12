using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {

    [SerializeField] private TextMeshPro entityName;
    [SerializeField] private TextMeshPro dialogue;

    private LivingInteractableEntity currentEntity;
    private ConversationHandler currentConversation;

    private bool IsDialogueOver;

    public void UpdateDialogueUI(LivingInteractableEntity entity, ConversationHandler currentConversation) {
        this.currentEntity = entity;
        this.currentConversation = currentConversation;

        entityName.text = entity.GetEntityInfo<LivingEntityInfo>().Name;
        StartCoroutine(AnimateDialogueTextWriter(currentConversation.GetNextDialogue(), 0.05f));

        IEnumerator AnimateDialogueTextWriter(string textToWrite, float delay) {
            IsDialogueOver = false;

            // predetermining the font size
            dialogue.text = textToWrite;
            float fontSize = dialogue.fontSize;
            dialogue.enableAutoSizing = false;
            dialogue.fontSize = fontSize;
            dialogue.text = "";
            
            for (int i = 0; i < textToWrite.Length && !IsDialogueOver; i++) {

                dialogue.text += textToWrite[i];

                yield return new WaitForSeconds(delay);
            }

            dialogue.text = textToWrite;
            dialogue.enableAutoSizing = true;
            IsDialogueOver = true;
        }
    }

    void OnMouseDown() {
        if (!IsDialogueOver) {
            IsDialogueOver = true;
            return;
        }

        if (currentConversation.HasNextDialogue()) {
            UpdateDialogueUI(currentEntity, currentConversation);
        } else {
            Destroy(this.gameObject);
        }

    }

}
