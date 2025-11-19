using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class TaskNPC : MonoBehaviour, IInteractable
{
    [Header("NPC 基本信息")]
    public string npcName;

    [Header("任务相关")]
    public string taskID;               // Inspector 指定这次任务ID
    public GameObject taskPopupPrefab;
    private bool hasGivenTask = false;

    [Header("交互提示")]
    public GameObject indicatorPrefab;
    [Tooltip("提示在NPC头顶的偏移")]
    public Vector3 indicatorOffset = new Vector3(0, 1.5f, 0);
    [Tooltip("提示的缩放")]
    public Vector3 indicatorScale = Vector3.one;

    private GameObject indicatorInstance;
    private List<string> activeTipIDs; // 记录当前显示的 Tip
    private bool isTalking = false;

    private void Start()
    {
        // 生成提示符（可一直显示）
        if (indicatorPrefab != null)
        {
            indicatorInstance = Instantiate(indicatorPrefab, transform);
            indicatorInstance.transform.localPosition = indicatorOffset;
            indicatorInstance.transform.localScale = indicatorScale;
            indicatorInstance.SetActive(true);
        }
    }

    public void Interact()
    {
        Debug.Log($"Trying to interact with {npcName}. isTalking={isTalking}, hasGivenTask={hasGivenTask}");
        if (isTalking || hasGivenTask) return;

        isTalking = true;

        // 关闭所有 Tip，并记录当前显示状态
        if (ActionTipUI.Instance != null)
            activeTipIDs = ActionTipUI.Instance.HideAllTipsAndReturnActive();


        PlayerActionManager.Instance.DisableAll();

        // 获取 CSV 对话数据
        DialogueEntryCSV data = DialogueLoader.Instance.GetDialogueCSV(npcName, taskID);
        if (data == null)
        {
            Debug.LogError($"NPC 对话未找到: {npcName} taskID: {taskID}");
            PlayerActionManager.Instance.EnableAll();
            isTalking = false;
            return;
        }

        // 播放初始对话
        if (data.dialogueLines != null && data.dialogueLines.Length > 0)
            DialogueManager.Instance.StartDialogue(npcName, data.dialogueLines);

        StartCoroutine(WaitForDialogueThenShowTask(data));
    }

    private IEnumerator WaitForDialogueThenShowTask(DialogueEntryCSV data)
    {
        // 记录当前显示状态
        Dictionary<string, bool> tipStates = new Dictionary<string, bool>();
        if (ActionTipUI.Instance != null)
        {
            foreach (var tip in ActionTipUI.Instance.tips)
            {
                tipStates[tip.tipID] = tip.tipText.gameObject.activeSelf;
                ActionTipUI.Instance.HideTip(tip.tipID);
            }
        }

        PlayerActionManager.Instance.DisableAll();

        // 等待对话结束
        yield return new WaitUntil(() => DialogueManager.Instance.dialoguePanel.activeSelf == false);

        // 弹出任务窗口
        ShowTaskPopup(data);
        isTalking = false;

        // 对话结束后恢复原本显示的提示
        if (ActionTipUI.Instance != null)
        {
            foreach (var tip in ActionTipUI.Instance.tips)
            {
                if (tipStates.TryGetValue(tip.tipID, out bool wasActive) && wasActive)
                {
                    tip.tipText.gameObject.SetActive(true);
                    if (tip.backgroundRect != null)
                        tip.backgroundRect.gameObject.SetActive(true);
                }
            }
        }
    }


    private void ShowTaskPopup(DialogueEntryCSV data)
    {
        if (taskPopupPrefab == null) return;

        // 实例化 Prefab，不指定 parent
        var popupGO = Instantiate(taskPopupPrefab);

        // 获取脚本组件
        var popup = popupGO.GetComponent<TaskPopup>();

        // 保证 Canvas 排序在最前
        Canvas popupCanvas = popupGO.GetComponent<Canvas>();
        if (popupCanvas != null)
        {
            popupCanvas.overrideSorting = true;
            popupCanvas.sortingOrder = 999; // 根据需要调整
        }

        // 设置任务内容和按钮回调
        popup.Setup(
            string.IsNullOrEmpty(data.taskTitle) ? "任务" : data.taskTitle,
            string.IsNullOrEmpty(data.taskDescription) ? "" : data.taskDescription,
            () => OnAccept(data),
            OnReject
        );

        popup.Show();
    }


    private void OnAccept(DialogueEntryCSV data)
    {
        hasGivenTask = true;
        TaskManager.Instance.AddTask(data);   //将任务添加至已接受的任务列表
        if (indicatorInstance != null)
            indicatorInstance.SetActive(false);

        // 播放任务接受后的对话
        if (data.afterAcceptDialogue != null && data.afterAcceptDialogue.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(data.npcName, data.afterAcceptDialogue);
            StartCoroutine(WaitForDialogueEndThenUnlock());
        }
        else
        {
            PlayerActionManager.Instance.EnableAll();
            // 恢复 Tip
            if (ActionTipUI.Instance != null && activeTipIDs != null)
                ActionTipUI.Instance.RestoreTips(activeTipIDs);
        }
    }

    private void OnReject()
    {
        PlayerActionManager.Instance.EnableAll();
        // 恢复 Tip
        if (ActionTipUI.Instance != null && activeTipIDs != null)
            ActionTipUI.Instance.RestoreTips(activeTipIDs);
    }

    private IEnumerator WaitForDialogueEndThenUnlock()
    {
        yield return new WaitUntil(() => DialogueManager.Instance.dialoguePanel.activeSelf == false);
        PlayerActionManager.Instance.EnableAll();
        // 恢复 Tip
        if (ActionTipUI.Instance != null && activeTipIDs != null)
            ActionTipUI.Instance.RestoreTips(activeTipIDs);
    }
}
