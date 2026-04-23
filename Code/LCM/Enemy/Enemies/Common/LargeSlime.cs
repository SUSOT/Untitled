using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections;
using UnityEngine;
using _01.Script.LCM.Enemy.Combat.Projectiles;
using _01.Script.LCM.Enemy.Core;
using _01.Script.LCM.Enemy.StateMachine;
using _01.Script.LCM.Misc;

namespace _01.Script.LCM.Enemy.Enemies.Common
{
public class LargeSlime : EnemyCore
{
    [SerializeField] private PoolItemSO _verticalBullet;
    [SerializeField] private PoolItemSO _horizontalBullet;
    [SerializeField] private Transform _firePos;
    [SerializeField] private int _attack2ShotCount = 2;
    [SerializeField] private float _attack2ShotInterval = 0.5f;

    protected override void ConfigureAttacks(EnemyAttackRegistry registry)
    {
        registry.Add("largeslime.vertical", EnemyStateType.Attack, ExecuteAttackA);
        registry.Add("largeslime.horizontal", EnemyStateType.Attack2, ExecuteAttackB);
    }

    protected override void OnEnemyAwakeCompleted()
    {
        base.OnEnemyAwakeCompleted();

        if (AnimTriggerCompo != null)
            AnimTriggerCompo.OnAttackTrigger += VerticalBulletFire;
    }

    protected override void OnDestroy()
    {
        if (AnimTriggerCompo != null)
            AnimTriggerCompo.OnAttackTrigger -= VerticalBulletFire;

        base.OnDestroy();
    }

    protected override void OnKnockBack(Vector2 knockBackForce)
    {
        KnockBack(knockBackForce, 0.5f);
    }

    private void ExecuteAttackA()
    {
        LockMass();
    }

    private void ExecuteAttackB()
    {
        LockMass();
        StartCoroutine(Attack2Coroutine());
    }

    protected override void OnAttackAnimationCompleted(EnemyStateType attackType)
    {
        if (attackType == EnemyStateType.Attack || attackType == EnemyStateType.Attack2)
            UnlockMass();

        base.OnAttackAnimationCompleted(attackType);
    }

    private void VerticalBulletFire()
    {
        if (TargetTrm == null)
            return;

        LargeSlimeBullet bullet = PoolManager.Instance.Pop(_verticalBullet.poolName) as LargeSlimeBullet;
        if (bullet == null)
            return;

        bullet.transform.position = transform.position;
        bullet.ThrowObject(TargetTrm.position);

        AudioManager.Instance.PlaySound2D("LargeSlimeAttack", 0f, false, SoundType.SfX);
    }

    private IEnumerator Attack2Coroutine()
    {
        for (int i = 0; i < _attack2ShotCount; i++)
        {
            yield return new WaitForSeconds(_attack2ShotInterval);

            BossBullet bullet = PoolManager.Instance.Pop(_horizontalBullet.poolName) as BossBullet;
            if (bullet == null)
                continue;

            bullet.transform.position = _firePos.position;
            bullet.Initialize(transform.localScale.x >= 0f ? Vector2.right : Vector2.left);

            AudioManager.Instance.PlaySound2D("LargeSlimeAttack2", 0f, false, SoundType.SfX);
        }
    }

    public override void Dead()
    {
        if (!BeginDeadState())
            return;

        UnlockMass();
        AudioManager.Instance.PlaySound2D("LargeSlimeDead", 0f, false, SoundType.SfX);
    }
}
}







