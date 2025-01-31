using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Response
{
    [SerializeField] private String responseText;
    [SerializeField] private DialogueObject dialogueObject;

    public String ResponseText => responseText;

    public DialogueObject DialogueObject => dialogueObject;
}
