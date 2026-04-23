using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine.States;
using _01.Script.LCM.Enemy.StateMachine;
using _01.Script.LCM.Misc;

namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class Marksman : EnemyCore
{
    [SerializeField] private Transform _firePos;
    [SerializeField] private PoolItemSO _arrow;

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("marksman.shot", EnemyStateType.Attack, null);
    }

    protected override void RegisterCustomStates(EnemyStateMachine machine)
    {
        base.RegisterCustomStates(machine);

        machine.Register(
            EnemyStateType.Spawn,
            new EnemyAnimationGateState(this, EnemyStateType.Spawn, EnemyStateType.Idle, isCanceled: () => IsDead, cancelFallbackState: EnemyStateType.Dead, timeoutSeconds: 5f, timeoutFallbackState: EnemyStateType.Idle)
        );
    }

    protected override void OnEnemyAwakeCompleted()
    {
        base.OnEnemyAwakeCompleted();

        if (AnimTriggerCompo != null)
            AnimTriggerCompo.OnAttackTrigger += FireArrow;
    }

    protected override void OnDestroy()
    {
        if (AnimTriggerCompo != null)
            AnimTriggerCompo.OnAttackTrigger -= FireArrow;

        base.OnDestroy();
    }

    protected override void OnKnockBack(Vector2 knockBackForce)
    {
        KnockBack(knockBackForce, 0.5f);
    }

    private void FireArrow()
    {
        if (TargetTrm == null)
            return;

        MarksmanBullet arrow = PoolManager.Instance.Pop(_arrow.poolName) as MarksmanBullet;
        if (arrow == null)
            return;

        arrow.transform.position = _firePos.position;
        arrow.ThrowObject(TargetTrm.position);

        AudioManager.Instance.PlaySound2D("MarksmanAttack", 0f, false, SoundType.SfX);
    }

    public void Spawn()
    {
        TransitionState(EnemyStateType.Spawn);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        AudioManager.Instance.PlaySound2D("MarksmanDead", 0f, false, SoundType.SfX);
    }
}
}







