using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TaskPanel : MonoBehaviour
{
    public GameObject taskEntryPrefab;
    public Transform taskListParent;
    public TextMeshProUGUI pageText;
    public Button prevPageButton;
    public Button nextPageButton;
    public TaskDetailPopup detailPopup;

    private bool previousCanMove;
    private bool previousCanJump;
    private bool previousCanAttack;
    private bool previousCanInteract;
    private bool previousCanBackpack;
    private bool previousCanDodge;
    private List<string> activeTipIDs;

    private int currentPage = 1;
    private int tasksPerPage = 5;
    private List<DialogueEntryCSV> activeTasks;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (TaskManager.Instance != null)
        {
            activeTasks = TaskManager.Instance.activeTasks;
            RefreshPanel(); //每次打开都刷新一下
        }
    }

    public void RefreshPanel()
    {
        //清空旧条目
        foreach (Transform child in taskListParent)
        {
            Destroy(child.gameObject);
        }

        if (activeTasks == null || activeTasks.Count == 0)
        {
            pageText.text = "Page 1 / 1";
            return;
        }

        //计算分页
        int totalPages = Mathf.CeilToInt((float)activeTasks.Count / tasksPerPage);
        if (currentPage > totalPages && totalPages > 0) currentPage = totalPages;

        int startIndex = (currentPage - 1) * tasksPerPage;
        int endIndex = Mathf.Min(startIndex + tasksPerPage, activeTasks.Count);

        //生成当前页的任务条目
        for (int i = startIndex; i < endIndex; i++)
        {
            var task = activeTasks[i];
            var entry = Instantiate(taskEntryPrefab, taskListParent);

            //获取Title
            var titleText = entry.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            if (titleText != null)
            {
                titleText.text = string.IsNullOrEmpty(task.taskTitle) ? "未命名任务" : task.taskTitle;
            }

            //获取Button组件
            Button button = entry.GetComponent<Button>();
            if (button == null) button = entry.AddComponent<Button>(); // 如果Prefab忘加Button，自动加一个

            button.onClick.AddListener(() =>
            {
                Debug.Log($"点击了任务按钮：{task.taskTitle}"); // 【调试点 1】

                if (detailPopup != null)
                {
                    detailPopup.Show(task);
                }
                else
                {
                    Debug.LogError("报错原因：TaskPanel 上的 DetailPopup 变量没赋值！快去拖拽！"); // 【调试点 2】
                }
            });
        }

        //更新页码UI
        pageText.text = $"Page {currentPage} / {totalPages}";
        prevPageButton.interactable = currentPage > 1;
        nextPageButton.interactable = currentPage < totalPages;
    }

    public void OnPrevPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            RefreshPanel();
        }
    }

    public void OnNextPage()
    {
        if (currentPage < Mathf.CeilToInt((float)activeTasks.Count / tasksPerPage))
        {
            currentPage++;
            RefreshPanel();
        }
    }

    public void TogglePanel()
    {
        if (PlayerActionManager.Instance == null || !PlayerActionManager.Instance.canTask) return;

        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);

        if (!isActive)
        {
            // 打开面板时，先关掉详情弹窗（防止残留）
            if (detailPopup != null) detailPopup.Hide();
            RefreshPanel();

            if (ActionTipUI.Instance != null)   // 隐藏 Tip
                activeTipIDs = ActionTipUI.Instance.HideAllTipsAndReturnActive();
            //保存之前状态
            previousCanMove = PlayerActionManager.Instance.canMove;
            previousCanJump = PlayerActionManager.Instance.canJump;
            previousCanAttack = PlayerActionManager.Instance.canAttack;
            previousCanInteract = PlayerActionManager.Instance.canInteract;
            previousCanBackpack = PlayerActionManager.Instance.canBackpack;
            previousCanDodge = PlayerActionManager.Instance.canDodge;

            //打开面板时禁用除canTask外的操作
            PlayerActionManager.Instance.EnableOnlyAction("task");
        }
        else
        {
            //关闭面板时，强制关闭弹窗
            if (detailPopup != null) detailPopup.Hide();

            //恢复之前的操作状态
            PlayerActionManager.Instance.canMove = previousCanMove;
            PlayerActionManager.Instance.canJump = previousCanJump;
            PlayerActionManager.Instance.canAttack = previousCanAttack;
            PlayerActionManager.Instance.canInteract = previousCanInteract;
            PlayerActionManager.Instance.canBackpack = previousCanBackpack;
            PlayerActionManager.Instance.canDodge = previousCanDodge;

            //恢复 Tip
            if (ActionTipUI.Instance != null && activeTipIDs != null)
                ActionTipUI.Instance.RestoreTips(activeTipIDs);
        }
    }
}