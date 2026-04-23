using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class DrawSlime : CounterableEnemy
{
    [SerializeField] private float _attackDashPower;
    [SerializeField] private float _jumpAssistPower = 2f;

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("drawslime.dash", EnemyStateType.Attack, ExecuteDashAttack);
    }

    protected override bool CanFinishAttackState(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack)
            return isAttackAnimationEnd && GroundCheck();

        return base.CanFinishAttackState(attackType);
    }

    private void ExecuteDashAttack()
    {
        Vector2 direction = GetMovementDirection();
        if (direction.sqrMagnitude > 0.0001f)
        {
            direction.Normalize();
            AddForceToEntity(direction * _attackDashPower);
        }

        AddForceToEntity(Vector2.up * _jumpAssistPower);
        AudioManager.Instance.PlaySound2D("DrawSlimeAttack", 0f, false, SoundType.SfX);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        AudioManager.Instance.PlaySound2D("DrawSlimeDead", 0f, false, SoundType.SfX);
    }
}
}







