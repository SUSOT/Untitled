using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.Enemies.Boss; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Boss.States
{
public class KnightShieldState : EnemyState
{
    private readonly IKnightStateContext _stateContext;

    public KnightShieldState(EnemyCore enemy, IKnightStateContext stateContext) : base(enemy, EnemyStateType.Shield)
    {
        _stateContext = stateContext;
    }

    protected override void OnEnter()
    {
        _stateContext.EnterShieldState();
    }

    public override void Update()
    {
        if (_stateContext.IsAttackAnimationEnd)
            _stateContext.ChangeState(EnemyStateType.Move);
    }

    protected override void OnExit()
    {
        _stateContext.ExitShieldState();
    }
}
}






