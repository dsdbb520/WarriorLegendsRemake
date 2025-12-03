using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    public List<DialogueEntryCSV> activeTasks = new List<DialogueEntryCSV>();    // 当前未完成 + 已接受的任务

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

        public void AddTask(DialogueEntryCSV task)
    {
        if (!activeTasks.Contains(task))
        {
            activeTasks.Add(task);
        }
    }

    //获取当前任务的ID列表（用于存档）
    public List<string> GetTaskIDs()
    {
        List<string> ids = new List<string>();
        foreach (var task in activeTasks)
        {
            //组合ID：NPC名字_任务ID
            ids.Add($"{task.npcName}_{task.taskID}");
        }
        return ids;
    }

    //根据ID列表恢复任务（用于读档）
    public void LoadTaskIDs(List<string> ids)
    {
        activeTasks.Clear();
        foreach (var fullID in ids)
        {
            string[] split = fullID.Split('_');
            if (split.Length == 2)
            {
                var task = DialogueLoader.Instance.GetDialogueCSV(split[0], split[1]);
                if (task != null)
                {
                    AddTask(task);
                }
            }
        }
    }
}
