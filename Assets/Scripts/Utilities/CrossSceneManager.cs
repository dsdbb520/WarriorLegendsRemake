using UnityEngine;
using System.Collections.Generic;

public class CrossSceneManager : MonoBehaviour
{
    public static CrossSceneManager Instance;

    [Header("跨场景常驻对象")]
    public GameObject[] persistentPrefabs; // 你可以拖 UI、Player 等 prefab

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 生成并管理跨场景 prefab
            foreach (var prefab in persistentPrefabs)
            {
                if (prefab == null) continue;

                string key = prefab.name;
                GameObject existing = GameObject.Find(key);
                if (existing != null)
                {
                    spawnedPrefabs[key] = existing;
                    DontDestroyOnLoad(existing);
                    continue;
                }

                GameObject obj = Instantiate(prefab);
                obj.name = key; // 保证名字不带 "(Clone)"
                DontDestroyOnLoad(obj);
                spawnedPrefabs[key] = obj;
            }
        }
        else
        {
            Destroy(gameObject); // 已经有一个实例，销毁重复的
        }
    }

    // 可以在代码里获取跨场景对象
    public GameObject GetPersistentObject(string name)
    {
        if (spawnedPrefabs.ContainsKey(name))
            return spawnedPrefabs[name];
        return null;
    }
}
