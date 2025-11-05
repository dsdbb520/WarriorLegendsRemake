using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("按钮")]
    public Button newGameButton;
    public Button continueButton;

    [Header("玩家预制体")]
    public GameObject playerPrefab;

    [Header("起始场景")]
    public string startingScene = "TutorialMap"; // 新游戏默认场景

    private void Start()
    {
        // 按钮事件绑定
        newGameButton.onClick.AddListener(StartNewGame);
        continueButton.onClick.AddListener(ContinueGame);

        // 控制Continue按钮是否可用
        continueButton.interactable = SaveSystemJSON.HasSave();
    }

    #region 新游戏
    private void StartNewGame()
    {
        // 清除旧存档
        SaveSystemJSON.ClearSave();

        // 加载起始场景
        SceneManager.LoadScene(startingScene);

        // 等场景加载完成后生成玩家
        SceneManager.sceneLoaded += OnSceneLoadedNewGame;
    }

    private void OnSceneLoadedNewGame(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedNewGame;

        // 生成玩家
        GameObject playerObj = Instantiate(playerPrefab);
        Character player = playerObj.GetComponent<Character>();
        if (player == null)
        {
            Debug.LogError("Player prefab 缺少 Character 组件！");
            return;
        }

        // 玩家属性是默认的，不需要读取存档
        Debug.Log("新游戏开始，玩家已生成");
    }
    #endregion

    #region 继续游戏
    private void ContinueGame()
    {
        if (!SaveSystemJSON.HasSave())
        {
            Debug.LogWarning("没有存档可继续游戏！");
            return;
        }

        // 获取存档的场景名
        string savedScene = SaveSystemJSON.GetSavedScene();
        SceneManager.LoadScene(savedScene);

        // 场景加载完成后再生成玩家并读取存档
        SceneManager.sceneLoaded += OnSceneLoadedContinueGame;
    }

    private void OnSceneLoadedContinueGame(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedContinueGame;

        // 尝试找到已有的 Player
        Character player = GameObject.FindWithTag("Player")?.GetComponent<Character>();

        if (player == null)
        {
            // 场景里没有 Player，则生成一个
            if (playerPrefab == null)
            {
                Debug.LogError("playerPrefab 未设置，无法生成 Player！");
                return;
            }

            GameObject playerObj = Instantiate(playerPrefab);
            player = playerObj.GetComponent<Character>();

            if (player == null)
            {
                Debug.LogError("生成的 Player prefab 缺少 Character 组件！");
                return;
            }
        }

        // 读取存档并应用属性和位置
        SaveSystemJSON.LoadPlayer(player);

        // 计算当前血量百分比
        float healthPercent = player.currentHealth / player.maxHealth;

        // 让 UI 立即同步
        player.playStatBar.healthImage.fillAmount = healthPercent;
        player.playStatBar.healthSlowImage.fillAmount = healthPercent;

        // 通知摄像机绑定Player
        CameraAutoFollow cam = FindObjectOfType<CameraAutoFollow>();
        if (cam != null)
            cam.BindToPlayer(player.gameObject);

        Debug.Log("继续游戏：玩家已加载并准备就绪");
    }

    #endregion


}
