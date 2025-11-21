using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TaskPanel : MonoBehaviour
{
    public GameObject taskEntryPrefab;      // 任务条目的 Prefab
    public Transform taskListParent;        // 任务列表的父物体
    public TextMeshProUGUI pageText;        // 显示当前页数和总页数的文本
    public Button prevPageButton;           // 上一页按钮
    public Button nextPageButton;           // 下一页按钮

    private bool previousCanMove;
    private bool previousCanJump;
    private bool previousCanAttack;
    private bool previousCanInteract;
    private bool previousCanBackpack;

    // 保存打开面板前显示的 Tip ID
    private List<string> activeTipIDs;

    // 分页控制
    private int currentPage = 1;            // 当前页
    private int tasksPerPage = 5;           // 每页显示的任务数量
    private List<DialogueEntryCSV> activeTasks;         // 当前的任务列表

    private void Awake()
    {
        // 初始隐藏面板
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (TaskManager.Instance != null)
        {
            activeTasks = TaskManager.Instance.activeTasks;   // 获取任务列表
        }
    }

    // 刷新任务面板
    public void RefreshPanel()
    {
        // 清空旧任务
        foreach (Transform child in taskListParent)
        {
            Destroy(child.gameObject);
        }

        // 获取当前页需要显示的任务
        int startIndex = (currentPage - 1) * tasksPerPage;
        int endIndex = Mathf.Min(startIndex + tasksPerPage, activeTasks.Count);

        // 显示当前页的任务
        for (int i = startIndex; i < endIndex; i++)
        {
            var task = activeTasks[i];
            var entry = Instantiate(taskEntryPrefab, taskListParent);

            var titleText = entry.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            var descriptionText = entry.transform.Find("Description").GetComponent<TextMeshProUGUI>();

            titleText.text = string.IsNullOrEmpty(task.taskTitle) ? "未命名任务" : task.taskTitle;
            descriptionText.text = string.IsNullOrEmpty(task.taskDescription) ? "" : task.taskDescription;

            // 动态调整任务条目的大小
            RectTransform entryRect = entry.GetComponent<RectTransform>();
            float width = Mathf.Max(titleText.preferredWidth, descriptionText.preferredWidth) + 20;  // 适应文本宽度
            entryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            entryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, titleText.preferredHeight + descriptionText.preferredHeight + 10); // 适应高度
        }

        // 更新页数显示
        pageText.text = $"Page {currentPage} / {Mathf.CeilToInt((float)activeTasks.Count / tasksPerPage)}";

        // 更新翻页按钮的状态
        prevPageButton.interactable = currentPage > 1;
        nextPageButton.interactable = currentPage < Mathf.CeilToInt((float)activeTasks.Count / tasksPerPage);
    }

    // 上一页按钮
    public void OnPrevPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            RefreshPanel();
        }
    }

    // 下一页按钮
    public void OnNextPage()
    {
        if (currentPage < Mathf.CeilToInt((float)activeTasks.Count / tasksPerPage))
        {
            currentPage++;
            RefreshPanel();
        }
    }

    // 按键切换面板显示
    public void TogglePanel()
    {
        if (PlayerActionManager.Instance == null || !PlayerActionManager.Instance.canTask) return;

        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive); // 切换显示状态

        if (!isActive)
        {
            RefreshPanel(); // 打开时刷新任务列表
            if (ActionTipUI.Instance != null)   // 隐藏 Tip
                activeTipIDs = ActionTipUI.Instance.HideAllTipsAndReturnActive();
            // 保存之前状态
            previousCanMove = PlayerActionManager.Instance.canMove;
            previousCanJump = PlayerActionManager.Instance.canJump;
            previousCanAttack = PlayerActionManager.Instance.canAttack;
            previousCanInteract = PlayerActionManager.Instance.canInteract;
            previousCanBackpack = PlayerActionManager.Instance.canBackpack;

            // 打开面板时禁用除 canTask 外的操作
            PlayerActionManager.Instance.EnableOnlyAction("task");

        }
        else
        {
            // 恢复之前的操作状态
            PlayerActionManager.Instance.canMove = previousCanMove;
            PlayerActionManager.Instance.canJump = previousCanJump;
            PlayerActionManager.Instance.canAttack = previousCanAttack;
            PlayerActionManager.Instance.canInteract = previousCanInteract;
            PlayerActionManager.Instance.canBackpack = previousCanBackpack;

            // 恢复 Tip
            if (ActionTipUI.Instance != null && activeTipIDs != null)
                ActionTipUI.Instance.RestoreTips(activeTipIDs);
        }
    }
}
