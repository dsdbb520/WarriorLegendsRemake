using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class TaskNPC : MonoBehaviour, IInteractable
{
    [Header("NPC 基本信息")]
    public string npcName;

    [Header("任务设置")]
    public string taskID;
    public GameObject taskPopupPrefab;
    private bool hasGivenTask = false;

    [Header("联动设置")]
    public BlockWall linkedWall;

    [Header("交互提示")]
    public GameObject indicatorPrefab;
    [Tooltip("显示在NPC头顶的偏移")]
    public Vector3 indicatorOffset = new Vector3(0, 1.5f, 0);
    public Vector3 indicatorScale = Vector3.one;

    private GameObject indicatorInstance;
    private List<string> activeTipIDs;
    private bool isTalking = false;

    private const string LockOwner = "tasknpc";

    private void Start()
    {
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
        if (isTalking || hasGivenTask) return;

        isTalking = true;

        if (ActionTipUI.Instance != null)
            activeTipIDs = ActionTipUI.Instance.HideAllTipsAndReturnActive();

        // Single lock — was incorrectly called twice before (here AND inside the coroutine)
        PlayerActionManager.Instance.LockActions(LockOwner);

        DialogueEntryCSV data = DialogueLoader.Instance.GetDialogueCSV(npcName, taskID);
        if (data == null)
        {
            Debug.LogError($"NPC dialogue not found: npcName={npcName} taskID={taskID}");
            Unlock();
            isTalking = false;
            return;
        }

        if (data.dialogueLines != null && data.dialogueLines.Length > 0)
            DialogueManager.Instance.StartDialogue(npcName, data.dialogueLines);

        StartCoroutine(WaitForDialogueThenShowTask(data));
    }

    private IEnumerator WaitForDialogueThenShowTask(DialogueEntryCSV data)
    {
        // No second DisableAll/LockActions here — the lock is already held from Interact()
        yield return new WaitUntil(() => !DialogueManager.Instance.dialoguePanel.activeSelf);

        ShowTaskPopup(data);
        isTalking = false;
    }

    private void ShowTaskPopup(DialogueEntryCSV data)
    {
        if (taskPopupPrefab == null) return;

        var popupGO = Instantiate(taskPopupPrefab);

        var popupCanvas = popupGO.GetComponent<Canvas>();
        if (popupCanvas != null)
        {
            popupCanvas.overrideSorting = true;
            popupCanvas.sortingOrder = 999;
        }

        popupGO.GetComponent<TaskPopup>().Setup(
            string.IsNullOrEmpty(data.taskTitle) ? "任务" : data.taskTitle,
            string.IsNullOrEmpty(data.taskDescription) ? "" : data.taskDescription,
            () => OnAccept(data),
            OnReject
        );

        popupGO.GetComponent<TaskPopup>().Show();
    }

    private void OnAccept(DialogueEntryCSV data)
    {
        hasGivenTask = true;
        TaskManager.Instance.AddTask(data);
        if (indicatorInstance != null) indicatorInstance.SetActive(false);
        if (linkedWall != null) linkedWall.Unlock();

        if (data.afterAcceptDialogue != null && data.afterAcceptDialogue.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(data.npcName, data.afterAcceptDialogue);
            StartCoroutine(WaitForDialogueEndThenUnlock());
        }
        else
        {
            Unlock();
        }
    }

    private void OnReject()
    {
        Unlock();
    }

    private IEnumerator WaitForDialogueEndThenUnlock()
    {
        yield return new WaitUntil(() => !DialogueManager.Instance.dialoguePanel.activeSelf);
        Unlock();
    }

    private void Unlock()
    {
        PlayerActionManager.Instance.UnlockActions(LockOwner);
        if (ActionTipUI.Instance != null && activeTipIDs != null)
            ActionTipUI.Instance.RestoreTips(activeTipIDs);
    }
}
