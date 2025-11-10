using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("基础属性")]
    public int baseDamage = 10;         // 攻击基础伤害

    [Header("攻击者")]
    public Character owner;             // 攻击所属角色（可为空）

    [Header("攻击冷却管理器")]
    public PlayerAttackManager attackManager;  // Inspector 手动指定

    // 已命中的目标，防止同一段攻击重复伤害
    private HashSet<Character> hitTargets = new HashSet<Character>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ResetHitTargets();    //可能导致bug：在Collider反复进出的时候会多次造成伤害
        TryAttack(collision);
    }
    
    private void TryAttack(Collider2D collision)
    {
        if (attackManager == null)
            return;

        if (!attackManager.CanAttack())
            return;

        Character target = collision.GetComponent<Character>();
        if (target == null || target == owner) return;
        if (!owner.IsHostileTo(target)) return;
        if (hitTargets.Contains(target)) return;

        hitTargets.Add(target);

        float finalDamage = baseDamage + (owner != null ? owner.stats.attack : 0);
        target.TakeDamage(finalDamage, owner);

        attackManager.TriggerCooldown();

        Debug.Log($"{owner.gameObject.name} hit {target.gameObject.name} for {finalDamage}");
    }

    // 可在每段攻击动画结束或销毁时清空已命中目标
    public void ResetHitTargets()
    {
        Debug.Log("HitTargets Reset！");
        hitTargets.Clear();
    }
}
