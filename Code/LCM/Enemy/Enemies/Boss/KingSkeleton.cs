using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.Enemies.Common;
using _01.Script.LCM.Enemy.StateMachine.States;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public class KingSkeleton : EnemyCore
{
    public List<EnemyAttackStruct> _kingSkeletonAttacks;

    [SerializeField] private float _attack3CoolTime;
    [SerializeField] private ParticleSystem _buffPart;
    [SerializeField] private ParticleSystem _buffExplosionPart;
    [SerializeField] private float _buffTime;
    [SerializeField] private float _attackDamageBuffMultiple = 2f;
    [SerializeField] private float _attack1SoundDelay = 0.7f;
    [SerializeField] private float _attack2SoundDelay = 1f;
    [SerializeField] private float _buffExplosionDuration = 1f;
    [SerializeField] private float _summonRandomOffset = 5f;
    [SerializeField] private float _summonSpawnOffsetY = -0.5f;

    [SerializeField] private GameObject _gladiator;
    [SerializeField] private GameObject _marksMan;

    private EnemyAttackCompo _enemyAttackCompo;
    private int _attackIndex;
    private float _attack3Elapsed;
    private float _attackDamageMultiple = 1f;

    private Coroutine _buffCoroutine;

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("kingskeleton.attack.a", EnemyStateType.Attack, ExecuteAttackA);
        registry.Add("kingskeleton.attack.b", EnemyStateType.Attack2, ExecuteAttackB);
    }

    protected override void Awake()
    {
        base.Awake();
        _enemyAttackCompo = GetComponentInChildren<EnemyAttackCompo>();
    }

    protected override void RegisterCustomStates(EnemyStateMachine machine)
    {
        base.RegisterCustomStates(machine);
        machine.Register(
            EnemyStateType.Attack3,
            new EnemyAnimationGateState(
                this,
                EnemyStateType.Attack3,
                EnemyStateType.Move,
                onEnter: OnEnterAttack3State,
                onExit: OnExitAttack3State,
                isCanceled: () => IsDead,
                cancelFallbackState: EnemyStateType.Dead,
                timeoutSeconds: 8f,
                timeoutFallbackState: EnemyStateType.Move
            )
        );
    }

    protected override void Update()
    {
        base.Update();

        if (IsDead)
            return;

        _attack3Elapsed += Time.deltaTime;
        if (_attack3Elapsed < _attack3CoolTime)
            return;

        if (CurrentState == EnemyStateType.Attack ||
            CurrentState == EnemyStateType.Attack2 ||
            CurrentState == EnemyStateType.Attack3)
            return;

        TransitionState(EnemyStateType.Attack3);
        _attack3Elapsed = 0f;
    }

    private void ExecuteAttackA()
    {
        _attackIndex = 0;
        ApplyAttackData(_attackIndex);
        LockMass();
        StartCoroutine(AttackAudioCoroutine());
    }

    private void ExecuteAttackB()
    {
        _attackIndex = 1;
        ApplyAttackData(_attackIndex);
        LockMass();
        StartCoroutine(Attack2AudioCoroutine());
    }

    private void ExecuteBuffSummonAttack()
    {
        if (_buffCoroutine != null)
            StopCoroutine(_buffCoroutine);

        _buffCoroutine = StartCoroutine(BuffCoolTimeCoroutine());

        float randomOffset = Random.Range(-_summonRandomOffset, _summonRandomOffset);

        if (_gladiator != null)
        {
            GameObject gladiatorObject = Instantiate(
                _gladiator,
                new Vector3(transform.position.x + randomOffset, transform.position.y + _summonSpawnOffsetY),
                Quaternion.identity
            );

            Gladiator gladiator = gladiatorObject.GetComponent<Gladiator>();
            if (gladiator != null)
                gladiator.Spawn();
        }

        if (_marksMan != null)
        {
            GameObject marksmanObject = Instantiate(
                _marksMan,
                new Vector3(transform.position.x - randomOffset, transform.position.y + _summonSpawnOffsetY),
                Quaternion.identity
            );

            Marksman marksman = marksmanObject.GetComponent<Marksman>();
            if (marksman != null)
                marksman.Spawn();
        }
    }

    protected override void OnAttackAnimationCompleted(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack || attackType == EnemyStateType.Attack2)
            UnlockMass();

        base.OnAttackAnimationCompleted(attackType);
    }

    private void ApplyAttackData(int index)
    {
        if (_enemyAttackCompo == null)
            return;

        if (_kingSkeletonAttacks == null || index < 0 || index >= _kingSkeletonAttacks.Count)
            return;

        EnemyAttackStruct attackData = _kingSkeletonAttacks[index];

        _enemyAttackCompo.AttackSetting(
            attackData.damage * _attackDamageMultiple,
            attackData.force,
            attackData.attackBoxSize,
            attackData.attackRadius,
            attackData.castType
        );
    }

    private IEnumerator AttackAudioCoroutine()
    {
        yield return new WaitForSeconds(_attack1SoundDelay);
        AudioManager.Instance.PlaySound2D("KingSkeletonAttack", 2f, false, SoundType.SfX);
    }

    private IEnumerator Attack2AudioCoroutine()
    {
        yield return new WaitForSeconds(_attack2SoundDelay);
        AudioManager.Instance.PlaySound2D("KingSkeletonAttack2", 2f, false, SoundType.SfX);
    }

    private IEnumerator BuffCoolTimeCoroutine()
    {
        if (_buffPart != null)
            _buffPart.Play();

        _attackDamageMultiple = _attackDamageBuffMultiple;

        yield return new WaitForSeconds(_buffTime);

        if (_buffPart != null)
            _buffPart.Stop();

        if (_buffExplosionPart != null)
            _buffExplosionPart.gameObject.SetActive(true);

        _attackDamageMultiple = 1f;

        yield return new WaitForSeconds(_buffExplosionDuration);

        if (_buffExplosionPart != null)
            _buffExplosionPart.gameObject.SetActive(false);

        _buffCoroutine = null;
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        UnlockMass();
        AudioManager.Instance.PlaySound2D("KingSkeletonDead", 0f, false, SoundType.SfX);
    }

    private void OnEnterAttack3State()
    {
        LockMass();
        AudioManager.Instance.PlaySound2D("KingSkeletonAttack3", 0f, false, SoundType.SfX);
    }

    private void OnExitAttack3State()
    {
        UnlockMass();
        ExecuteBuffSummonAttack();
    }
}
}







