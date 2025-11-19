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
}
