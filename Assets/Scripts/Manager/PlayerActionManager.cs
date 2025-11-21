using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerActionManager : MonoBehaviour
{
    public static PlayerActionManager Instance;

    [Header("操作权限")]
    public bool canMove = true;
    public bool canJump = true;
    public bool canAttack = true;
    public bool canInteract = true;
    public bool canTask = true;
    public bool canBackpack = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 基本操作控制

    public void DisableAll()
    {
        canMove = false;
        canJump = false;
        canAttack = false;
        canInteract = false;
        canTask = false;
        canBackpack = false;
    }

    public void EnableAll()
    {
        canMove = true;
        canJump = true;
        canAttack = true;
        canInteract = true;
        canTask = true;
        canBackpack = true;
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
            case "task":
                canTask = enable;
                break;
            case "backpack":
                canBackpack = enable;
                break;
            default:
                Debug.LogWarning("未识别的操作: " + actionName);
                break;
        }
    }

    #endregion

    #region 带时间的操作禁用

    private Dictionary<string, Coroutine> tempDisableCoroutines = new Dictionary<string, Coroutine>();

    public void DisableActionTemporary(string actionName, float duration)
    {
        actionName = actionName.ToLower();

        if (tempDisableCoroutines.ContainsKey(actionName) && tempDisableCoroutines[actionName] != null)
        {
            StopCoroutine(tempDisableCoroutines[actionName]);
            tempDisableCoroutines.Remove(actionName);
        }

        SetAction(actionName, false);

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

    #region 只保留指定操作

    /// <summary>
    /// 只保留指定操作，将其他操作全部禁用。
    /// </summary>
    /// <param name="actionName">要保留的操作名称</param>
    public void EnableOnlyAction(string actionName)
    {
        DisableAll();
        SetAction(actionName, true);
    }

    #endregion
}
