using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.Enemies.Boss; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Boss.States
{
public class KnightRunState : EnemyState
{
    private readonly IKnightStateContext _stateContext;

    public KnightRunState(EnemyCore enemy, IKnightStateContext stateContext) : base(enemy, EnemyStateType.Run)
    {
        _stateContext = stateContext;
    }

    public override void FixedUpdate()
    {
        _stateContext.TickRunState();
    }
}
}






