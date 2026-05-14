using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TaskManager : SingletonMono<TaskManager>
{
    public List<DialogueEntryCSV> activeTasks = new List<DialogueEntryCSV>();
    public Dictionary<string, int> taskProgressCounts = new Dictionary<string, int>();
    public List<string> finishedTaskIDs = new List<string>();

    public void AddTask(DialogueEntryCSV task)
    {
        if (!activeTasks.Contains(task))
        {
            activeTasks.Add(task);
            if (!taskProgressCounts.ContainsKey(task.taskID))
                taskProgressCounts[task.taskID] = 0;
        }
    }

    public void UpdateTaskProgress(string targetID, int amount = 1)
    {
        foreach (var task in activeTasks)
        {
            if (task.targetID != targetID) continue;

            if (taskProgressCounts.ContainsKey(task.taskID))
                taskProgressCounts[task.taskID] += amount;
            else
                taskProgressCounts[task.taskID] = amount;

            CheckTaskFinish(task);
        }

        FindObjectOfType<TaskPanel>()?.RefreshPanel();
    }

    public List<TaskSaveData> GetTaskSaveData()
    {
        var dataList = new List<TaskSaveData>();
        foreach (var task in activeTasks)
        {
            dataList.Add(new TaskSaveData
            {
                taskID       = $"{task.npcName}_{task.taskID}",
                currentCount = GetProgress(task.taskID),
                isFinished   = IsFinished(task.taskID)
            });
        }
        return dataList;
    }

    public void LoadTaskSaveData(List<TaskSaveData> dataList)
    {
        activeTasks.Clear();
        taskProgressCounts.Clear();
        finishedTaskIDs.Clear();

        if (dataList == null) return;

        foreach (var data in dataList)
        {
            int splitIndex = data.taskID.LastIndexOf('_');
            if (splitIndex < 0) continue;

            string npcName   = data.taskID.Substring(0, splitIndex);
            string rawTaskID = data.taskID.Substring(splitIndex + 1);

            taskProgressCounts[rawTaskID] = data.currentCount;

            if (data.isFinished && !finishedTaskIDs.Contains(rawTaskID))
                finishedTaskIDs.Add(rawTaskID);

            var task = DialogueLoader.Instance.GetDialogueCSV(npcName, rawTaskID);
            if (task != null && !activeTasks.Contains(task))
                activeTasks.Add(task);
        }

        FindObjectOfType<TaskPanel>()?.RefreshPanel();
    }

    private void CheckTaskFinish(DialogueEntryCSV task)
    {
        if (taskProgressCounts[task.taskID] >= task.targetAmount && !finishedTaskIDs.Contains(task.taskID))
        {
            finishedTaskIDs.Add(task.taskID);
            NotificationManager.Instance?.Show($"任务完成：{task.taskTitle}");
        }
    }

    public int GetProgress(string taskID) =>
        taskProgressCounts.TryGetValue(taskID, out int v) ? v : 0;

    public bool IsFinished(string taskID) => finishedTaskIDs.Contains(taskID);
}
