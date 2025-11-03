using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        Debug.Log("当前进入追击状态");
        currentEnemy.currectSpeed = currentEnemy.chaseSpeed;
        currentEnemy.animator.SetBool("isRun",true);
        currentEnemy.wait = false;
        currentEnemy.waitTimeCounter = currentEnemy.waitTime;   //强制停止等待，直接加速开始追击
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        if ((!currentEnemy.physicsCheck.isGround) || (currentEnemy.physicsCheck.touchedLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchedRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.Turn();
        }
    }
    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        Debug.Log("退出追击状态");
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.animator.SetBool("isRun", false);
    }
}
