using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.StateMachine.States
{
public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyCore enemy) : base(enemy, EnemyStateType.Idle)
    {
    }

    protected override void OnEnter()
    {
        Enemy.StopImmediately(true);
    }

    public override void Update()
    {
        Enemy.TargetingPlayer();

        if (Enemy.TargetTrm == null)
            return;

        Enemy.EnemyRotation();

        if (Enemy.TryTransitionToAttack())
            return;

        Enemy.TransitionState(EnemyStateType.Move);
    }
}
}






