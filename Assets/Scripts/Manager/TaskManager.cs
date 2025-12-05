using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    public List<DialogueEntryCSV> activeTasks = new List<DialogueEntryCSV>();    // 当前未完成 + 已接受的任务
    public Dictionary<string, int> taskProgressCounts = new Dictionary<string, int>();
    public List<string> finishedTaskIDs = new List<string>();

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
            if (!taskProgressCounts.ContainsKey(task.taskID))
            {
                taskProgressCounts[task.taskID] = 0;
            }
        }
    }

    public void UpdateTaskProgress(string targetID, int amount = 1)
    {
        //遍历所有正在进行及其目标匹配的任务
        foreach (var task in activeTasks)
        {
            if (task.targetID == targetID)
            {
                //增加计数
                if (taskProgressCounts.ContainsKey(task.taskID))
                {
                    taskProgressCounts[task.taskID] += amount;
                }
                else
                {
                    taskProgressCounts[task.taskID] = amount;
                }

                //检查是否完成
                CheckTaskFinish(task);
            }
        }
        //刷新UI
        FindObjectOfType<TaskPanel>()?.RefreshPanel();
    }

    public List<TaskSaveData> GetTaskSaveData()
    {
        List<TaskSaveData> dataList = new List<TaskSaveData>();

        //遍历所有活跃任务（包括进行中和已完成但还在列表里的）
        foreach (var task in activeTasks)
        {
            //组合唯一Key：NPC名_任务ID
            //这样读档时才能知道是哪个NPC的任务
            string fullID = $"{task.npcName}_{task.taskID}";

            TaskSaveData data = new TaskSaveData
            {
                taskID = fullID,                        //保存组合ID
                currentCount = GetProgress(task.taskID),//保存当前杀怪数
                isFinished = IsFinished(task.taskID)    //保存是否完成
            };
            dataList.Add(data);
        }
        return dataList;
    }

    public void LoadTaskSaveData(List<TaskSaveData> dataList)
    {
        //清空当前状态
        activeTasks.Clear();
        taskProgressCounts.Clear();
        finishedTaskIDs.Clear();

        if (dataList == null) return;

        foreach (var data in dataList)
        {
            //解析组合ID (NPC名_任务ID)
            int splitIndex = data.taskID.LastIndexOf('_');
            if (splitIndex < 0) continue;

            string npcName = data.taskID.Substring(0, splitIndex);
            string rawTaskID = data.taskID.Substring(splitIndex + 1);

            //恢复进度数据
            if (taskProgressCounts.ContainsKey(rawTaskID))
                taskProgressCounts[rawTaskID] = data.currentCount;
            else
                taskProgressCounts.Add(rawTaskID, data.currentCount);

            //恢复完成状态
            if (data.isFinished)
            {
                if (!finishedTaskIDs.Contains(rawTaskID))
                    finishedTaskIDs.Add(rawTaskID);
            }

            //恢复activeTasks列表
            var task = DialogueLoader.Instance.GetDialogueCSV(npcName, rawTaskID);
            if (task != null)
            {
                if (!activeTasks.Contains(task))
                {
                    activeTasks.Add(task);
                }
            }
        }

        //刷新 UI
        FindObjectOfType<TaskPanel>()?.RefreshPanel();
    }



    private void CheckTaskFinish(DialogueEntryCSV task)
    {
        int current = taskProgressCounts[task.taskID];
        if (current >= task.targetAmount)
        {
            Debug.Log($"任务完成：{task.taskTitle}");

            //标记完成
            if (!finishedTaskIDs.Contains(task.taskID))
            {
                finishedTaskIDs.Add(task.taskID);
                //TODO：可以在这里播放完成音效或特效
                NotificationManager.Instance?.Show($"任务完成：{task.taskTitle}");
            }
        }
    }

    public int GetProgress(string taskID)
    {
        return taskProgressCounts.ContainsKey(taskID) ? taskProgressCounts[taskID] : 0;
    }

    public bool IsFinished(string taskID)
    {
        return finishedTaskIDs.Contains(taskID);
    }
}
