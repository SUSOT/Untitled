using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.StateMachine.States
{
public class EnemyMoveState : EnemyState
{
    public EnemyMoveState(EnemyCore enemy) : base(enemy, EnemyStateType.Move)
    {
    }

    public override void FixedUpdate()
    {
        if (!Enemy.CanMove)
            return;

        Enemy.TargetingPlayer();

        if (Enemy.TargetTrm == null)
        {
            Enemy.TransitionState(EnemyStateType.Idle);
            return;
        }

        Vector2 moveDirection = Enemy.GetMovementDirection();
        if (moveDirection.sqrMagnitude <= 0.0001f)
        {
            Enemy.StopImmediately(false);
            return;
        }

        moveDirection.Normalize();

        Enemy.EnemyRotation();
        Enemy.RbCompo.linearVelocityX = moveDirection.x * Enemy.EnemyData.movementSpeed;

        Enemy.TryTransitionToAttack();
    }
}
}







