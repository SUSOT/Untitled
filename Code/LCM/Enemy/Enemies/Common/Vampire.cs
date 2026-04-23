using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections;
using UnityEngine;
using _01.Script.LCM.Enemy.Combat.Projectiles;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;

namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class Vampire : EnemyCore
{
    [SerializeField] private Transform _firePos;
    [SerializeField] private PoolItemSO _bullet1;
    [SerializeField] private float _projectileDelay = 0.5f;

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("vampire.projectile", EnemyStateType.Attack, ExecuteAttackA);
        registry.Add("vampire.slash", EnemyStateType.Attack2, ExecuteAttackB);
    }

    protected override void OnKnockBack(Vector2 knockBackForce)
    {
        KnockBack(knockBackForce, 0.5f);
    }

    private void ExecuteAttackA()
    {
        AudioManager.Instance.PlaySound2D("VampireAttack", 0f, false, SoundType.SfX);
        StartCoroutine(AttackCoroutine());
    }

    private void ExecuteAttackB()
    {
        AudioManager.Instance.PlaySound2D("VampireAttack2", 0f, false, SoundType.SfX);
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(_projectileDelay);

        BossBullet bullet = PoolManager.Instance.Pop(_bullet1.poolName) as BossBullet;
        if (bullet == null)
            yield break;

        bullet.transform.position = _firePos.position;
        bullet.Initialize(transform.localScale.x >= 0f ? Vector2.right : Vector2.left);
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        AudioManager.Instance.PlaySound2D("VampireDead", 0f, false, SoundType.SfX);
    }
}
}







