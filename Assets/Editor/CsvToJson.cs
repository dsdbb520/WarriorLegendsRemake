using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CsvToJson : EditorWindow
{
    private string csvPath = "Assets/Dialogues.csv";
    private string jsonOutputPath = "Assets/StreamingAssets/dialogues.json";

    // 正则表达式：匹配CSV中的一列（支持双引号包裹，支持引号转义）
    private const string CSV_SPLIT_REGEX = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

    [MenuItem("Tools/CSV To Dialogue JSON")]
    public static void ShowWindow()
    {
        GetWindow<CsvToJson>("CSV To JSON");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV -> JSON 导出工具 (增强版)", EditorStyles.boldLabel);
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

        // 从第1行开始（跳过表头）
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 使用正则分割
            string[] cols = Regex.Split(line, CSV_SPLIT_REGEX);

            // 清理引号
            for (int j = 0; j < cols.Length; j++)
            {
                cols[j] = cols[j].Trim();
                if (cols[j].StartsWith("\"") && cols[j].EndsWith("\""))
                {
                    cols[j] = cols[j].Substring(1, cols[j].Length - 2).Replace("\"\"", "\"");
                }
            }

            // 格式检查
            if (cols.Length < 7)
            {
                if (cols.Length == 6)
                {
                    var newCols = new List<string>(cols);
                    newCols.Add("");
                    cols = newCols.ToArray();
                }
                else
                {
                    Debug.LogWarning($"跳过格式错误的行 (行号 {i + 1}): {line}");
                    continue;
                }
            }

            // 数据映射
            string npcName = cols[0];
            string type = cols[1];
            string content = cols[2];
            string taskID = cols[3];
            string taskTitle = cols[4];
            string taskDescription = cols[5];
            string taskGoal = cols[6];

            string key = string.IsNullOrEmpty(taskID) ? npcName : $"{npcName}_{taskID}";

            if (!dict.ContainsKey(key))
            {
                dict[key] = new DialogueEntryCSV
                {
                    npcName = npcName,
                    taskID = taskID,
                    taskTitle = taskTitle,
                    taskDescription = taskDescription,
                    taskGoal = taskGoal,
                    dialogueLines = new string[0],
                    afterAcceptDialogue = new string[0]
                };
            }

            var entry = dict[key];

            switch (type)
            {
                case "dialogueLines":
                    var listD = new List<string>(entry.dialogueLines);
                    listD.Add(content);
                    entry.dialogueLines = listD.ToArray();
                    break;

                case "afterAcceptDialogue":
                    var listA = new List<string>(entry.afterAcceptDialogue);
                    listA.Add(content);
                    entry.afterAcceptDialogue = listA.ToArray();
                    break;

                case "task":
                    entry.taskID = taskID;
                    entry.taskTitle = taskTitle;
                    entry.taskDescription = taskDescription;
                    entry.taskGoal = taskGoal;
                    break;
            }
        }

        DialogueDatabaseCSV db = new DialogueDatabaseCSV
        {
            entries = new List<DialogueEntryCSV>(dict.Values).ToArray()
        };

        if (!Directory.Exists(Path.GetDirectoryName(jsonOutputPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(jsonOutputPath));
        }

        File.WriteAllText(jsonOutputPath, JsonUtility.ToJson(db, true), System.Text.Encoding.UTF8);

        Debug.Log($"<color=green>导出成功！</color> 共导出 {db.entries.Length} 个任务/NPC组。\n路径: {jsonOutputPath}");
        AssetDatabase.Refresh();
    }
}