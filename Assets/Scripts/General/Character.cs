using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("属性")]
    public float maxHealth;
    public float currectHealth;
    private float beforeHealth;

    [Header("详细参数")]
    public float noDamageTime;
    private float noDamageCounter;
    public bool noDamage;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent<Transform> Dead;
    private void Start()
    {
        currectHealth = maxHealth;
        beforeHealth = currectHealth;
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        if (noDamage)
        {
            noDamageCounter -= Time.deltaTime;
            if(noDamageCounter <= 0)noDamage = false;
        }
        if (beforeHealth != currectHealth)
        {
            OnHealthChange?.Invoke(this);
            beforeHealth = currectHealth;
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (noDamage) return;
        if (currectHealth - attacker.damage > 0)
        {
            currectHealth -= attacker.damage;
            TriggerNoDamage();
            //受伤
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currectHealth = 0;
            //死亡
            OnTakeDamage?.Invoke(attacker.transform);
            Dead?.Invoke(attacker.transform);
        }
        
    }
    private void TriggerNoDamage()
    {
        if (!noDamage)
        {
            noDamage = true;
            noDamageCounter = noDamageTime;
        }
    }
}
