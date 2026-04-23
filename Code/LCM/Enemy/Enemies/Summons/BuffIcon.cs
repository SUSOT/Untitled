using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.Enemies.Boss;
using _01.Script.LCM.Enemy.StateMachine.States;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Summons
{
public class BuffIcon : EnemyCore
{
    [SerializeField] private Transform _queen;

    protected override void RegisterCustomStates(EnemyStateMachine machine)
    {
        base.RegisterCustomStates(machine);

        machine.Register(EnemyStateType.Attack, new EnemyAttackState(this, EnemyStateType.Attack, null));
        machine.Register(EnemyStateType.Attack2, new EnemyAttackState(this, EnemyStateType.Attack2, null));
        machine.Register(EnemyStateType.Attack3, new EnemyAttackState(this, EnemyStateType.Attack3, null));
    }

    protected override void OnAttackAnimationCompleted(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack || attackType == EnemyStateType.Attack2 || attackType == EnemyStateType.Attack3)
        {
            TransitionState(EnemyStateType.Idle);
            return;
        }

        base.OnAttackAnimationCompleted(attackType);
    }

    public void BuffIconChange(QueenSlimeBuffType buffType)
    {
        if (_queen != null)
            transform.position = new Vector3(_queen.position.x, transform.position.y, transform.position.z);

        if (buffType == QueenSlimeBuffType.Attack)
        {
            TransitionState(EnemyStateType.Attack);
            return;
        }

        if (buffType == QueenSlimeBuffType.Defend)
        {
            TransitionState(EnemyStateType.Attack2);
            return;
        }

        TransitionState(EnemyStateType.Attack3);
    }

    public override void Dead()
    {
    }
}
}







