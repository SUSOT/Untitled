using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.StateMachine.States
{
public class EnemyAnimationGateState : EnemyState
{
    private readonly EnemyCore _owner;
    private readonly EnemyStateType _nextState;
    private readonly Action _onEnter;
    private readonly Action _onExit;
    private readonly Func<bool> _isCompleted;
    private readonly bool _stopImmediately;
    private readonly bool _resetAnimationEndOnEnter;
    private readonly bool _resetAnimationEndOnExit;

    private readonly Func<bool> _isCanceled;
    private readonly Action _onCanceled;
    private readonly bool _useCancelFallback;
    private readonly EnemyStateType _cancelFallbackState;

    private readonly bool _useTimeoutFallback;
    private readonly float _timeoutSeconds;
    private readonly EnemyStateType _timeoutFallbackState;

    private float _enteredTime;

    public EnemyAnimationGateState(
        EnemyCore owner,
        EnemyStateType stateType,
        EnemyStateType nextState,
        Action onEnter = null,
        Action onExit = null,
        Func<bool> isCompleted = null,
        bool stopImmediately = true,
        bool resetAnimationEndOnEnter = true,
        bool resetAnimationEndOnExit = true,
        Func<bool> isCanceled = null,
        Action onCanceled = null,
        EnemyStateType? cancelFallbackState = null,
        float timeoutSeconds = -1f,
        EnemyStateType? timeoutFallbackState = null
    ) : base(owner, stateType)
    {
        _owner = owner;
        _nextState = nextState;
        _onEnter = onEnter;
        _onExit = onExit;
        _isCompleted = isCompleted ?? (() => _owner.isAttackAnimationEnd);
        _stopImmediately = stopImmediately;
        _resetAnimationEndOnEnter = resetAnimationEndOnEnter;
        _resetAnimationEndOnExit = resetAnimationEndOnExit;

        _isCanceled = isCanceled;
        _onCanceled = onCanceled;
        _useCancelFallback = cancelFallbackState.HasValue;
        _cancelFallbackState = cancelFallbackState.GetValueOrDefault(EnemyStateType.Idle);

        _useTimeoutFallback = timeoutSeconds > 0f && timeoutFallbackState.HasValue;
        _timeoutSeconds = timeoutSeconds;
        _timeoutFallbackState = timeoutFallbackState.GetValueOrDefault(EnemyStateType.Idle);
    }

    protected override void OnEnter()
    {
        _enteredTime = Time.time;

        if (_stopImmediately)
            _owner.StopImmediately(true);

        if (_resetAnimationEndOnEnter)
            _owner.isAttackAnimationEnd = false;

        _onEnter?.Invoke();
    }

    public override void Update()
    {
        if (_isCanceled != null && _isCanceled())
        {
            _onCanceled?.Invoke();
            EnemyStateType fallbackState = _useCancelFallback ? _cancelFallbackState : _nextState;
            _owner.TransitionState(fallbackState);
            return;
        }

        if (_isCompleted())
        {
            _owner.TransitionState(_nextState);
            return;
        }

        if (_useTimeoutFallback && Time.time >= _enteredTime + _timeoutSeconds)
            _owner.TransitionState(_timeoutFallbackState);
    }

    protected override void OnExit()
    {
        _onExit?.Invoke();

        if (_resetAnimationEndOnExit)
            _owner.isAttackAnimationEnd = false;
    }
}
}







