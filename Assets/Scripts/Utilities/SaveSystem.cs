using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerSaveData
{
    public CharacterStats stats;
    public Vector3 position;
    public string sceneName;
}

public static class SaveSystemJSON
{
    private static string GetSavePath(int slot = 1)
    {
        return Path.Combine(Application.persistentDataPath, $"save{slot}.json");
    }

    // 保存玩家数据
    public static void SavePlayer(Character player, int slot = 1)
    {
        if (player == null)
        {
            Debug.LogError("Save failed: player is null!");
            return;
        }

        PlayerSaveData data = new PlayerSaveData()
        {
            stats = player.stats,
            position = player.transform.position,
            sceneName = SceneManager.GetActiveScene().name
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);
        string path = Application.persistentDataPath + $"/save{slot}.json";
        Debug.Log($"存档已保存到 {path}: 场景={data.sceneName}, 坐标=({data.position.x:F2},{data.position.y:F2},{data.position.z:F2})");
    }

    // 加载玩家数据
    public static void LoadPlayer(Character player, int slot = 1)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning("没有可用存档！");
            return;
        }

        string json = File.ReadAllText(path);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        if (player == null)
        {
            Debug.LogError("Load failed: player is null!");
            return;
        }

        // 安全赋值，逐字段更新
        player.stats.maxHealth = data.stats.maxHealth;
        player.stats.currentHealth = data.stats.currentHealth;
        player.stats.attack = data.stats.attack;
        player.stats.defense = data.stats.defense;
        player.stats.moveSpeed = data.stats.moveSpeed;
        player.stats.jumpForce = data.stats.jumpForce;

        // 设置玩家坐标
        player.transform.position = data.position;

        Debug.Log($"存档读取成功: 场景={data.sceneName}, 坐标=({data.position.x:F2},{data.position.y:F2},{data.position.z:F2})");
    }


    // 获取存档场景名，主菜单读档时使用
    public static string GetSavedScene(int slot = 1)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path)) return "GameScene";
        string json = File.ReadAllText(path);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        return data.sceneName;
    }

    // 是否存在存档
    public static bool HasSave(int slot = 1)
    {
        return File.Exists(GetSavePath(slot));
    }

    // 删除存档
    public static void ClearSave(int slot = 1)
    {
        string path = GetSavePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"存档 save{slot}.json 已删除");
        }
    }
}

//示例代码
//SaveSystemJSON.SavePlayer(playerCharacter, slot: 2);