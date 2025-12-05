using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskDetailPopup : MonoBehaviour
{
    [Header("UI 组件")]
    public TextMeshProUGUI titleText;       //任务名称
    public TextMeshProUGUI descText;        //任务介绍
    public TextMeshProUGUI goalText;        //任务目标
    public Button closeButton;              //关闭按钮

    private void Awake()
    {
        //绑定关闭按钮
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
        gameObject.SetActive(false);
    }

    public void Show(DialogueEntryCSV task)
    {
        gameObject.SetActive(true);

        // 更新文本显示
        if (titleText != null) titleText.text = task.taskTitle;
        if (descText != null) descText.text = task.taskDescription;
        if (goalText != null)
        {
            // 获取当前进度
            int current = TaskManager.Instance.GetProgress(task.taskID);
            int target = task.targetAmount;
            bool isFinished = TaskManager.Instance.IsFinished(task.taskID);

            if (isFinished)
            {
                goalText.text = $"<color=green>已完成</color>";
            }
            else
            {
                if (target > 0)
                    goalText.text = $"目标：{task.taskGoal} ({current}/{target})";
                else
                    goalText.text = $"目标：{task.taskGoal}";
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}