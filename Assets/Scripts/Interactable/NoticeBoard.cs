using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeBoard : MonoBehaviour, IInteractable
{
    public string boardName;
    public string dialogueID;

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogueID);
    }
}