using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    [Tooltip("默认出生点名称，如果没有指定其他名称，就使用这个")]
    public string defaultSpawnName = "SpawnPoint";

    [Tooltip("可以在切换场景时指定要去的出生点名称")]
    public string spawnNameToUse = "";

    private void Start()
    {
        // 获取 PlayerManager 实例
        PlayerManager playerManager = PlayerManager.Instance;

        // 检查 PlayerManager 是否为空
        if (playerManager == null)
        {
            Debug.LogWarning("SceneInitializer: PlayerManager.Instance 为 null，无法设置玩家位置。");
            return;
        }

        // 获取玩家的 GameObject
        GameObject playerObj = playerManager.gameObject;

        // 判断是否有存档
        if (SaveSystemJSON.HasSave())
        {
            // 获取存档的场景名称
            string savedScene = SaveSystemJSON.GetSavedScene();

            // 如果当前场景与存档中的场景不匹配，则不加载存档位置，而是使用默认出生点
            if (savedScene != SceneManager.GetActiveScene().name)
            {
                SetPlayerToSpawnPoint(playerManager);
            }
            else
            {
                // 如果当前场景与存档的场景匹配，读取存档并传送到存档中的位置
                SaveSystemJSON.LoadPlayer(playerManager.GetComponent<Character>());
                Debug.Log($"SceneInitializer: 玩家已传送至存档位置 {playerObj.transform.position}");
            }
        }
        else
        {
            // 没有存档，则使用默认出生点或指定的 spawnNameToUse
            SetPlayerToSpawnPoint(playerManager);
        }
    }

    private void SetPlayerToSpawnPoint(PlayerManager playerManager)
    {
        GameObject playerObj = playerManager.gameObject;

        // 判断是否指定了出生点
        string targetSpawnName = string.IsNullOrEmpty(spawnNameToUse) ? defaultSpawnName : spawnNameToUse;

        // 查找出生点物体
        GameObject spawnPoint = GameObject.Find(targetSpawnName);
        if (spawnPoint != null)
        {
            // 如果找到了出生点，则设置玩家位置
            playerObj.transform.position = spawnPoint.transform.position;
            Debug.Log($"SceneInitializer: 玩家使用出生点 {targetSpawnName}");
        }
        else
        {
            // 如果没有找到出生点，则保持玩家当前位置
            Debug.LogWarning($"SceneInitializer: 未找到名为 {targetSpawnName} 的物体，玩家将保持当前位置。");
        }
    }
}
