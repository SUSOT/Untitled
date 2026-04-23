using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.StateMachine.States; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class Gladiator : CounterableEnemy
{
    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("gladiator.attack", EnemyStateType.Attack, ExecuteAttack);
    }

    protected override void RegisterCustomStates(EnemyStateMachine machine)
    {
        base.RegisterCustomStates(machine);
        machine.Register(
            EnemyStateType.Spawn,
            new EnemyAnimationGateState(this, EnemyStateType.Spawn, EnemyStateType.Idle, isCanceled: () => IsDead, cancelFallbackState: EnemyStateType.Dead, timeoutSeconds: 5f, timeoutFallbackState: EnemyStateType.Idle)
        );
    }

    private void ExecuteAttack()
    {
        AudioManager.Instance.PlaySound2D("GladiatorAttack", 0f, false, SoundType.SfX);
    }

    public void Spawn()
    {
        TransitionState(EnemyStateType.Spawn);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        AudioManager.Instance.PlaySound2D("GladiatorDead", 0f, false, SoundType.SfX);
    }
}
}






