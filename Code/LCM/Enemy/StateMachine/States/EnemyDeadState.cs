using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.StateMachine.States
{
public class EnemyDeadState : EnemyState
{
    public EnemyDeadState(EnemyCore enemy) : base(enemy, EnemyStateType.Dead)
    {
    }

    protected override void OnEnter()
    {
        Enemy.StopImmediately(true);
    }
}
}






