using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    [Tooltip("默认出生点名称，如果没有指定其他名称，就使用这个")]
    public string defaultSpawnName = "SpawnPoint";

    [Tooltip("可以在切换场景时指定要去的出生点名称")]
    public string spawnNameToUse = "";

    private void Start()
    {
        if (PlayerManager.Instance != null)
        {
            // 优先使用 spawnNameToUse，如果为空则使用 defaultSpawnName
            string targetSpawnName = string.IsNullOrEmpty(spawnNameToUse) ? defaultSpawnName : spawnNameToUse;

            GameObject spawnPoint = GameObject.Find(targetSpawnName);
            if (spawnPoint != null)
            {
                PlayerManager.Instance.transform.position = spawnPoint.transform.position;
            }
            else
            {
                Debug.LogWarning($"SceneInitializer: 未找到名为 {targetSpawnName} 的物体，玩家将保持当前位置。");
            }
        }
    }
}
