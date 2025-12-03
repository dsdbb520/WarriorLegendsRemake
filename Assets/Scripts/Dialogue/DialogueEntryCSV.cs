using System;
using UnityEngine;

[Serializable]
public class DialogueEntryCSV
{
    public string npcName;
    public string taskID;
    public string taskTitle;
    public string taskDescription;
    public string taskGoal;
    public string[] dialogueLines;
    public string[] afterAcceptDialogue;
}

[Serializable]
public class DialogueDatabaseCSV
{
    public DialogueEntryCSV[] entries;
}
