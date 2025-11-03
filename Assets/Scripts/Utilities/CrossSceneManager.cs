using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossSceneManager : MonoBehaviour
{
    public static CrossSceneManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保留整个管理器及其子对象
        }
        else
        {
            Destroy(gameObject); // 已经有一个管理器，销毁重复的
            return;
        }
    }
}
