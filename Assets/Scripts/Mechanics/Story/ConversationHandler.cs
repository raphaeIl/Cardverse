using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConversationHandler : MonoBehaviour {

    private Queue<string> dialogues;

    [SerializeField] public List<string> test;

    public void SetConversation(List<DialogueData> dialogues) {
        this.dialogues = new Queue<string>();

        if (dialogues == null || !dialogues.Any())
            return;

        foreach (DialogueData dialogue in dialogues)
            this.dialogues.Enqueue(dialogue.dialogueText);

        //EnqeueDialogues();

        test = this.dialogues.ToList();
    }

    //private void EnqeueDialogues(ConversationData conversation) {
    //    if (!conversation.HasOptions()) {
    //        dialogues.Enqueue(conversation.text); // move this up
    //        return;
    //    }
    //    dialogues.Enqueue(conversation.text);
    //    EnqeueDialogues(conversation.options[0].nextDialogueId);
    //}

    public string GetNextDialogue() {
        if (!HasNextDialogue()) {
            return null;
        }

        return dialogues.Dequeue();
    }

    public bool HasNextDialogue() {
        return dialogues.Count > 0;
    }

    [System.Serializable]
    public struct DialogueData {
        public string dialogueID;
        [Multiline]
        public string dialogueText;
        public List<DialogueOption> dialogueOptions;

        public bool HasOptions() {
            return dialogueOptions.Count > 0;
        }

    }

    [System.Serializable]
    public struct DialogueOption {
        public string text;
        public string nextDialogueId;

        public bool HasNextDialogue() {
            return !string.IsNullOrEmpty(nextDialogueId);
        }
    }
}

