using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections;

public class CameraAutoFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        // 保证摄像机自己不会被销毁
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedFindPlayer());
    }

    private IEnumerator DelayedFindPlayer()
    {
        // 延迟 0.1 秒，确保玩家对象加载完毕
        yield return new WaitForSeconds(0.1f);
        FindAndFollowPlayer();
    }

    private void FindAndFollowPlayer()
    {
        // 找到那个从上个场景保留下来的玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("未找到带有 Player 标签的对象！（可能在另一个场景里）");
            return;
        }

        if (vcam == null)
        {
            Debug.LogWarning("未找到 CinemachineVirtualCamera 组件！");
            return;
        }

        vcam.Follow = player.transform;
        vcam.LookAt = player.transform;

        Debug.Log($"摄像机已自动跟随玩家：{player.name}");
    }
}
