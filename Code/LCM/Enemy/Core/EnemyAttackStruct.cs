using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System;
using UnityEngine;

namespace _01.Script.LCM.Enemy.Core
{
[Serializable]
public struct EnemyAttackStruct
{
    public float damage;
    public Vector2 force;
    public Vector2 attackBoxSize;
    public float attackRadius;
    public OverlapDamageCaster.OverlapCastType castType;
}
}







