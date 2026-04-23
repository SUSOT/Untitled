using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;

namespace _01.Script.LCM.Enemy.Core
{
public class EnemyAttackCooldown
{
    private float _lastAttackTime = float.NegativeInfinity;
    private bool _isTargetInRange;

    public void UpdateRangeState(bool isTargetInRange, float currentTime)
    {
        if (_isTargetInRange == isTargetInRange)
            return;

        _isTargetInRange = isTargetInRange;
        if (_isTargetInRange)
            _lastAttackTime = currentTime;
    }

    public bool IsReady(float currentTime, float coolTime)
    {
        return _isTargetInRange && currentTime >= _lastAttackTime + coolTime;
    }

    public void Consume(float currentTime)
    {
        _isTargetInRange = true;
        _lastAttackTime = currentTime;
    }
}
}







