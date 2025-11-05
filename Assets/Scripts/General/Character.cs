using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Faction
{
    Player,
    Enemy,
    Neutral
}

[System.Serializable]
public class CharacterStats
{
    public float maxHealth = 100;
    public float currentHealth;
    public float attack = 10;
    public float defense = 5;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
}

public class Character : MonoBehaviour
{
    [Header("阵营设置")]
    public Faction faction = Faction.Neutral;

    [Header("核心属性")]
    public CharacterStats stats = new CharacterStats();
    public PlayStatBar playStatBar;

    public float maxHealth => stats.maxHealth;    // 兼容旧UI
    public float currentHealth => stats.currentHealth;

    private float beforeHealth;

    [Header("受击参数")]
    public float noDamageTime = 0.5f;
    private float noDamageCounter;
    public bool noDamage;

    [Header("事件")]
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent<Transform> Dead;

    private void Start()
    {
        beforeHealth = stats.currentHealth;
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        if (noDamage)
        {
            noDamageCounter -= Time.deltaTime;
            Debug.Log($"[Update] {gameObject.name} noDamageCounter: {noDamageCounter}");
            if (noDamageCounter <= 0)
            {
                noDamage = false;
                Debug.Log($"[Update] {gameObject.name} noDamage ended.");
            }
        }

        if (beforeHealth != stats.currentHealth)
        {
            OnHealthChange?.Invoke(this);
            beforeHealth = stats.currentHealth;
        }
    }

    public void TakeDamage(float damage, Character attacker)
    {
        Debug.Log($"[TakeDamage] Called on {gameObject.name}. Current Health: {stats.currentHealth}, Damage: {damage}, NoDamage: {noDamage}");

        if (noDamage)
        {
            Debug.Log($"[TakeDamage] {gameObject.name} is in noDamage state, ignoring damage.");
            return;
        }

        // 计算实际伤害
        float damageTaken = Mathf.Max(damage - stats.defense, 0);
        stats.currentHealth -= damageTaken;
        Debug.Log($"[TakeDamage] {gameObject.name} took {damageTaken} damage. New Health: {stats.currentHealth}");

        // 只在首次受到伤害时触发无敌时间
        if (!noDamage)
        {
            TriggerNoDamage();
            Debug.Log($"[TakeDamage] {gameObject.name} triggered noDamage for {noDamageTime} seconds.");
        }

        // 触发受击事件
        OnTakeDamage?.Invoke(attacker ? attacker.transform : null);
        Debug.Log($"[TakeDamage] OnTakeDamage invoked by {attacker?.gameObject.name}");

        // 死亡处理
        if (stats.currentHealth <= 0)
        {
            stats.currentHealth = 0;
            Dead?.Invoke(attacker ? attacker.transform : null);
            Debug.Log($"[TakeDamage] {gameObject.name} died.");
        }
    }




    private void TriggerNoDamage()
    {
        noDamage = true;
        noDamageCounter = noDamageTime;
    }

    //判断是否敌对
    public bool IsHostileTo(Character other)
    {
        if (other == null) return false;
        return faction != other.faction;
    }
}
