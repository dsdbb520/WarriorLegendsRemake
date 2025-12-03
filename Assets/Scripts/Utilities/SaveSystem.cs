using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 1. 定义总存档结构
[System.Serializable]
public class GameSaveData
{
    public string sceneName;
    public CharacterStats playerStats;
    public Vector3 playerPosition;
    public List<string> worldObjectIDs; // 世界交互状态
    public List<string> activeTaskIDs;  // 任务状态
}

public static class SaveSystemJSON
{
    public static bool IsLoadingFromSave = false;
    private static string GetSavePath(int slot = 1)
    {
        return Path.Combine(Application.persistentDataPath, $"save{slot}.json");
    }

    //保存游戏
    public static void SaveGame(int slot = 1)
    {
        //获取各个单例中的数据
        var player = GameObject.FindWithTag("Player")?.GetComponent<Character>();
        var playerManager = PlayerManager.Instance;
        var taskManager = TaskManager.Instance;

        if (player == null) { Debug.LogError("保存失败：找不到玩家！"); return; }

        //构建存档数据
        GameSaveData data = new GameSaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerStats = player.stats,
            playerPosition = player.transform.position,
            //获取列表
            worldObjectIDs = playerManager != null ? playerManager.triggeredObjectIDs : new List<string>(),
            activeTaskIDs = taskManager != null ? taskManager.GetTaskIDs() : new List<string>()
        };

        //写入文件
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);
        Debug.Log($"游戏保存成功！Slot: {slot}");
    }

    //加载游戏
    public static void LoadGame(int slot = 1)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        //恢复玩家数据
        var player = GameObject.FindWithTag("Player")?.GetComponent<Character>();
        if (player != null)
        {
            player.stats = data.playerStats;
            player.transform.position = data.playerPosition;
            //刷新UI
            if (player.playStatBar != null) player.OnHealthChange?.Invoke(player);
        }

        //恢复世界状态 (把读取的列表塞回PlayerManager)
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.triggeredObjectIDs = data.worldObjectIDs;
        }

        //恢复任务状态
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.LoadTaskIDs(data.activeTaskIDs);
        }

        Debug.Log("游戏读取成功！");
    }

    public static bool HasSave(int slot = 1) => File.Exists(GetSavePath(slot));

    public static string GetSavedScene(int slot = 1)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path)) return null;
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GameSaveData>(json).sceneName;
    }

    public static void ClearSave(int slot = 1)
    {
        if (HasSave(slot)) File.Delete(GetSavePath(slot));
    }
}