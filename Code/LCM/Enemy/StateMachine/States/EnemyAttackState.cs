using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.StateMachine.States
{
    public class EnemyAttackState : EnemyState
    {
        private readonly Action _onEnterAttack;
        private readonly Func<bool> _canFinishAttack;
        private readonly EnemyStateType _attackType;

        public EnemyAttackState(
            EnemyCore enemy,
            EnemyStateType attackType,
            Action onEnterAttack,
            Func<bool> canFinishAttack = null
        ) : base(enemy, attackType)
        {
            _attackType = attackType;
            _onEnterAttack = onEnterAttack;
            _canFinishAttack = canFinishAttack ?? (() => enemy.isAttackAnimationEnd);
        }

        protected override void OnEnter()
        {
            Enemy.StopImmediately(true);
            Enemy.isAttackAnimationEnd = false;
            _onEnterAttack?.Invoke();
        }

        public override void Update()
        {
            if (!_canFinishAttack())
                return;

            Enemy.isAttackAnimationEnd = false;
            Enemy.HandleCommonAttackFinished(_attackType);
        }

        protected override void OnExit()
        {
            Enemy.isAttackAnimationEnd = false;
        }
    }
}







