using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.Enemies.Boss.States;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public class Knight : EnemyCore
{
    [Header("Combat")]
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _dashPower;
    [SerializeField] private PoolItemSO _bossBullet;
    [SerializeField] private List<EnemyAttackStruct> _knightAttacks;
    [SerializeField] private KnightAttackTimingTuning _attackTiming = new KnightAttackTimingTuning();

    [Header("Shield")]
    [SerializeField] private float _takeShieldCoolTime;
    [FormerlySerializedAs("_ShieldCoolTime")]
    [SerializeField] private float _shieldDuration;
    [SerializeField] private int _shieldAttackCount;
    [SerializeField] private PoolItemSO _spinSword;
    [SerializeField] private float _spinSwordSpawnTime;
    [SerializeField] private Slider _shieldSlider;

    public UnityEvent OnPrepareShield;
    public UnityEvent OnDestroyShield;

    [Header("Phase")]
    [SerializeField] private KnightPhaseTuning _phaseTuning = new KnightPhaseTuning();

    [Header("Movement")]
    [SerializeField] private KnightMovementTuning _movementTuning = new KnightMovementTuning();

    [Header("Particles")]
    [SerializeField] private ParticleSystem _shieldParticle;
    [SerializeField] private ParticleSystem _attack2Particle;
    [SerializeField] private ParticleSystem _attack3Particle;
    [SerializeField] private ParticleSystem _attack4EnergyParticle;
    [SerializeField] private ParticleSystem _attack4Particle;
    [SerializeField] private ParticleSystem _attack5Particle;
    [SerializeField] private ParticleSystem _attack6Particle;
    [SerializeField] private ParticleSystem _attack6TrailParticle;
    [SerializeField] private ParticleSystem _pageTwoParticle;
    [SerializeField] private ParticleSystem _pageTwoExplosionParticle;
    [SerializeField] private ParticleSystem _auraParticle;

    private KnightCombatController _combatController;
    private KnightShieldController _shieldController;
    private KnightPhaseController _phaseController;
    private KnightLocomotionController _locomotionController;
    private IKnightStateContext _stateContext;

    internal float JumpPower => _jumpPower;
    internal float DashPower => _dashPower;
    internal float TakeShieldCooldown => _takeShieldCoolTime;
    internal float ShieldDuration => _shieldDuration;
    internal int ShieldHitLimit => _shieldAttackCount;
    internal float SpinSwordSpawnInterval => _spinSwordSpawnTime;

    internal PoolItemSO SpinSwordPoolItem => _spinSword;
    internal PoolItemSO BossBulletPoolItem => _bossBullet;

    internal List<EnemyAttackStruct> KnightAttacks => _knightAttacks;
    internal KnightAttackTimingTuning AttackTiming => _attackTiming;
    internal KnightPhaseTuning PhaseTuning => _phaseTuning;

    internal ParticleSystem ShieldParticle => _shieldParticle;
    internal ParticleSystem Attack2Particle => _attack2Particle;
    internal ParticleSystem Attack3Particle => _attack3Particle;
    internal ParticleSystem Attack4EnergyParticle => _attack4EnergyParticle;
    internal ParticleSystem Attack4Particle => _attack4Particle;
    internal ParticleSystem Attack5Particle => _attack5Particle;
    internal ParticleSystem Attack6Particle => _attack6Particle;
    internal ParticleSystem Attack6TrailParticle => _attack6TrailParticle;
    internal ParticleSystem PageTwoParticle => _pageTwoParticle;
    internal ParticleSystem PageTwoExplosionParticle => _pageTwoExplosionParticle;
    internal ParticleSystem AuraParticle => _auraParticle;

    internal Slider ShieldSlider => _shieldSlider;

    protected override void OnEnemyAwakeCompleted()
    {
        base.OnEnemyAwakeCompleted();

        _combatController = new KnightCombatController(this);
        _combatController.Initialize();

        _shieldController = new KnightShieldController(this);
        _phaseController = new KnightPhaseController(this, _shieldController, _combatController);
        _locomotionController = new KnightLocomotionController(this, _shieldController, _movementTuning);
        _stateContext = new KnightStateContext(this, _locomotionController, _shieldController, _phaseController);

        if (EntityHealth != null)
            EntityHealth.hp.OnValueChanged += HandleHpChanged;
    }

    protected override void OnDestroy()
    {
        if (EntityHealth != null)
            EntityHealth.hp.OnValueChanged -= HandleHpChanged;

        base.OnDestroy();
    }

    private void OnEnable()
    {
        _shieldController?.ResetAbilityCooldownClock();
    }

    protected override void RegisterCustomStates(EnemyStateMachine machine)
    {
        base.RegisterCustomStates(machine);

        machine.Register(EnemyStateType.Move, new KnightMoveState(this, _stateContext));
        machine.Register(EnemyStateType.Run, new KnightRunState(this, _stateContext));
        machine.Register(EnemyStateType.Shield, new KnightShieldState(this, _stateContext));
        machine.Register(EnemyStateType.PageTwo, new KnightPageTwoState(this, _stateContext));
    }

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        _combatController.ConfigureAttacks(registry, () => _phaseController.IsPageTwo);
    }

    protected override EnemyAttackDefinition SelectAttack(IReadOnlyList<EnemyAttackDefinition> availableAttacks)
    {
        return _combatController.SelectAttack(availableAttacks);
    }

    protected override void OnAttackAnimationCompleted(EnemyStateType attackType)
    {
        _combatController.HandleAttackAnimationCompleted(attackType);
        base.OnAttackAnimationCompleted(attackType);
    }

    protected override void Update()
    {
        base.Update();
        _shieldController.Tick(_phaseController.IsPageTwo, TargetTrm);
    }

    public override void IsCanShield()
    {
        _shieldController.TryTakeShield();
    }

    public override void CreateShield()
    {
        _shieldController.ActivateShield();
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        _phaseController.OnDead();
        _shieldController.ForceEndShield();
        _combatController.StopAllEffects();
        UnlockMass();

        AudioManager.Instance.PlaySound2D("KnightDead", 0f, false, SoundType.SfX);
    }

    internal void LockMassForControllers()
    {
        LockMass();
    }

    internal void UnlockMassForControllers()
    {
        UnlockMass();
    }

    internal Coroutine StartKnightRoutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    internal void StopKnightRoutine(Coroutine routine)
    {
        if (routine != null)
            StopCoroutine(routine);
    }

    internal void InvokePrepareShieldEvent()
    {
        OnPrepareShield?.Invoke();
    }

    internal void InvokeDestroyShieldEvent()
    {
        OnDestroyShield?.Invoke();
    }

    private void HandleHpChanged(float previous, float next)
    {
        _shieldController.NotifyDamageTaken(previous, next);
        _phaseController.TryStartPageTwo(next);
    }
}
}







