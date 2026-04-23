using System.Collections.Generic;
using UnityEngine;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Core
{
public abstract class Enemy : Entity
{
    [field: SerializeField] public EnemyDataSO EnemyData { get; private set; }

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundChecker;

    private readonly List<EnemyAttackDefinition> _attackDefinitions = new List<EnemyAttackDefinition>(8);

    public EnemyBody Body { get; private set; }
    public EnemyBrain Brain { get; private set; }

    public EntityHealth EntityHealth => Body?.EntityHealth;
    public Rigidbody2D RbCompo => Body?.Rigidbody;
    public Animator AnimatorCompo => Body?.Animator;
    protected EntityAnimationTrigger AnimTriggerCompo => Body?.AnimationTrigger;

    public Transform TargetTrm => Brain?.TargetTrm;

    public bool CanMove => Body?.CanMove ?? true;

    public bool isShield
    {
        get => Body?.IsShield ?? false;
        set
        {
            if (Body != null)
                Body.IsShield = value;
        }
    }

    public bool isAttackAnimationEnd
    {
        get => Body?.IsAttackAnimationEnd ?? false;
        set
        {
            if (Body != null)
                Body.IsAttackAnimationEnd = value;
        }
    }

    public EnemyStateType CurrentState => Brain != null ? Brain.CurrentState : EnemyStateType.Idle;

    protected override void Awake()
    {
        base.Awake();

        Body = new EnemyBody(this);
        Body.Initialize();
        Body.BindCallbacks(HandleAttackAnimationEnd, HandleKnockBackInternal);

        OnEnemyAwakeCompleted();

        Brain = new EnemyBrain(this);
        BuildAttackDefinitions();
        Brain.Initialize();
    }

    protected virtual void OnEnemyAwakeCompleted()
    {
    }

    protected override void OnDestroy()
    {
        Body?.Dispose();
        base.OnDestroy();
    }

    protected override void HandleHit()
    {
        OnHitReceived();
    }

    protected override void HandleDead()
    {
        Dead();
    }

    protected virtual EnemyStateType GetInitialState()
    {
        return EnemyStateType.Idle;
    }

    protected virtual void RegisterCustomStates(EnemyStateMachine machine)
    {
    }

    protected virtual void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("default.attack", EnemyStateType.Attack, null);
    }

    protected virtual EnemyAttackDefinition SelectAttack(IReadOnlyList<EnemyAttackDefinition> availableAttacks)
    {
        if (availableAttacks == null || availableAttacks.Count == 0)
            return null;

        return availableAttacks[Random.Range(0, availableAttacks.Count)];
    }

    protected virtual bool CanFinishAttackState(EnemyStateType attackType)
    {
        return isAttackAnimationEnd;
    }

    protected virtual void OnAttackAnimationCompleted(EnemyStateType attackType)
    {
    }

    protected virtual void OnKnockBack(Vector2 knockBackForce)
    {
    }

    protected virtual void OnHitReceived()
    {
    }

    internal EnemyStateType GetInitialStateInternal()
    {
        return GetInitialState();
    }

    internal void RegisterCustomStatesInternal(EnemyStateMachine machine)
    {
        RegisterCustomStates(machine);
    }

    internal IReadOnlyList<EnemyAttackDefinition> GetAttackDefinitionsInternal()
    {
        return _attackDefinitions;
    }

    internal EnemyAttackDefinition SelectAttackInternal(IReadOnlyList<EnemyAttackDefinition> availableAttacks)
    {
        return SelectAttack(availableAttacks);
    }

    internal bool CanFinishAttackStateInternal(EnemyStateType attackType)
    {
        return CanFinishAttackState(attackType);
    }

    internal void OnAttackAnimationCompletedInternal(EnemyStateType attackType)
    {
        OnAttackAnimationCompleted(attackType);
    }

    public bool TransitionState(EnemyStateType nextState, bool forceRestart = false)
    {
        return Brain != null && Brain.ChangeState(nextState, forceRestart);
    }

    protected virtual void Update()
    {
        Brain?.Update();
    }

    private void FixedUpdate()
    {
        Brain?.FixedUpdate();
    }

    internal bool TryTransitionToAttack()
    {
        return Brain != null && Brain.TryTransitionToAttack();
    }

    internal void HandleCommonAttackFinished(EnemyStateType attackType)
    {
        Brain?.HandleCommonAttackFinished(attackType);
    }

    protected bool BeginDeadState()
    {
        if (IsDead)
            return false;

        gameObject.layer = DeadBodyLayer;
        IsDead = true;
        TransitionState(EnemyStateType.Dead);
        return true;
    }

    public Vector2 GetMovementDirection()
    {
        if (TargetTrm == null)
            return Vector2.zero;

        return TargetTrm.position - transform.position;
    }

    public void TargetingPlayer()
    {
        Brain?.TargetingPlayer();
    }

    public bool CanTargetingPlayer()
    {
        return Brain != null && Brain.CanTargetingPlayer();
    }

    public bool CanAttackRangePlayer()
    {
        return Brain != null && Brain.CanAttackRangePlayer();
    }

    public bool CanAttackCoolTime()
    {
        return Brain != null && Brain.CanAttackCoolTime();
    }

    public bool GroundCheck()
    {
        if (groundChecker == null || EnemyData == null)
            return false;

        return Physics2D.OverlapBox(groundChecker.position, EnemyData.groundCheckerBoxSize, 0f, whatIsGround) != null;
    }

    public void EnemyRotation()
    {
        Body?.RotateTowards(TargetTrm);
    }

    private void HandleAttackAnimationEnd()
    {
        isAttackAnimationEnd = true;
    }

    private void HandleKnockBackInternal(Vector2 knockBackForce)
    {
        OnKnockBack(knockBackForce);
    }

    public void AddForceToEntity(Vector2 force)
    {
        Body?.AddForce(force);
    }

    public void StopImmediately(bool includeY)
    {
        Body?.StopImmediately(includeY);
    }

    public void KnockBack(Vector2 force, float time)
    {
        Body?.KnockBack(force, time);
    }

    protected void LockMass(float mass = 100f)
    {
        Body?.LockMass(mass);
    }

    protected void UnlockMass()
    {
        Body?.UnlockMass();
    }

    public virtual void Dead()
    {
        BeginDeadState();
    }

    public virtual void IsCanShield()
    {
    }

    public virtual void CreateShield()
    {
    }

    private void BuildAttackDefinitions()
    {
        _attackDefinitions.Clear();

        EnemyAttackRegistry registry = new EnemyAttackRegistry();
        ConfigureAttacks(registry);

        IReadOnlyList<EnemyAttackDefinition> attacks = registry.Attacks;
        for (int i = 0; i < attacks.Count; i++)
            _attackDefinitions.Add(attacks[i]);

        if (_attackDefinitions.Count == 0)
            _attackDefinitions.Add(new EnemyAttackDefinition("default.attack", EnemyStateType.Attack, null));
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (EnemyData == null)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, EnemyData.targetingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyData.attackRange);

        if (groundChecker != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundChecker.position, EnemyData.groundCheckerBoxSize);
        }

        Gizmos.color = Color.white;
    }
#endif
}
}





