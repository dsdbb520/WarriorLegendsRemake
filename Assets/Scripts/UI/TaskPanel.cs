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

    private List<string> activeTipIDs;
    private int currentPage = 1;
    private const int TasksPerPage = 5;
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
            RefreshPanel();
        }
    }

    public void RefreshPanel()
    {
        foreach (Transform child in taskListParent)
            Destroy(child.gameObject);

        if (activeTasks == null || activeTasks.Count == 0)
        {
            pageText.text = "Page 1 / 1";
            return;
        }

        int totalPages = Mathf.CeilToInt((float)activeTasks.Count / TasksPerPage);
        if (currentPage > totalPages) currentPage = totalPages;

        int startIndex = (currentPage - 1) * TasksPerPage;
        int endIndex = Mathf.Min(startIndex + TasksPerPage, activeTasks.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            var task = activeTasks[i];
            var entry = Instantiate(taskEntryPrefab, taskListParent);

            var titleText = entry.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            if (titleText != null)
                titleText.text = string.IsNullOrEmpty(task.taskTitle) ? "未命名任务" : task.taskTitle;

            Button button = entry.GetComponent<Button>() ?? entry.AddComponent<Button>();
            button.onClick.AddListener(() => detailPopup?.Show(task));
        }

        pageText.text = $"Page {currentPage} / {totalPages}";
        prevPageButton.interactable = currentPage > 1;
        nextPageButton.interactable = currentPage < totalPages;
    }

    public void OnPrevPage()
    {
        if (currentPage > 1) { currentPage--; RefreshPanel(); }
    }

    public void OnNextPage()
    {
        int totalPages = Mathf.CeilToInt((float)activeTasks.Count / TasksPerPage);
        if (currentPage < totalPages) { currentPage++; RefreshPanel(); }
    }

    public void TogglePanel()
    {
        if (PlayerActionManager.Instance == null || !PlayerActionManager.Instance.canTask) return;

        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);

        if (!isActive)
        {
            if (detailPopup != null) detailPopup.Hide();
            RefreshPanel();
            if (ActionTipUI.Instance != null)
                activeTipIDs = ActionTipUI.Instance.HideAllTipsAndReturnActive();

            // Lock everything except the task action
            PlayerActionManager.Instance.LockActions("task", "task");
        }
        else
        {
            if (detailPopup != null) detailPopup.Hide();
            PlayerActionManager.Instance.UnlockActions("task");
            if (ActionTipUI.Instance != null && activeTipIDs != null)
                ActionTipUI.Instance.RestoreTips(activeTipIDs);
        }
    }
}
