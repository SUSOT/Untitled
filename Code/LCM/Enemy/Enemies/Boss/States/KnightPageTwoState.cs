using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.Enemies.Boss; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Boss.States
{
public class KnightPageTwoState : EnemyState
{
    private readonly IKnightStateContext _stateContext;

    public KnightPageTwoState(EnemyCore enemy, IKnightStateContext stateContext) : base(enemy, EnemyStateType.PageTwo)
    {
        _stateContext = stateContext;
    }

    protected override void OnEnter()
    {
        _stateContext.EnterPageTwoState();
    }

    protected override void OnExit()
    {
        _stateContext.ExitPageTwoState();
    }
}
}






