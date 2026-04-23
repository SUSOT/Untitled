using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public class KnightShieldController
{
    private readonly Knight _owner;

    private float _lastAbilityTime;
    private float _shieldStartTime;
    private float _lastSpinSwordSpawnTime = Mathf.NegativeInfinity;

    private bool _isShieldActive;
    private int _shieldHitCount;

    public bool IsShieldActive => _isShieldActive;

    public KnightShieldController(Knight owner)
    {
        _owner = owner;
    }

    public void ResetAbilityCooldownClock()
    {
        _lastAbilityTime = Time.time;
    }

    public void ResetSpinSwordSpawnClock()
    {
        _lastSpinSwordSpawnTime = Time.time;
    }

    public void NotifyDamageTaken(float previousHealth, float nextHealth)
    {
        if (!_isShieldActive)
            return;

        if (nextHealth < previousHealth)
            _shieldHitCount++;
    }

    public void Tick(bool isPageTwo, Transform target)
    {
        if (!_isShieldActive)
            return;

        if (ShouldDeactivateShield())
        {
            DeactivateShield();
            return;
        }

        UpdateSlider();
        TrySpawnSpinSword(isPageTwo, target);
    }

    public void TryTakeShield()
    {
        if (_owner.IsDead)
            return;

        if (_isShieldActive)
            return;

        if (_owner.CurrentState == EnemyStateType.Shield || _owner.CurrentState == EnemyStateType.PageTwo)
            return;

        if (Time.time < _lastAbilityTime + _owner.TakeShieldCooldown)
            return;

        _lastAbilityTime = Time.time;
        _owner.TransitionState(EnemyStateType.Shield);
        _owner.InvokePrepareShieldEvent();
    }

    public void EnterShieldState()
    {
        _owner.isShield = true;
        _owner.StopImmediately(true);

        if (_owner.EntityHealth != null)
            _owner.EntityHealth.IsInvincibility = true;

        _owner.LockMassForControllers();
    }

    public void ExitShieldState()
    {
        ActivateShield();
        _owner.isAttackAnimationEnd = false;

        if (_owner.EntityHealth != null)
            _owner.EntityHealth.IsInvincibility = false;

        _owner.UnlockMassForControllers();
        _owner.isShield = false;
    }

    public void ActivateShield()
    {
        if (_owner.ShieldParticle != null)
            _owner.ShieldParticle.Play();

        AudioManager.Instance.PlaySound2D("KnightShield", 0f, false, SoundType.SfX);

        _isShieldActive = true;
        _shieldHitCount = 0;
        _shieldStartTime = Time.time;
        _lastSpinSwordSpawnTime = Time.time;

        if (_owner.EntityHealth != null)
            _owner.EntityHealth.IsShield = true;

        if (_owner.ShieldSlider != null)
            _owner.ShieldSlider.value = 1f;
    }

    public void ForceEndShield(bool invokeEvent = false)
    {
        if (!_isShieldActive)
        {
            if (_owner.EntityHealth != null)
                _owner.EntityHealth.IsShield = false;

            if (_owner.ShieldSlider != null)
                _owner.ShieldSlider.value = 0f;

            if (_owner.ShieldParticle != null)
                _owner.ShieldParticle.Stop();

            return;
        }

        DeactivateShield(invokeEvent);
    }

    private bool ShouldDeactivateShield()
    {
        return _shieldHitCount >= _owner.ShieldHitLimit ||
               Time.time >= _shieldStartTime + _owner.ShieldDuration;
    }

    private void DeactivateShield(bool invokeEvent = true)
    {
        if (invokeEvent)
            _owner.InvokeDestroyShieldEvent();

        if (_owner.ShieldParticle != null)
            _owner.ShieldParticle.Stop();

        _isShieldActive = false;
        _shieldHitCount = 0;

        if (_owner.EntityHealth != null)
            _owner.EntityHealth.IsShield = false;

        if (_owner.ShieldSlider != null)
            _owner.ShieldSlider.value = 0f;
    }

    private void UpdateSlider()
    {
        if (_owner.ShieldSlider == null)
            return;

        if (_owner.ShieldDuration <= 0f)
        {
            _owner.ShieldSlider.value = 0f;
            return;
        }

        float elapsedTime = Time.time - _shieldStartTime;
        float remainingTime = _owner.ShieldDuration - elapsedTime;
        _owner.ShieldSlider.value = Mathf.Clamp01(remainingTime / _owner.ShieldDuration);
    }

    private void TrySpawnSpinSword(bool isPageTwo, Transform target)
    {
        if (!isPageTwo || target == null)
            return;

        if (_owner.SpinSwordPoolItem == null)
            return;

        if (Time.time < _lastSpinSwordSpawnTime + _owner.SpinSwordSpawnInterval)
            return;

        MonoBehaviour spinSword = PoolManager.Instance.Pop(_owner.SpinSwordPoolItem.poolName) as MonoBehaviour;
        if (spinSword == null)
            return;

        spinSword.transform.position = new Vector3(target.position.x, 0f, 0f);
        _lastSpinSwordSpawnTime = Time.time;
    }
}
}







