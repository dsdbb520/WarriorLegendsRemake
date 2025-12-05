using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class TaskSaveData
{
    public string taskID;       // 任务ID
    public int currentCount;    // 当前杀怪数量
    public bool isFinished;     // 是否已完成
}

[System.Serializable]
public class GameSaveData
{
    // 场景信息
    public string sceneName;

    // 玩家数据
    public CharacterStats playerStats;
    public Vector3 playerPosition;

    // 世界交互状态 (宝箱、门等)
    public List<string> worldObjectIDs;

    // 任务状态 (包含进度)
    public List<TaskSaveData> taskDataList;
}

public static class SaveSystemJSON
{
    //是否正在从存档加载 (给SceneInitializer用)
    public static bool IsLoadingFromSave = false;

    private static string GetSavePath(int slot = 1)
    {
        return Path.Combine(Application.persistentDataPath, $"save{slot}.json");
    }

    //保存游戏
    public static void SaveGame(int slot = 1)
    {
        var player = GameObject.FindWithTag("Player")?.GetComponent<Character>();
        var playerManager = PlayerManager.Instance;
        var taskManager = TaskManager.Instance;

        if (player == null)
        {
            Debug.LogError("保存失败：找不到玩家物体！");
            return;
        }

        GameSaveData data = new GameSaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerStats = player.stats,
            playerPosition = player.transform.position,

            worldObjectIDs = playerManager != null ? playerManager.triggeredObjectIDs : new List<string>(),

            taskDataList = taskManager != null ? taskManager.GetTaskSaveData() : new List<TaskSaveData>()
        };

        //序列化并写入文件
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);

        Debug.Log($"游戏保存成功！Slot: {slot}\n任务数量: {data.taskDataList.Count}");

        //保存成功提示
        NotificationManager.Instance?.Show("游戏已保存");
    }

    public static void LoadGame(int slot = 1)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning("加载失败：找不到存档文件");
            return;
        }

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        //恢复玩家数据
        var player = GameObject.FindWithTag("Player")?.GetComponent<Character>();
        if (player != null)
        {
            player.stats = data.playerStats;
            player.transform.position = data.playerPosition;

            //刷新血条UI
            if (player.playStatBar != null)
                player.OnHealthChange?.Invoke(player);
        }

        //恢复世界状态 (宝箱/空气墙)
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.triggeredObjectIDs = data.worldObjectIDs;
        }

        //恢复任务状态 (包含进度)
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.LoadTaskSaveData(data.taskDataList);
        }

        Debug.Log("游戏读取成功！所有状态已恢复。");
    }


    //获取存档所在的场景名
    public static string GetSavedScene(int slot = 1)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path)) return null;

        try
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameSaveData>(json).sceneName;
        }
        catch
        {
            return null;
        }
    }

    //是否存在存档
    public static bool HasSave(int slot = 1)
    {
        return File.Exists(GetSavePath(slot));
    }

    //删除存档
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