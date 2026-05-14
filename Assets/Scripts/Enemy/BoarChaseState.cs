using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currectSpeed = currentEnemy.chaseSpeed;
        currentEnemy.animator.SetBool("isRun", true);
        currentEnemy.waitTimeCounter = -100;  // force stop patrol wait immediately
        currentEnemy.wait = false;

        // BUG FIX: reset the lost-timer every time chase begins,
        // so the enemy doesn't immediately exit on first detection.
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
            currentEnemy.SwitchState(NPCState.Patrol);

        bool hitWall = (currentEnemy.physicsCheck.touchedLeftWall && currentEnemy.faceDir.x < 0)
                    || (currentEnemy.physicsCheck.touchedRightWall && currentEnemy.faceDir.x > 0);

        if (!currentEnemy.physicsCheck.isGround || hitWall)
            currentEnemy.Turn();
    }

    public override void PhysicsUpdate() { }

    public override void OnExit()
    {
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.animator.SetBool("isRun", false);
    }
}
