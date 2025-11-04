using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 管理玩家操作权限，可以控制移动、跳跃、攻击、互动等。
// 支持短时间禁用操作


public class PlayerActionManager : MonoBehaviour
{

    public static PlayerActionManager Instance;

    [Header("操作权限")]
    public bool canMove = true;
    public bool canJump = true;
    public bool canAttack = true;
    public bool canInteract = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 不跨场景保留，因为同一物体已经有跨场景脚本
        }
        else
        {
            Destroy(gameObject); // 避免重复实例
        }
    }

    #region 基本操作控制

    public void DisableAll()
    {
        canMove = false;
        canJump = false;
        canAttack = false;
        canInteract = false;
    }

    public void EnableAll()
    {
        canMove = true;
        canJump = true;
        canAttack = true;
        canInteract = true;
    }

    public void SetAction(string actionName, bool enable)
    {
        switch (actionName.ToLower())
        {
            case "move":
                canMove = enable;
                break;
            case "jump":
                canJump = enable;
                break;
            case "attack":
                canAttack = enable;
                break;
            case "interact":
                canInteract = enable;
                break;
            default:
                Debug.LogWarning("未识别的操作: " + actionName);
                break;
        }
    }

    #endregion

    #region 带时间的操作禁用

    private Dictionary<string, Coroutine> tempDisableCoroutines = new Dictionary<string, Coroutine>();

    //
    /// 禁用指定操作一段时间，时间结束后自动恢复
    /// <param name="actionName">操作名：move/jump/attack/interact</param>
    /// <param name="duration">禁用时长，单位秒</param>
    public void DisableActionTemporary(string actionName, float duration)
    {
        actionName = actionName.ToLower();

        // 如果当前已经有协程在控制这个操作，先停止
        if (tempDisableCoroutines.ContainsKey(actionName) && tempDisableCoroutines[actionName] != null)
        {
            StopCoroutine(tempDisableCoroutines[actionName]);
            tempDisableCoroutines.Remove(actionName);
        }

        // 禁用操作
        SetAction(actionName, false);

        // 开始计时恢复
        Coroutine c = StartCoroutine(ReenableActionAfterTime(actionName, duration));
        tempDisableCoroutines[actionName] = c;
    }

    private IEnumerator ReenableActionAfterTime(string actionName, float duration)
    {
        yield return new WaitForSeconds(duration);
        SetAction(actionName, true);
        tempDisableCoroutines.Remove(actionName);
    }

    #endregion
}

//示例代码
//PlayerActionManager.Instance.DisableActionTemporary("move", 3f);
//PlayerActionManager.Instance.DisableActionTemporary("attack", 5f);
//PlayerActionManager.Instance.SetAction("jump", false);
//PlayerActionManager.Instance.EnableAll();

