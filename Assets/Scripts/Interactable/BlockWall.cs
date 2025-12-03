using UnityEngine;

public class BlockWall : MonoBehaviour
{
    [Header("持久化设置")]
    [Tooltip("必须唯一")]
    public string wallID;

    private void Start()
    {
        //游戏开始时，检查自己是否在“已触发列表”里
        if (!string.IsNullOrEmpty(wallID) && PlayerManager.Instance.IsObjectTriggered(wallID))
        {
            gameObject.SetActive(false); //如果之前解开了，直接隐藏
        }
    }

    //供外部调用
    public void Unlock()
    {
        //记录到全局状态（存档用）
        if (!string.IsNullOrEmpty(wallID))
        {
            PlayerManager.Instance.TriggerObject(wallID);
        }

        //隐藏自己
        gameObject.SetActive(false);
        Debug.Log($"空气墙 {wallID} 已解锁！");
    }
}