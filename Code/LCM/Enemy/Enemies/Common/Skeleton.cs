using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class Skeleton : CounterableEnemy
{
    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("skeleton.slash.a", EnemyStateType.Attack, ExecuteAttackA);
        registry.Add("skeleton.slash.b", EnemyStateType.Attack2, ExecuteAttackB);
    }

    private void ExecuteAttackA()
    {
        AudioManager.Instance.PlaySound2D("SkeletonAttack", 0.2f, false, SoundType.SfX);
    }

    private void ExecuteAttackB()
    {
        AudioManager.Instance.PlaySound2D("SkeletonAttack", 0f, false, SoundType.SfX);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        AudioManager.Instance.PlaySound2D("SkeletonDead", 0f, false, SoundType.SfX);
    }
}
}






