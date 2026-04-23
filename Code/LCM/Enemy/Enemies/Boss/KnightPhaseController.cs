using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public class KnightPhaseController
{
    private readonly Knight _owner;
    private readonly KnightShieldController _shieldController;
    private readonly KnightCombatController _combatController;

    private Coroutine _phaseTransitionCoroutine;

    public bool IsPageTwo { get; private set; }

    public KnightPhaseController(
        Knight owner,
        KnightShieldController shieldController,
        KnightCombatController combatController
    )
    {
        _owner = owner;
        _shieldController = shieldController;
        _combatController = combatController;
    }

    public void EnterPageTwoState()
    {
        _owner.StopImmediately(true);

        if (_owner.EntityHealth != null)
            _owner.EntityHealth.IsInvincibility = true;

        _owner.LockMassForControllers();
    }

    public void ExitPageTwoState()
    {
        if (_owner.EntityHealth != null)
            _owner.EntityHealth.IsInvincibility = false;

        _owner.UnlockMassForControllers();
    }

    public void TryStartPageTwo(float nextHealth)
    {
        if (IsPageTwo || _owner.EntityHealth == null || _owner.IsDead)
            return;

        float threshold = _owner.EntityHealth.maxHealth * _owner.PhaseTuning.phaseTwoHealthThresholdRatio;
        if (nextHealth > threshold)
            return;

        IsPageTwo = true;
        _owner.TransitionState(EnemyStateType.PageTwo);

        _shieldController.ForceEndShield();
        _combatController.StopAllEffects();

        if (_phaseTransitionCoroutine != null)
            _owner.StopKnightRoutine(_phaseTransitionCoroutine);

        _phaseTransitionCoroutine = _owner.StartKnightRoutine(PageTwoCoroutine());
    }

    public void OnDead()
    {
        if (_phaseTransitionCoroutine == null)
            return;

        _owner.StopKnightRoutine(_phaseTransitionCoroutine);
        _phaseTransitionCoroutine = null;
    }

    private IEnumerator PageTwoCoroutine()
    {
        yield return new WaitForSeconds(_owner.PhaseTuning.phaseTwoChargeDelay);

        if (_owner.PageTwoParticle != null)
            _owner.PageTwoParticle.Play();

        yield return new WaitForSeconds(_owner.PhaseTuning.phaseTwoExplosionDelay);

        AudioManager.Instance.PlaySound2D("BossExplosion", 0f, false, SoundType.SfX);

        if (_owner.PageTwoExplosionParticle != null)
            _owner.PageTwoExplosionParticle.Play();

        yield return new WaitForSeconds(_owner.PhaseTuning.phaseTwoRecoveryDelay);

        _owner.TransitionState(EnemyStateType.Idle);
        _shieldController.ActivateShield();
        _shieldController.ResetAbilityCooldownClock();
        _shieldController.ResetSpinSwordSpawnClock();

        if (_owner.AuraParticle != null)
            _owner.AuraParticle.Play();

        _phaseTransitionCoroutine = null;
    }
}
}







