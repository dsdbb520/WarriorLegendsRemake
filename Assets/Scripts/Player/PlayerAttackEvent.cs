using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackEvents : MonoBehaviour
{
    [Header("攻击Collider列表")]
    public List<Collider2D> attackColliders = new List<Collider2D>();
    [Header("Attack脚本列表")]
    public List<Attack> attackScripts = new List<Attack>();


    /// 动画事件调用，开启指定段的攻击
    public void EnableAttack(int index)
    {
        if (index < 0 || index >= attackColliders.Count) return;

        attackColliders[index].enabled = true;

    }


    /// 动画事件调用，关闭指定段的攻击
    public void DisableAttack(int index)
    {
        if (index < 0 || index >= attackColliders.Count) return;

        attackColliders[index].enabled = false;
    }

    public void PlayAttack1Sound() => AudioManager.Instance.PlayCharacterSound("attack1");
    public void PlayAttack2Sound() => AudioManager.Instance.PlayCharacterSound("attack2");

}
