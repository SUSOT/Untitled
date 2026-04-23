using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class Bear : CounterableEnemy
{
    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("bear.attack.a", EnemyStateType.Attack, ExecuteAttackA);
        registry.Add("bear.attack.b", EnemyStateType.Attack2, ExecuteAttackB);
    }

    private void ExecuteAttackA()
    {
        AudioManager.Instance.PlaySound2D("BearAttack", 0f, false, SoundType.SfX);
    }

    private void ExecuteAttackB()
    {
        AudioManager.Instance.PlaySound2D("BearAttack", 0f, false, SoundType.SfX);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        AudioManager.Instance.PlaySound2D("BearDead", 0f, false, SoundType.SfX);
    }
}
}






