using UnityEngine;

public class DialogueNPC : BaseNPC
{
    [Header("对话数据")]
    public string npcName;
    [TextArea] public string[] dialogueLines;

    public override void Interact()
    {
        base.Interact();
        DialogueManager.Instance.StartDialogue(npcName, dialogueLines);
    }
}
