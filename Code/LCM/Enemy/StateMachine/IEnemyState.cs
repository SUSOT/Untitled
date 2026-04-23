using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;using _01.Script.LCM.Enemy.Core;




namespace _01.Script.LCM.Enemy.StateMachine
{
public interface IEnemyState
{
    void Enter();
    void Update();
    void FixedUpdate();
    void Exit();
}
}






