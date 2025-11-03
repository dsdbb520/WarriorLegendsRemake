using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    private void Start()
    {
        // 场景开始时，把玩家放在上一次保存的位置
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.transform.position = PlayerManager.Instance.lastPosition;
        }
    }
}
