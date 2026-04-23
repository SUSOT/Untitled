using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class TallSlime : CounterableEnemy
{
    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("tallslime.attack.a", EnemyStateType.Attack, ExecuteAttackA);
        registry.Add("tallslime.attack.b", EnemyStateType.Attack2, ExecuteAttackB);
    }

    protected override void OnKnockBack(Vector2 knockBackForce)
    {
        KnockBack(knockBackForce / 2f, 0.5f);
    }

    private void ExecuteAttackA()
    {
        LockMass();
        AudioManager.Instance.PlaySound2D("TallSlimeAttack", 0f, false, SoundType.SfX);
    }

    private void ExecuteAttackB()
    {
        LockMass();
        AudioManager.Instance.PlaySound2D("TallSlimeAttack2", 0f, false, SoundType.SfX);
    }

    protected override void OnAttackAnimationCompleted(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack || attackType == EnemyStateType.Attack2)
            UnlockMass();

        base.OnAttackAnimationCompleted(attackType);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        UnlockMass();
        AudioManager.Instance.PlaySound2D("TallSlimeDead", 0f, false, SoundType.SfX);
    }
}
}







