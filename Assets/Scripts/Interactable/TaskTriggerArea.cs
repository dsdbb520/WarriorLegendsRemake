using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TaskTriggerArea : MonoBehaviour
{
    [Header("任务目标设置")]
    public string targetID;

    [Tooltip("触发一次增加的进度")]
    public int amount = 1;

    [Header("设置")]
    public bool oneTimeOnly = true; //是否是一次性的

    private void OnTriggerEnter2D(Collider2D other)
    {
        //只有玩家能触发
        if (other.CompareTag("Player"))
        {
            if (TaskManager.Instance != null)
            {
                TaskManager.Instance.UpdateTaskProgress(targetID, amount);
                Debug.Log($"触发地点任务：{targetID}");
            }

            if (oneTimeOnly)
            {
                gameObject.SetActive(false); 
            }
        }
    }
}