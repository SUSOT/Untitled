using System;
using UnityEngine;

public class EnemyAttackCompo : MonoBehaviour,IEntityComponent,IAfterInit
{
    [SerializeField] private DamageCaster damageCaster;
    private EntityAnimationTrigger _triggerCompo;
    private OverlapDamageCaster _overlapDamageCaster;

    [SerializeField]
    private float damage;
    [SerializeField]
    private Vector2 knockBackForce;

    public void Initialize(Entity entity)
    {
        _triggerCompo = entity.GetCompo<EntityAnimationTrigger>();
        damageCaster.InitCaster(entity);
        _overlapDamageCaster = GetComponentInChildren<OverlapDamageCaster>();
    }

    public void AfterInitialize()
    {
        _triggerCompo.OnAttackTrigger += HandleAttackTrigger;
    }

    private void OnDestroy()
    {
        _triggerCompo.OnAttackTrigger -= HandleAttackTrigger;
    }

    public void AttackSetting(float atkDamage, Vector2 force, Vector2 boxSize, float radius, OverlapDamageCaster.OverlapCastType type)
    {
        damage = atkDamage;
        knockBackForce = force;
        _overlapDamageCaster.CasterSizeSetting(boxSize, radius, type);
    }

    private void HandleAttackTrigger()
    {
        bool success = damageCaster.CastDamage(damage, knockBackForce, false);
        if (success)
        {
            Debug.Log($"<color=red>Damaged! - {damage}</color>");
        }
    }
}
