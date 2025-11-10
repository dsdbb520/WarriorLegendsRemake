using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DialogueLoader : MonoBehaviour
{
    public static DialogueLoader Instance;

    private Dictionary<string, DialogueEntryCSV> dialogues;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadDialogues();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadDialogues()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "dialogues.json");
        if (!File.Exists(path))
        {
            Debug.LogError("找不到对话 JSON 文件: " + path);
            dialogues = new Dictionary<string, DialogueEntryCSV>();
            return;
        }

        string json = File.ReadAllText(path);
        DialogueDatabaseCSV db = JsonUtility.FromJson<DialogueDatabaseCSV>(json);

        dialogues = new Dictionary<string, DialogueEntryCSV>();
        foreach (var entry in db.entries)
        {
            string key = $"{entry.npcName}_{entry.taskID}";
            if (!dialogues.ContainsKey(key))
                dialogues.Add(key, entry);
        }
    }

    // 按 npcName + taskID 获取对话
    public DialogueEntryCSV GetDialogueCSV(string npcName, string taskID)
    {
        string key = $"{npcName}_{taskID}";
        if (dialogues != null && dialogues.ContainsKey(key))
            return dialogues[key];

        Debug.LogWarning($"找不到 NPC 对话: {npcName} taskID: {taskID}");
        return null;
    }
}
