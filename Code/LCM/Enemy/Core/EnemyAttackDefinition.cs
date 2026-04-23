using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Core
{
    public class EnemyAttackDefinition
    {
        public string Id { get; }
        public EnemyStateType StateType { get; }
        public Action Execute { get; }
        public Func<bool> CanUse { get; }
        public Func<bool> CanFinish { get; }

        public EnemyAttackDefinition(
            string id,
            EnemyStateType stateType,
            Action execute,
            Func<bool> canUse = null,
            Func<bool> canFinish = null
        )
        {
            Id = string.IsNullOrWhiteSpace(id) ? stateType.ToString() : id;
            StateType = stateType;
            Execute = execute;
            CanUse = canUse;
            CanFinish = canFinish;
        }
    }
}