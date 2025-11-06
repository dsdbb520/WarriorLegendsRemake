using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class TaskNPC : MonoBehaviour, IInteractable
{
    [Header("NPC 基本信息")]
    public string npcName = "村民";
    [TextArea(2, 5)] public string[] dialogueLines; // 对话内容

    [Header("任务相关")]
    public GameObject taskPopupPrefab; // 任务弹窗Prefab
    private bool hasGivenTask = false;

    [Header("交互提示")]
    public GameObject indicatorPrefab;
    private GameObject indicatorInstance;

    private bool isTalking = false;

    private void Start()
    {
        // 生成提示符
        if (indicatorPrefab != null)
        {
            indicatorInstance = Instantiate(indicatorPrefab, transform);
            indicatorInstance.transform.localPosition = Vector3.up * 1.5f;
            indicatorInstance.SetActive(true);
        }
    }

    public void Interact()
    {
        if (isTalking || hasGivenTask) return;

        isTalking = true;
        PlayerActionManager.Instance.DisableAll();

        // 调用 DialogueManager 来播放对话
        DialogueManager.Instance.StartDialogue(npcName, dialogueLines);

        // 启动协程检测对话结束
        StartCoroutine(WaitForDialogueThenShowTask());
    }

    private IEnumerator WaitForDialogueThenShowTask()
    {
        // 等待对话面板关闭
        yield return new WaitUntil(() => DialogueManager.Instance.dialoguePanel.activeSelf == false);

        // 对话结束 → 弹出任务窗口
        ShowTaskPopup();

        isTalking = false;
    }

    private void ShowTaskPopup()
    {
        if (taskPopupPrefab == null) return;

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        var popup = Instantiate(taskPopupPrefab, canvas.transform).GetComponent<TaskPopup>();
        popup.Setup("帮我收集10个蘑菇吧！", OnAccept, OnReject);
        popup.Show();
    }

    private void OnAccept()
    {
        Debug.Log("玩家接受了任务");
        hasGivenTask = true;
        PlayerActionManager.Instance.EnableAll();

        // 移除提示符
        if (indicatorInstance != null)
            indicatorInstance.SetActive(false);
    }

    private void OnReject()
    {
        Debug.Log("玩家拒绝了任务");
        PlayerActionManager.Instance.EnableAll();
    }
}
