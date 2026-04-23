using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public class KnightLocomotionController
{
    private readonly Knight _owner;
    private readonly KnightShieldController _shieldController;
    private readonly KnightMovementTuning _movementTuning;

    public KnightLocomotionController(
        Knight owner,
        KnightShieldController shieldController,
        KnightMovementTuning movementTuning
    )
    {
        _owner = owner;
        _shieldController = shieldController;
        _movementTuning = movementTuning;
    }

    public void TickMove()
    {
        if (!CanTickMovement())
            return;

        if (!TryPrepareTarget())
            return;

        if (_owner.TryTransitionToAttack())
            return;

        if (ShouldEnterRunState())
        {
            _owner.TransitionState(EnemyStateType.Run);
            return;
        }

        Move(_owner.EnemyData != null ? _owner.EnemyData.movementSpeed : 0f);
        _shieldController.TryTakeShield();
    }

    public void TickRun()
    {
        if (!CanTickMovement())
            return;

        if (!TryPrepareTarget())
            return;

        if (_owner.TryTransitionToAttack())
            return;

        if (ShouldEnterMoveState())
        {
            _owner.TransitionState(EnemyStateType.Move);
            return;
        }

        float moveSpeed = _owner.EnemyData != null ? _owner.EnemyData.movementSpeed : 0f;
        Move(moveSpeed + _movementTuning.runSpeedBonus);
        _shieldController.TryTakeShield();
    }

    private bool CanTickMovement()
    {
        return _owner.CanMove;
    }

    private bool TryPrepareTarget()
    {
        _owner.TargetingPlayer();
        if (_owner.TargetTrm != null)
            return true;

        _owner.TransitionState(EnemyStateType.Idle);
        return false;
    }

    private bool ShouldEnterRunState()
    {
        if (_owner.isShield)
            return false;

        Vector2 toTarget = _owner.TargetTrm.position - _owner.transform.position;
        float runEnterDistance = _movementTuning.runEnterDistance;
        return toTarget.sqrMagnitude > runEnterDistance * runEnterDistance;
    }

    private bool ShouldEnterMoveState()
    {
        if (_owner.isShield)
            return false;

        Vector2 toTarget = _owner.TargetTrm.position - _owner.transform.position;
        float runExitDistance = _movementTuning.runExitDistance;
        return toTarget.sqrMagnitude <= runExitDistance * runExitDistance;
    }

    private void Move(float speed)
    {
        Vector2 moveDirection = _owner.GetMovementDirection();
        if (moveDirection.sqrMagnitude <= 0.0001f)
        {
            _owner.StopImmediately(false);
            return;
        }

        moveDirection.Normalize();
        _owner.EnemyRotation();

        if (_owner.RbCompo != null)
            _owner.RbCompo.linearVelocityX = moveDirection.x * speed;
    }
}
}







