using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public float attackCooldown = 0.5f;
    private float attackTimer = 0f;

    private void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }

    public bool CanAttack() => attackTimer <= 0;

    public void TriggerCooldown() => attackTimer = attackCooldown;
}
