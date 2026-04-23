using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine.Events;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine.States;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public enum QueenSlimeBuffType
{
    Attack,
    Defend,
    Heal
}

public class QueenSlime : EnemyCore
{
    public UnityEvent<QueenSlimeBuffType> OnBuff;
    public UnityEvent OnAttack;
    public UnityEvent OnDefendBuff;

    [SerializeField] private PoolItemSO _healSmoke;

    private QueenSlimeBuffType _currentBuffType;

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("queenslime.buff", EnemyStateType.Attack, null);
    }

    protected override void RegisterCustomStates(EnemyStateMachine machine)
    {
        base.RegisterCustomStates(machine);

        machine.Register(
            EnemyStateType.Attack,
            new EnemyAttackState(this, EnemyStateType.Attack, PrepareBuffAttack)
        );
    }

    private void PrepareBuffAttack()
    {
        AudioManager.Instance.PlaySound2D("QueenSlimeAttack", 0f, false, SoundType.SfX);

        LockMass();

        if (EntityHealth != null)
            EntityHealth.IsInvincibility = true;

        _currentBuffType = (QueenSlimeBuffType)Random.Range(0, 3);
        OnBuff?.Invoke(_currentBuffType);
    }

    private void ExecuteCurrentBuff()
    {
        if (_currentBuffType == QueenSlimeBuffType.Attack)
        {
            OnAttack?.Invoke();
            AudioManager.Instance.PlaySound2D("QueenSlimeBuff1", 0f, false, SoundType.SfX);
            return;
        }

        if (_currentBuffType == QueenSlimeBuffType.Defend)
        {
            OnDefendBuff?.Invoke();
            AudioManager.Instance.PlaySound2D("QueenSlimeBuff2", 0f, false, SoundType.SfX);
            return;
        }

        EffectPlayer healSmoke = PoolManager.Instance.Pop(_healSmoke.poolName) as EffectPlayer;
        if (healSmoke != null)
            healSmoke.SetPositionAndPlay(transform.position);

        AudioManager.Instance.PlaySound2D("QueenSlimeBuff3", 0f, false, SoundType.SfX);
    }

    protected override void OnAttackAnimationCompleted(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack)
        {
            ExecuteCurrentBuff();
            UnlockMass();

            if (EntityHealth != null)
                EntityHealth.IsInvincibility = false;
        }

        base.OnAttackAnimationCompleted(attackType);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        UnlockMass();

        if (EntityHealth != null)
            EntityHealth.IsInvincibility = false;

        AudioManager.Instance.PlaySound2D("QueenSlimeDead", 0f, false, SoundType.SfX);
    }
}
}







