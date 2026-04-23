using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public class KingSlime : CounterableEnemy
{
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _attack3JumpYPower = 1f;
    [SerializeField] private float _attack3Delay = 1f;

    public UnityEvent OnAttack3;

    public List<EnemyAttackStruct> _kingSlimeAttacks;

    private int _attackIndex;
    private EnemyAttackCompo _enemyAttackCompo;

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("kingslime.attack.a", EnemyStateType.Attack, ExecuteAttackA);
        registry.Add("kingslime.attack.b", EnemyStateType.Attack2, ExecuteAttackB);
        registry.Add("kingslime.attack.c", EnemyStateType.Attack3, ExecuteAttackC);
    }

    protected override void Awake()
    {
        base.Awake();
        _enemyAttackCompo = GetComponentInChildren<EnemyAttackCompo>();
    }

    private void ExecuteAttackA()
    {
        _attackIndex = 0;
        ApplyAttackData(_attackIndex);
        LockMass();
        AudioManager.Instance.PlaySound2D("KingSlimeAttack", 0f, false, SoundType.SfX);
    }

    private void ExecuteAttackB()
    {
        _attackIndex = 1;
        ApplyAttackData(_attackIndex);
        LockMass();
        AudioManager.Instance.PlaySound2D("KingSlimeAttack2", 0f, false, SoundType.SfX);
    }

    private void ExecuteAttackC()
    {
        _attackIndex = 2;
        ApplyAttackData(_attackIndex);
        StartCoroutine(Attack3Coroutine());
    }

    protected override void OnAttackAnimationCompleted(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack || attackType == EnemyStateType.Attack2)
            UnlockMass();

        base.OnAttackAnimationCompleted(attackType);
    }

    private void ApplyAttackData(int index)
    {
        if (_enemyAttackCompo == null)
            return;

        if (_kingSlimeAttacks == null || index < 0 || index >= _kingSlimeAttacks.Count)
            return;

        EnemyAttackStruct attackData = _kingSlimeAttacks[index];

        _enemyAttackCompo.AttackSetting(
            attackData.damage,
            attackData.force,
            attackData.attackBoxSize,
            attackData.attackRadius,
            attackData.castType
        );
    }

    private IEnumerator Attack3Coroutine()
    {
        Vector2 direction = GetMovementDirection().normalized;

        RbCompo.AddForce(new Vector2(direction.x, _attack3JumpYPower) * _jumpPower, ForceMode2D.Impulse);

        yield return new WaitForSeconds(_attack3Delay);

        OnAttack3?.Invoke();
        AudioManager.Instance.PlaySound2D("KingSlimeAttack3", 0f, false, SoundType.SfX);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        UnlockMass();
        AudioManager.Instance.PlaySound2D("KingSlimeDead", 0f, false, SoundType.SfX);
    }
}
}







