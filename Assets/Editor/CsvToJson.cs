using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class DialogueDatabase
{
    public DialogueEntryCSV[] entries;
}

public class CsvToJson : EditorWindow
{
    private string csvPath = "Assets/Dialogues.csv";
    private string jsonOutputPath = "Assets/StreamingAssets/dialogues.json";

    [MenuItem("Tools/CSV To Dialogue JSON")]
    public static void ShowWindow()
    {
        GetWindow<CsvToJson>("CSV To JSON");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV -> JSON 导出工具", EditorStyles.boldLabel);
        csvPath = EditorGUILayout.TextField("CSV 文件路径", csvPath);
        jsonOutputPath = EditorGUILayout.TextField("JSON 输出路径", jsonOutputPath);

        if (GUILayout.Button("导出 JSON"))
        {
            ExportCsvToJson();
        }
    }

    private void ExportCsvToJson()
    {
        if (!File.Exists(csvPath))
        {
            Debug.LogError("找不到 CSV 文件: " + csvPath);
            return;
        }

        string[] lines = File.ReadAllLines(csvPath, System.Text.Encoding.UTF8);
        var dict = new Dictionary<string, DialogueEntryCSV>();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Split(',');

            if (cols.Length < 6) continue;

            string npcName = cols[0].Trim();
            string type = cols[1].Trim();
            string content = cols[2].Trim();
            string taskID = cols[3].Trim();
            string taskTitle = cols[4].Trim();
            string taskDescription = cols[5].Trim();

            // 以 npcName+taskID 作为 key，如果 taskID 为空，则用 npcName
            string key = string.IsNullOrEmpty(taskID) ? npcName : npcName + "_" + taskID;

            if (!dict.ContainsKey(key))
            {
                dict[key] = new DialogueEntryCSV
                {
                    npcName = npcName,
                    taskID = taskID,
                    taskTitle = taskTitle,
                    taskDescription = taskDescription,
                    dialogueLines = new string[0],
                    afterAcceptDialogue = new string[0]
                };
            }

            var entry = dict[key];

            // 对话类型处理
            if (type == "dialogueLines")
            {
                var list = new List<string>(entry.dialogueLines);
                list.Add(content);
                entry.dialogueLines = list.ToArray();
            }
            else if (type == "afterAcceptDialogue")
            {
                var list = new List<string>(entry.afterAcceptDialogue);
                list.Add(content);
                entry.afterAcceptDialogue = list.ToArray();
            }
            // task 类型处理（覆盖 title/desc，保留对话）
            else if (type == "task")
            {
                entry.taskID = taskID;
                entry.taskTitle = taskTitle;
                entry.taskDescription = taskDescription;
            }
        }

        DialogueDatabase db = new DialogueDatabase
        {
            entries = new List<DialogueEntryCSV>(dict.Values).ToArray()
        };

        Directory.CreateDirectory(Path.GetDirectoryName(jsonOutputPath));
        File.WriteAllText(jsonOutputPath, JsonUtility.ToJson(db, true), System.Text.Encoding.UTF8);

        Debug.Log("导出完成: " + jsonOutputPath);
    }

}
