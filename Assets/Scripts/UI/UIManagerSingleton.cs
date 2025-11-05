using UnityEngine;

public class UIManagerSingleton : MonoBehaviour
{
    public static UIManagerSingleton Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // 如果已有实例，销毁重复的
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 保持跨场景
    }
}
