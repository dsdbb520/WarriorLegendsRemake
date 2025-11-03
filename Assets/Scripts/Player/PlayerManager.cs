using UnityEngine;

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

    // 保存位置到切换场景时使用
    public Vector2 lastPosition;

    // 设置玩家位置
    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
        lastPosition = pos;
    }
}
