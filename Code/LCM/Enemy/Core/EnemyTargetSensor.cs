using UnityEngine;

namespace _01.Script.LCM.Enemy.Core
{
public class EnemyTargetSensor
{
    private readonly Transform _owner;
    private readonly Collider2D[] _targetBuffer = new Collider2D[16];
    private ContactFilter2D _targetFilter;

    public EnemyTargetSensor(Transform owner)
    {
        _owner = owner;
        _targetFilter = new ContactFilter2D
        {
            useLayerMask = true
        };
    }

    public Transform FindClosestTarget(float range, LayerMask targetLayer)
    {
        int targetCount = CollectTargets(range, targetLayer);
        if (targetCount <= 0)
            return null;

        Transform closest = null;
        float minDistanceSq = float.PositiveInfinity;

        for (int i = 0; i < targetCount; i++)
        {
            Collider2D collider = _targetBuffer[i];
            if (collider == null)
                continue;

            Vector2 delta = collider.transform.position - _owner.position;
            float distanceSq = delta.sqrMagnitude;
            if (distanceSq >= minDistanceSq)
                continue;

            minDistanceSq = distanceSq;
            closest = collider.transform;
        }

        return closest;
    }

    public Transform AcquireTarget(Transform currentTarget, float range, LayerMask targetLayer)
    {
        if (IsCurrentTargetValid(currentTarget, range, targetLayer))
            return currentTarget;

        return FindClosestTarget(range, targetLayer);
    }

    public bool HasTargetInRange(float range, LayerMask targetLayer)
    {
        return CollectTargets(range, targetLayer) > 0;
    }

    public bool IsCurrentTargetValid(Transform target, float range, LayerMask targetLayer)
    {
        if (target == null)
            return false;

        Vector2 direction = target.position - _owner.position;
        if (direction.sqrMagnitude > range * range)
            return false;

        int layerMask = 1 << target.gameObject.layer;
        return (targetLayer.value & layerMask) != 0;
    }

    private int CollectTargets(float range, LayerMask targetLayer)
    {
        if (range <= 0f)
            return 0;

        _targetFilter.SetLayerMask(targetLayer);
        return Physics2D.OverlapCircle(_owner.position, range, _targetFilter, _targetBuffer);
    }
}
}
