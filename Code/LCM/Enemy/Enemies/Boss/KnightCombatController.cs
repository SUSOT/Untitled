using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using _01.Script.LCM.Enemy.Combat.Projectiles;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public class KnightCombatController
{
    private const string Attack1Id = "knight.combo";
    private const string Attack2Id = "knight.jump_slam";
    private const string Attack3Id = "knight.double_strike";
    private const string Attack4Id = "knight.energy_crash";
    private const string Attack5Id = "knight.bullet_burst";
    private const string Attack6Id = "knight.dash_crash";

    private readonly Knight _owner;
    private readonly List<string> _recentAttackIds = new List<string>(2);
    private readonly List<EnemyAttackDefinition> _selectionBuffer = new List<EnemyAttackDefinition>(8);

    private EnemyAttackCompo _enemyAttackCompo;
    private Coroutine _activeAttackCoroutine;

    public KnightCombatController(Knight owner)
    {
        _owner = owner;
    }

    public void Initialize()
    {
        _enemyAttackCompo = _owner.GetComponentInChildren<EnemyAttackCompo>();
    }

    public void ConfigureAttacks(EnemyAttackRegistry registry, System.Func<bool> canUseAdvancedAttacks)
    {
        registry.Add(Attack1Id, EnemyStateType.Attack, ExecuteAttack1);
        registry.Add(Attack2Id, EnemyStateType.Attack2, ExecuteAttack2);
        registry.Add(Attack3Id, EnemyStateType.Attack3, ExecuteAttack3);
        registry.Add(Attack4Id, EnemyStateType.Attack4, ExecuteAttack4, canUseAdvancedAttacks);
        registry.Add(Attack5Id, EnemyStateType.Attack5, ExecuteAttack5, canUseAdvancedAttacks);
        registry.Add(Attack6Id, EnemyStateType.Attack6, ExecuteAttack6, canUseAdvancedAttacks);
    }

    public EnemyAttackDefinition SelectAttack(IReadOnlyList<EnemyAttackDefinition> availableAttacks)
    {
        if (availableAttacks == null || availableAttacks.Count == 0)
            return null;

        _selectionBuffer.Clear();
        for (int i = 0; i < availableAttacks.Count; i++)
        {
            EnemyAttackDefinition attack = availableAttacks[i];
            if (!_recentAttackIds.Contains(attack.Id))
                _selectionBuffer.Add(attack);
        }

        if (_selectionBuffer.Count == 0)
            _selectionBuffer.AddRange(availableAttacks);

        EnemyAttackDefinition selected = _selectionBuffer[Random.Range(0, _selectionBuffer.Count)];
        RememberAttack(selected.Id);
        return selected;
    }

    public void HandleAttackAnimationCompleted(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack4 || attackType == EnemyStateType.Attack6)
            _owner.UnlockMassForControllers();
    }

    public void StopAllEffects()
    {
        StopAttackRoutine();

        if (_owner.Attack2Particle != null) _owner.Attack2Particle.Stop();
        if (_owner.Attack3Particle != null) _owner.Attack3Particle.Stop();
        if (_owner.Attack4Particle != null) _owner.Attack4Particle.Stop();
        if (_owner.Attack4EnergyParticle != null) _owner.Attack4EnergyParticle.Stop();
        if (_owner.Attack5Particle != null) _owner.Attack5Particle.Stop();
        if (_owner.Attack6Particle != null) _owner.Attack6Particle.Stop();
        if (_owner.Attack6TrailParticle != null) _owner.Attack6TrailParticle.Stop();
    }

    private void ExecuteAttack1()
    {
        ApplyAttackData(0);
        StartAttackRoutine(Attack1Coroutine());
    }

    private void ExecuteAttack2()
    {
        ApplyAttackData(1);

        _owner.AddForceToEntity(Vector2.up * _owner.JumpPower);
        _owner.AddForceToEntity(new Vector2(_owner.GetMovementDirection().x * _owner.AttackTiming.attack2HorizontalBoost, 0f));

        StartAttackRoutine(Attack2Coroutine());
    }

    private void ExecuteAttack3()
    {
        ApplyAttackData(2);
        StartAttackRoutine(Attack3Coroutine());
    }

    private void ExecuteAttack4()
    {
        ApplyAttackData(3);
        _owner.LockMassForControllers();
        StartAttackRoutine(Attack4Coroutine());
    }

    private void ExecuteAttack5()
    {
        ApplyAttackData(4);
        StartAttackRoutine(Attack5Coroutine());
    }

    private void ExecuteAttack6()
    {
        ApplyAttackData(5);
        _owner.LockMassForControllers();
        StartAttackRoutine(Attack6Coroutine());
    }

    private void ApplyAttackData(int attackIndex)
    {
        if (_enemyAttackCompo == null)
            return;

        if (_owner.KnightAttacks == null || attackIndex < 0 || attackIndex >= _owner.KnightAttacks.Count)
            return;

        EnemyAttackStruct attackData = _owner.KnightAttacks[attackIndex];

        _enemyAttackCompo.AttackSetting(
            attackData.damage,
            attackData.force,
            attackData.attackBoxSize,
            attackData.attackRadius,
            attackData.castType
        );
    }

    private IEnumerator Attack1Coroutine()
    {
        AudioManager.Instance.PlaySound2D("KnightAttack", 0f, false, SoundType.SfX);
        yield return new WaitForSeconds(_owner.AttackTiming.attack1FirstSoundDelay);

        AudioManager.Instance.PlaySound2D("KnightAttack", 0f, false, SoundType.SfX);
        yield return new WaitForSeconds(_owner.AttackTiming.attack1SecondSoundDelay);

        AudioManager.Instance.PlaySound2D("KnightAttack", 0f, false, SoundType.SfX);
    }

    private IEnumerator Attack2Coroutine()
    {
        yield return new WaitForSeconds(_owner.AttackTiming.attack2EffectDelay);

        AudioManager.Instance.PlaySound2D("KnightAttack2", 0f, false, SoundType.SfX);

        if (_owner.Attack2Particle != null)
            _owner.Attack2Particle.Play();
    }

    private IEnumerator Attack3Coroutine()
    {
        yield return new WaitForSeconds(_owner.AttackTiming.attack3FirstWaveDelay);

        AudioManager.Instance.PlaySound2D("KnightAttack3", 0f, false, SoundType.SfX);

        if (_owner.Attack3Particle != null)
        {
            _owner.Attack3Particle.transform.Rotate(180f, 0f, 0f);
            _owner.Attack3Particle.Play();
        }

        yield return new WaitForSeconds(_owner.AttackTiming.attack3SecondWaveDelay);

        AudioManager.Instance.PlaySound2D("KnightAttack3", 0f, false, SoundType.SfX);

        if (_owner.Attack3Particle != null)
        {
            _owner.Attack3Particle.transform.Rotate(180f, 0f, 0f);
            _owner.Attack3Particle.Play();
        }
    }

    private IEnumerator Attack4Coroutine()
    {
        if (_owner.Attack4EnergyParticle != null)
            _owner.Attack4EnergyParticle.Play();

        AudioManager.Instance.PlaySound2D("KnightAttack4Charging", 0f, false, SoundType.SfX);

        yield return new WaitForSeconds(_owner.AttackTiming.attack4ChargeDuration);

        if (_owner.Attack4Particle != null)
            _owner.Attack4Particle.Play();

        AudioManager.Instance.PlaySound2D("KnightAttack4", 0f, false, SoundType.SfX);
    }

    private IEnumerator Attack5Coroutine()
    {
        if (_owner.Attack5Particle != null)
            _owner.Attack5Particle.Play();

        yield return new WaitForSeconds(_owner.AttackTiming.attack5FirstShotDelay);
        SpawnForwardBullet();

        yield return new WaitForSeconds(_owner.AttackTiming.attack5SecondShotDelay);
        SpawnForwardBullet();
    }

    private IEnumerator Attack6Coroutine()
    {
        Vector2 movementDirection = _owner.GetMovementDirection();

        AudioManager.Instance.PlaySound2D("KnightAttack6Charging", 0f, false, SoundType.SfX);

        if (_owner.Attack6Particle != null)
            _owner.Attack6Particle.Play();

        if (_owner.Attack6TrailParticle != null)
            _owner.Attack6TrailParticle.Play();

        yield return new WaitForSeconds(_owner.AttackTiming.attack6ChargeDuration);

        Vector2 dashDirection = new Vector2(movementDirection.x, 0f).normalized;
        _owner.AddForceToEntity(dashDirection * _owner.DashPower);
        AudioManager.Instance.PlaySound2D("KnightAttack6", 0f, false, SoundType.SfX);

        yield return new WaitForSeconds(_owner.AttackTiming.attack6TrailStopDelay);

        if (_owner.Attack6TrailParticle != null)
            _owner.Attack6TrailParticle.Stop();
    }

    private void SpawnForwardBullet()
    {
        if (_owner.BossBulletPoolItem == null)
            return;

        BossBullet bullet = PoolManager.Instance.Pop(_owner.BossBulletPoolItem.poolName) as BossBullet;
        if (bullet == null)
            return;

        bullet.Initialize(_owner.transform.localScale.x >= 0f ? Vector2.right : Vector2.left);
        bullet.transform.position = _owner.transform.position;
        AudioManager.Instance.PlaySound2D("KnightAttack5", 0f, false, SoundType.SfX);
    }

    private void StartAttackRoutine(IEnumerator routine)
    {
        StopAttackRoutine();
        _activeAttackCoroutine = _owner.StartKnightRoutine(WrapAttackRoutine(routine));
    }

    private IEnumerator WrapAttackRoutine(IEnumerator routine)
    {
        yield return routine;
        _activeAttackCoroutine = null;
    }

    private void StopAttackRoutine()
    {
        if (_activeAttackCoroutine == null)
            return;

        _owner.StopKnightRoutine(_activeAttackCoroutine);
        _activeAttackCoroutine = null;
    }

    private void RememberAttack(string attackId)
    {
        if (_recentAttackIds.Count >= 2)
            _recentAttackIds.RemoveAt(0);

        _recentAttackIds.Add(attackId);
    }
}
}







