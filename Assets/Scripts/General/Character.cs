using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum Faction { Player, Enemy, Neutral }

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

    [Header("角色属性")]
    public CharacterStats stats = new CharacterStats();
    public PlayStatBar playStatBar;

    [Header("Shader 效果")]
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;
    private int flashAmountID;

    public float maxHealth => stats.maxHealth;
    public float currentHealth => stats.currentHealth;

    private float beforeHealth;

    [Header("无敌帧")]
    public float noDamageTime = 0.5f;
    private float noDamageCounter;
    public bool noDamage;

    [Header("事件")]
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent<Transform> Dead;

    private Coroutine _flashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
        flashAmountID = Shader.PropertyToID("_FlashAmount");
    }

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
            if (noDamageCounter <= 0)
                noDamage = false;
        }

        if (beforeHealth != stats.currentHealth)
        {
            OnHealthChange?.Invoke(this);
            beforeHealth = stats.currentHealth;
        }
    }

    public void TakeDamage(float damage, Character attacker)
    {
        if (noDamage) return;

        float damageTaken = Mathf.Max(damage - stats.defense, 0);
        stats.currentHealth -= damageTaken;

        if (stats.currentHealth <= 0)
        {
            stats.currentHealth = 0;
            Dead?.Invoke(attacker ? attacker.transform : null);
        }

        TriggerNoDamage();
        OnTakeDamage?.Invoke(attacker ? attacker.transform : null);

        // Stop previous flash before starting a new one
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        while (noDamage)
        {
            SetFlash(1f);
            yield return new WaitForSeconds(0.2f);
            SetFlash(0f);
            yield return new WaitForSeconds(0.2f);
        }
        SetFlash(0f);
        _flashCoroutine = null;
    }

    private void SetFlash(float value)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat(flashAmountID, value);
        spriteRenderer.SetPropertyBlock(propBlock);
    }

    public void Heal(float amount)
    {
        stats.currentHealth = Mathf.Min(stats.currentHealth + amount, stats.maxHealth);
        OnHealthChange?.Invoke(this);
    }

    public void TriggerNoDamage()
    {
        noDamage = true;
        noDamageCounter = noDamageTime;
    }

    public bool IsHostileTo(Character other)
    {
        if (other == null) return false;
        return faction != other.faction;
    }
}
