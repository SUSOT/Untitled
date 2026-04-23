using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;

namespace _01.Script.LCM.Enemy.Core
{
public abstract class CounterableEnemy : EnemyCore, ICounterable
{
    public bool CanCounter { get; private set; }

    protected override void OnEnemyAwakeCompleted()
    {
        base.OnEnemyAwakeCompleted();

        if (AnimTriggerCompo != null)
            AnimTriggerCompo.OnCounterStatusChange += SetCounterStatus;
    }

    protected override void OnDestroy()
    {
        if (AnimTriggerCompo != null)
            AnimTriggerCompo.OnCounterStatusChange -= SetCounterStatus;

        base.OnDestroy();
    }

    protected override void OnKnockBack(Vector2 knockBackForce)
    {
        KnockBack(knockBackForce, 0.5f);
    }

    public virtual void ApplyCounter(float damage, Vector2 direction, Vector2 knockBackForce, bool isPowerAttack, Entity dealer)
    {
        CanCounter = false;

        if (EntityHealth != null)
            EntityHealth.ApplyDamage(damage, direction, knockBackForce, isPowerAttack, dealer);
    }

    private void SetCounterStatus(bool canCounter)
    {
        CanCounter = canCounter;
    }
}
}







