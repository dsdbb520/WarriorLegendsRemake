using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using System.Collections.Generic;

public class GameIntroManager : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject blackBars; //黑边
    public CinemachineVirtualCamera vcamCloseup; //特写相机
    public string selfDialogueID = "PlayerSelf";
    public Transform playerTransform;
    public GameObject playerStstusBar;
    public GameObject TriggerTutorial;

    public void StartGameSequence()
    {
        vcamCloseup.Priority = 0;
        blackBars.SetActive(false);
        director.Play();
    }

    private void Start()
    {
        if (SaveSystemJSON.IsLoadingFromSave)
        {
            if (vcamCloseup != null) vcamCloseup.Priority = 0; //确保优先级最低
            if (blackBars != null) blackBars.SetActive(false);
            if (director != null) director.Stop();

            if (PlayerActionManager.Instance != null) PlayerActionManager.Instance.EnableAll();
            if (playerStstusBar != null) playerStstusBar.SetActive(true);

            Destroy(gameObject); // 销毁自己，任务完成
            return;
        }
        StartGameSequence();
    }

    public void TriggerDialogue()
    {
        DialogueEntryCSV data = DialogueLoader.Instance.GetDialogueCSV("PlayerSelf", "0");
        //启动对话
        DialogueManager.Instance.StartDialogue(selfDialogueID, data.dialogueLines, OnDialogueFinished);
    }

    //对话结束
    void OnDialogueFinished()
    {
        blackBars.SetActive(false);
        if (playerTransform != null)
        {
            Vector3 finalPos = playerTransform.position;
            Quaternion finalRot = playerTransform.rotation;
            director.Stop();

            playerTransform.position = finalPos;
            playerTransform.rotation = finalRot;
        }
        else
        {
            director.Stop();
        }

        vcamCloseup.Priority = 0;
        if (PlayerActionManager.Instance != null)
            PlayerActionManager.Instance.EnableAll();
        playerStstusBar.SetActive(true);
        TriggerTutorial.SetActive(true);
        Destroy(gameObject);
    }
}