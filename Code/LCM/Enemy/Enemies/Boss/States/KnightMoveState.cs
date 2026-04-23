using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.Enemies.Boss; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Boss.States
{
public class KnightMoveState : EnemyState
{
    private readonly IKnightStateContext _stateContext;

    public KnightMoveState(EnemyCore enemy, IKnightStateContext stateContext) : base(enemy, EnemyStateType.Move)
    {
        _stateContext = stateContext;
    }

    public override void FixedUpdate()
    {
        _stateContext.TickMoveState();
    }
}
}






