using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public class KnightStateContext : IKnightStateContext
{
    private readonly Knight _owner;
    private readonly KnightLocomotionController _locomotionController;
    private readonly KnightShieldController _shieldController;
    private readonly KnightPhaseController _phaseController;

    public bool IsAttackAnimationEnd => _owner.isAttackAnimationEnd;

    public KnightStateContext(
        Knight owner,
        KnightLocomotionController locomotionController,
        KnightShieldController shieldController,
        KnightPhaseController phaseController
    )
    {
        _owner = owner;
        _locomotionController = locomotionController;
        _shieldController = shieldController;
        _phaseController = phaseController;
    }

    public void TickMoveState()
    {
        _locomotionController.TickMove();
    }

    public void TickRunState()
    {
        _locomotionController.TickRun();
    }

    public void EnterShieldState()
    {
        _shieldController.EnterShieldState();
    }

    public void ExitShieldState()
    {
        _shieldController.ExitShieldState();
    }

    public void EnterPageTwoState()
    {
        _phaseController.EnterPageTwoState();
    }

    public void ExitPageTwoState()
    {
        _phaseController.ExitPageTwoState();
    }

    public void ChangeState(EnemyStateType nextState, bool forceRestart = false)
    {
        _owner.TransitionState(nextState, forceRestart);
    }
}
}






