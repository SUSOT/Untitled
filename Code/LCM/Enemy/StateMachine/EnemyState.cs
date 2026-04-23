using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;

namespace _01.Script.LCM.Enemy.StateMachine
{
public abstract class EnemyState : IEnemyState
{
    protected readonly EnemyCore Enemy;
    private readonly int _animBoolHash;

    protected EnemyState(EnemyCore enemy, EnemyStateType stateType)
    {
        Enemy = enemy;
        _animBoolHash = Animator.StringToHash(stateType.ToString());
    }

    public void Enter()
    {
        if (Enemy.AnimatorCompo != null)
            Enemy.AnimatorCompo.SetBool(_animBoolHash, true);

        OnEnter();
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public void Exit()
    {
        OnExit();

        if (Enemy.AnimatorCompo != null)
            Enemy.AnimatorCompo.SetBool(_animBoolHash, false);
    }

    protected virtual void OnEnter()
    {
    }

    protected virtual void OnExit()
    {
    }
}
}







