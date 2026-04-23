using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections.Generic;
using _01.Script.LCM.Enemy.Core;

namespace _01.Script.LCM.Enemy.StateMachine
{
public class EnemyStateMachine
{
    private readonly Dictionary<EnemyStateType, IEnemyState> _states = new Dictionary<EnemyStateType, IEnemyState>();

    public EnemyStateType CurrentState { get; private set; }
    public bool IsInitialized { get; private set; }

    public void Register(EnemyStateType stateType, IEnemyState state)
    {
        _states[stateType] = state;
    }

    public bool HasState(EnemyStateType stateType)
    {
        return _states.ContainsKey(stateType);
    }

    public bool ChangeState(EnemyStateType nextState, bool forceRestart = false)
    {
        if (!_states.TryGetValue(nextState, out IEnemyState next))
            return false;

        if (IsInitialized && CurrentState == nextState && !forceRestart)
            return false;

        if (IsInitialized)
            _states[CurrentState].Exit();

        CurrentState = nextState;
        IsInitialized = true;

        next.Enter();
        return true;
    }

    public void Update()
    {
        if (!IsInitialized)
            return;

        _states[CurrentState].Update();
    }

    public void FixedUpdate()
    {
        if (!IsInitialized)
            return;

        _states[CurrentState].FixedUpdate();
    }
}
}







