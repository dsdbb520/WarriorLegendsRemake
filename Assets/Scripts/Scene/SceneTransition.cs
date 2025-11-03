using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;
    public Vector2 targetPosition; // 玩家在新场景的目标位置

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 保存玩家目标位置到 PlayerManager
            PlayerManager.Instance.SetPosition(targetPosition);

            // 切换场景
            SceneManager.LoadScene(targetScene);
        }
    }
}
