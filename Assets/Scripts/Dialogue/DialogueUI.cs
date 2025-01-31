using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    public bool isOpen { get; private set; }

    private ResponseHandler responseHandler;
    private TypeWriterEffect typeWriterEffect;

    void Start() {
        typeWriterEffect = GetComponent<TypeWriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject) {
        isOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents) {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject) {

        for (int i = 0; i < dialogueObject.Dialogue.Length; i++) {
            String dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);

            textLabel.text = dialogue;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (dialogueObject.HasResponses) {
            responseHandler.ShowResponses(dialogueObject.Responses);
        } else {
            CloseDialogueBox();
        }
    }

    private IEnumerator RunTypingEffect(String dialogue) {
        typeWriterEffect.Run(dialogue, textLabel);

        while (typeWriterEffect.isRunning) {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space)) {
                typeWriterEffect.Stop();
            }
        }
    }

    public void CloseDialogueBox() {
        isOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = String.Empty;
    }
}
