using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        Debug.Log("进入巡逻模式");
        currentEnemy = enemy;
        currentEnemy.currectSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        //发现Player后进入Chase，Player脱离视线范围回到Patrol
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        if ((!currentEnemy.physicsCheck.isGround) || (currentEnemy.physicsCheck.touchedLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchedRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.Turn();
            currentEnemy.wait = true;
            currentEnemy.animator.SetBool("isWalk", currentEnemy.wait);
        }
        else
        {
           currentEnemy.animator.SetBool("isWalk", !currentEnemy.wait);
        }
    }
    public override void PhysicsUpdate()
    {
        
    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("isWalk", false);
        Debug.Log("退出PatrolState");
    }


}
