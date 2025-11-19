using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TaskPanel : MonoBehaviour
{
    public GameObject taskEntryPrefab;      // 任务条目的 Prefab
    public Transform taskListParent;        // 任务列表的父物体

    private bool previousCanMove;
    private bool previousCanJump;
    private bool previousCanAttack;
    private bool previousCanInteract;

    // 保存打开面板前显示的 Tip ID
    private List<string> activeTipIDs;

    private void Awake()
    {
        // 初始隐藏面板
        gameObject.SetActive(false);
    }

    // 刷新任务面板
    public void RefreshPanel()
    {
        // 清空旧任务
        foreach (Transform child in taskListParent)
        {
            Destroy(child.gameObject);
        }

        // 从 TaskManager 取任务
        foreach (var task in TaskManager.Instance.activeTasks)
        {
            var entry = Instantiate(taskEntryPrefab, taskListParent);

            entry.transform.Find("Title").GetComponent<TextMeshProUGUI>().text =
                string.IsNullOrEmpty(task.taskTitle) ? "未命名任务" : task.taskTitle;

            entry.transform.Find("Description").GetComponent<TextMeshProUGUI>().text =
                string.IsNullOrEmpty(task.taskDescription) ? "" : task.taskDescription;
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

            // 打开面板时禁用除 canTask 外的操作
            PlayerActionManager.Instance.canMove = false;
            PlayerActionManager.Instance.canJump = false;
            PlayerActionManager.Instance.canAttack = false;
            PlayerActionManager.Instance.canInteract = false;

        }
        else
        {
            // 恢复之前的操作状态
            PlayerActionManager.Instance.canMove = previousCanMove;
            PlayerActionManager.Instance.canJump = previousCanJump;
            PlayerActionManager.Instance.canAttack = previousCanAttack;
            PlayerActionManager.Instance.canInteract = previousCanInteract;

            // 恢复 Tip
            if (ActionTipUI.Instance != null && activeTipIDs != null)
                ActionTipUI.Instance.RestoreTips(activeTipIDs);
        }
    }
}
