using System.Collections.Generic;
using UnityEngine;
using _01.Script.LCM.Enemy.StateMachine;
using _01.Script.LCM.Enemy.StateMachine.States;

namespace _01.Script.LCM.Enemy.Core
{
public class EnemyBrain
{
    private readonly Enemy _owner;
    private readonly EnemyStateMachine _stateMachine = new EnemyStateMachine();
    private readonly EnemyTargetSensor _targetSensor;
    private readonly EnemyAttackCooldown _attackCooldown;
    private readonly List<EnemyAttackDefinition> _allAttacks = new List<EnemyAttackDefinition>(8);
    private readonly List<EnemyAttackDefinition> _availableAttacks = new List<EnemyAttackDefinition>(8);

    private EnemyAttackDefinition _currentAttack;

    public Transform TargetTrm { get; private set; }

    public EnemyStateType CurrentState =>
        _stateMachine.IsInitialized ? _stateMachine.CurrentState : EnemyStateType.Idle;

    public EnemyBrain(Enemy owner)
    {
        _owner = owner;
        _targetSensor = new EnemyTargetSensor(owner.transform);
        _attackCooldown = new EnemyAttackCooldown();
    }

    public void Initialize()
    {
        RegisterDefaultStates();
        _owner.RegisterCustomStatesInternal(_stateMachine);
        _stateMachine.ChangeState(_owner.GetInitialStateInternal(), true);
    }

    public void Update()
    {
        _stateMachine.Update();
    }

    public void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    public bool ChangeState(EnemyStateType nextState, bool forceRestart = false)
    {
        return _stateMachine.ChangeState(nextState, forceRestart);
    }

    public void TargetingPlayer()
    {
        if (!TryGetEnemyData(out EnemyDataSO enemyData))
        {
            TargetTrm = null;
            return;
        }

        TargetTrm = _targetSensor.AcquireTarget(TargetTrm, enemyData.targetingRange, enemyData.whatIsPlayer);
    }

    public bool CanTargetingPlayer()
    {
        if (!TryGetEnemyData(out EnemyDataSO enemyData))
            return false;

        return _targetSensor.HasTargetInRange(enemyData.targetingRange, enemyData.whatIsPlayer);
    }

    public bool CanAttackRangePlayer()
    {
        if (!TryGetEnemyData(out EnemyDataSO enemyData))
            return false;

        return _targetSensor.HasTargetInRange(enemyData.attackRange, enemyData.whatIsPlayer);
    }

    public bool CanAttackCoolTime()
    {
        if (!TryGetEnemyData(out EnemyDataSO enemyData))
            return false;

        bool isTargetInRange = _targetSensor.HasTargetInRange(enemyData.attackRange, enemyData.whatIsPlayer);
        _attackCooldown.UpdateRangeState(isTargetInRange, Time.time);
        return _attackCooldown.IsReady(Time.time, enemyData.attackCoolTime);
    }

    public bool TryTransitionToAttack()
    {
        if (!TryGetEnemyData(out EnemyDataSO enemyData))
            return false;

        bool isTargetInRange = _targetSensor.HasTargetInRange(enemyData.attackRange, enemyData.whatIsPlayer);
        _attackCooldown.UpdateRangeState(isTargetInRange, Time.time);
        if (!isTargetInRange)
            return false;

        BuildAvailableAttackList();
        if (_availableAttacks.Count == 0)
            return false;

        if (!_attackCooldown.IsReady(Time.time, enemyData.attackCoolTime))
            return false;

        EnemyAttackDefinition selectedAttack = _owner.SelectAttackInternal(_availableAttacks);
        if (selectedAttack == null || !_availableAttacks.Contains(selectedAttack))
            selectedAttack = _availableAttacks[0];

        if (selectedAttack == null)
            return false;

        _currentAttack = selectedAttack;

        EnemyStateType targetState = ResolveAttackState(selectedAttack.StateType);
        if (!ChangeState(targetState))
            return false;

        _attackCooldown.Consume(Time.time);
        return true;
    }

    public void HandleCommonAttackFinished(EnemyStateType attackType)
    {
        EnemyStateType previousState = CurrentState;

        _owner.OnAttackAnimationCompletedInternal(attackType);
        _currentAttack = null;

        if (CurrentState != previousState)
            return;

        TargetingPlayer();
        ChangeState(TargetTrm != null ? EnemyStateType.Move : EnemyStateType.Idle);
    }

    private void RegisterDefaultStates()
    {
        _stateMachine.Register(EnemyStateType.Idle, new EnemyIdleState(_owner));
        _stateMachine.Register(EnemyStateType.Move, new EnemyMoveState(_owner));
        _stateMachine.Register(EnemyStateType.Dead, new EnemyDeadState(_owner));

        _allAttacks.Clear();
        IReadOnlyList<EnemyAttackDefinition> attackDefinitions = _owner.GetAttackDefinitionsInternal();

        for (int i = 0; i < attackDefinitions.Count; i++)
            _allAttacks.Add(attackDefinitions[i]);

        HashSet<EnemyStateType> registeredStates = new HashSet<EnemyStateType>();
        for (int i = 0; i < _allAttacks.Count; i++)
        {
            EnemyAttackDefinition attack = _allAttacks[i];
            if (!registeredStates.Add(attack.StateType))
                continue;

            RegisterAttackState(attack.StateType);
        }

        if (registeredStates.Count == 0)
            RegisterAttackState(EnemyStateType.Attack);
    }

    private void RegisterAttackState(EnemyStateType stateType)
    {
        _stateMachine.Register(
            stateType,
            new EnemyAttackState(
                _owner,
                stateType,
                () => ExecuteAttackByState(stateType),
                () => CanFinishAttackByState(stateType)
            )
        );
    }

    private void ExecuteAttackByState(EnemyStateType stateType)
    {
        if (_currentAttack == null || _currentAttack.StateType != stateType)
            _currentAttack = FindFirstAttackByState(stateType);

        _currentAttack?.Execute?.Invoke();
    }

    private bool CanFinishAttackByState(EnemyStateType stateType)
    {
        if (_currentAttack != null && _currentAttack.StateType == stateType && _currentAttack.CanFinish != null)
            return _currentAttack.CanFinish();

        return _owner.CanFinishAttackStateInternal(stateType);
    }

    private EnemyAttackDefinition FindFirstAttackByState(EnemyStateType stateType)
    {
        for (int i = 0; i < _allAttacks.Count; i++)
        {
            EnemyAttackDefinition attack = _allAttacks[i];
            if (attack.StateType == stateType)
                return attack;
        }

        return null;
    }

    private void BuildAvailableAttackList()
    {
        _availableAttacks.Clear();

        for (int i = 0; i < _allAttacks.Count; i++)
        {
            EnemyAttackDefinition attack = _allAttacks[i];
            if (!_stateMachine.HasState(attack.StateType))
                continue;

            if (attack.CanUse != null && !attack.CanUse())
                continue;

            _availableAttacks.Add(attack);
        }
    }

    private EnemyStateType ResolveAttackState(EnemyStateType selectedState)
    {
        if (_stateMachine.HasState(selectedState))
            return selectedState;

        for (int i = 0; i < _allAttacks.Count; i++)
        {
            EnemyAttackDefinition attack = _allAttacks[i];
            if (_stateMachine.HasState(attack.StateType))
                return attack.StateType;
        }

        return EnemyStateType.Attack;
    }

    private bool TryGetEnemyData(out EnemyDataSO enemyData)
    {
        enemyData = _owner.EnemyData;
        return enemyData != null;
    }
}
}
