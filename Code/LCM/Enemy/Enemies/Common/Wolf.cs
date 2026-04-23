using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class Wolf : CounterableEnemy
{
    [SerializeField] private float _dashPower;

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("wolf.dash", EnemyStateType.Attack, ExecuteDashAttack);
    }

    private void ExecuteDashAttack()
    {
        AddForceToEntity(Vector2.up * 2f);

        Vector2 moveDirection = GetMovementDirection();
        AddForceToEntity(moveDirection * _dashPower);

        AudioManager.Instance.PlaySound2D("WolfAttack", 0.5f, false, SoundType.SfX);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        AudioManager.Instance.PlaySound2D("WolfDead", 0.5f, false, SoundType.SfX);
    }
}
}







