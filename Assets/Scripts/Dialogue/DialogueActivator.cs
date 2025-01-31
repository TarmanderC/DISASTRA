using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    public bool isCutscene;
    private Collider2D dialogueCollider;

    private void Awake()
    {
        dialogueCollider = GetComponent<Collider2D>(); // Get the collider on this object
    }

    public void DisableDialogue()
    {
        if (dialogueCollider != null)
        {
            dialogueCollider.enabled = false; // Disable the trigger collider
        }
    }


    public void UpdateDialogueObject(DialogueObject dialogueObject) {
        this.dialogueObject = dialogueObject;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && other.TryGetComponent(out KnightMovement player)) {
            player.Interactable = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && other.TryGetComponent(out KnightMovement player)) {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this) {
                player.Interactable = null;
            }
        }
    }

    public void Interact(KnightMovement player) {

        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>()) {
            if (responseEvents.DialogueObject == dialogueObject) {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        player.DialogueUI.ShowDialogue(dialogueObject);
    }
}
