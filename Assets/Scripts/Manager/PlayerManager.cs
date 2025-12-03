using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // 已经有实例了，销毁多余的
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 切换场景不销毁
    }

    [Header("运行时世界状态")]
    //记录所有已经被触发的一次性物体ID（比如已开的宝箱、已解锁的门）
    public List<string> triggeredObjectIDs = new List<string>();

    public bool IsObjectTriggered(string id)
    {
        return triggeredObjectIDs.Contains(id);
    }

    public void TriggerObject(string id)
    {
        if (!triggeredObjectIDs.Contains(id))
        {
            triggeredObjectIDs.Add(id);
        }
    }

    // 保存位置到切换场景时使用
    public Vector2 lastPosition;

    // 设置玩家位置
    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
        lastPosition = pos;
    }
}
