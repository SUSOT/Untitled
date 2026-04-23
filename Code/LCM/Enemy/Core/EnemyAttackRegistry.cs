using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections.Generic;
using System;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Core
{
public class EnemyAttackRegistry
{
    private readonly List<EnemyAttackDefinition> _attacks = new List<EnemyAttackDefinition>();
    private readonly HashSet<string> _attackIds = new HashSet<string>();

    public IReadOnlyList<EnemyAttackDefinition> Attacks => _attacks;

    public EnemyAttackDefinition Add(
        string id,
        EnemyStateType stateType,
        Action execute,
        Func<bool> canUse = null,
        Func<bool> canFinish = null
    )
    {
        EnemyAttackDefinition definition = new EnemyAttackDefinition(id, stateType, execute, canUse, canFinish);
        string uniqueId = definition.Id;
        int suffix = 1;

        while (!_attackIds.Add(uniqueId))
        {
            uniqueId = $"{definition.Id}.{suffix}";
            suffix++;
        }

        if (uniqueId != definition.Id)
            definition = new EnemyAttackDefinition(uniqueId, stateType, execute, canUse, canFinish);

        _attacks.Add(definition);
        return definition;
    }
}
}







