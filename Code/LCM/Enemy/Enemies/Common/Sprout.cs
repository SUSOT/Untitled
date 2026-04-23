using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class Sprout : CounterableEnemy
{
    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("sprout.attack", EnemyStateType.Attack, ExecuteAttack);
    }

    protected override bool CanFinishAttackState(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack)
            return isAttackAnimationEnd && GroundCheck();

        return base.CanFinishAttackState(attackType);
    }

    private void ExecuteAttack()
    {
        AudioManager.Instance.PlaySound2D("SproutAttack", 0f, false, SoundType.SfX);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        AudioManager.Instance.PlaySound2D("SproutDead", 0f, false, SoundType.SfX);
    }
}
}






