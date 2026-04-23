using DG.Tweening;
using System;
using UnityEngine;

namespace _01.Script.LCM.Enemy.Core
{
public class EnemyBody
{
    private readonly Enemy _owner;

    private float _baseScaleX;
    private bool _isMassLocked;
    private float _cachedMass;
    private Tween _knockBackRecoverTween;

    private Action _onAnimationEnd;
    private Action<Vector2> _onKnockBack;

    public EntityHealth EntityHealth { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public EntityAnimationTrigger AnimationTrigger { get; private set; }

    public bool CanMove { get; private set; } = true;
    public bool IsShield { get; set; }
    public bool IsAttackAnimationEnd { get; set; }

    public EnemyBody(Enemy owner)
    {
        _owner = owner;
    }

    public void Initialize()
    {
        Rigidbody = _owner.GetComponent<Rigidbody2D>();
        Animator = _owner.GetComponentInChildren<Animator>();
        EntityHealth = _owner.GetCompo<EntityHealth>(true);
        AnimationTrigger = _owner.GetCompo<EntityAnimationTrigger>(true);

        float rawScaleX = _owner.transform.localScale.x;
        _baseScaleX = Mathf.Approximately(rawScaleX, 0f) ? 1f : Mathf.Abs(rawScaleX);
        CanMove = true;
    }

    public void BindCallbacks(Action onAnimationEnd, Action<Vector2> onKnockBack)
    {
        UnbindCallbacks();

        _onAnimationEnd = onAnimationEnd;
        _onKnockBack = onKnockBack;

        if (AnimationTrigger != null && _onAnimationEnd != null)
            AnimationTrigger.OnAnimationEnd += _onAnimationEnd;

        if (EntityHealth != null && _onKnockBack != null)
            EntityHealth.OnKnockback += _onKnockBack;
    }

    public void Dispose()
    {
        UnbindCallbacks();
        KillKnockBackRecoverTween();

        _onAnimationEnd = null;
        _onKnockBack = null;
    }

    private void UnbindCallbacks()
    {
        if (AnimationTrigger != null && _onAnimationEnd != null)
            AnimationTrigger.OnAnimationEnd -= _onAnimationEnd;

        if (EntityHealth != null && _onKnockBack != null)
            EntityHealth.OnKnockback -= _onKnockBack;
    }

    public void RotateTowards(Transform target)
    {
        if (target == null)
            return;

        float scaleX = target.position.x > _owner.transform.position.x ? _baseScaleX : -_baseScaleX;

        Vector3 currentScale = _owner.transform.localScale;
        _owner.transform.localScale = new Vector3(scaleX, currentScale.y, currentScale.z);
    }

    public void StopImmediately(bool includeY)
    {
        if (Rigidbody == null)
            return;

        if (includeY)
            Rigidbody.linearVelocity = Vector2.zero;
        else
            Rigidbody.linearVelocityX = 0f;
    }

    public void AddForce(Vector2 force)
    {
        if (Rigidbody == null)
            return;

        Rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    public void KnockBack(Vector2 force, float time)
    {
        CanMove = false;

        StopImmediately(true);
        AddForce(force);

        KillKnockBackRecoverTween();

        if (time <= 0f)
        {
            RecoverMovement();
            return;
        }

        _knockBackRecoverTween = DOVirtual.DelayedCall(time, RecoverMovement);
    }

    public void LockMass(float mass = 100f)
    {
        if (_isMassLocked || Rigidbody == null)
            return;

        _cachedMass = Rigidbody.mass;
        Rigidbody.mass = mass;
        _isMassLocked = true;
    }

    public void UnlockMass()
    {
        if (!_isMassLocked || Rigidbody == null)
            return;

        Rigidbody.mass = _cachedMass;
        _isMassLocked = false;
    }

    private void RecoverMovement()
    {
        _knockBackRecoverTween = null;
        CanMove = true;
    }

    private void KillKnockBackRecoverTween()
    {
        if (_knockBackRecoverTween == null)
            return;

        if (_knockBackRecoverTween.IsActive())
            _knockBackRecoverTween.Kill();

        _knockBackRecoverTween = null;
    }
}
}
