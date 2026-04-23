using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core; using _01.Script.LCM.Enemy.StateMachine;




namespace _01.Script.LCM.Enemy.Enemies.Boss
{
public interface IKnightStateContext
{
    bool IsAttackAnimationEnd { get; }

    void TickMoveState();
    void TickRunState();

    void EnterShieldState();
    void ExitShieldState();

    void EnterPageTwoState();
    void ExitPageTwoState();

    void ChangeState(EnemyStateType nextState, bool forceRestart = false);
}
}






