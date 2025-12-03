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
            goalText.text = string.IsNullOrEmpty(task.taskGoal) ? "目标：查看详情" : task.taskGoal;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}