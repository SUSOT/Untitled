using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine;

namespace _01.Script.LCM.Enemy.Core
{
[CreateAssetMenu(menuName = "SO/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Movement")]
    public float movementSpeed;

    public Vector2 groundCheckerBoxSize;
    
    
    [Header("Combat")]
    public LayerMask whatIsPlayer;
    public float targetingRange;
    public float attackRange;
    public float attackCoolTime;
}
}







