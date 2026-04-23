using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;

namespace _01.Script.LCM.Enemy.Combat.Projectiles
{
public class BossSpinSword : Entity, IPoolable
{
    [SerializeField] private string _poolName;
    [SerializeField] private float _waitTime;
    [SerializeField] private float _returnDelay = 0.1f;
    [SerializeField] private ParticleSystem _spinEffect;

    public UnityEvent OnExplosion;

    public string PoolName => _poolName;
    public GameObject ObjectPrefab => gameObject;

    private Coroutine _attackCoroutine;

    private void OnEnable()
    {
        _attackCoroutine = StartCoroutine(SwordAttackCoroutine());
    }

    private void OnDisable()
    {
        if (_attackCoroutine == null)
            return;

        StopCoroutine(_attackCoroutine);
        _attackCoroutine = null;
    }

    private IEnumerator SwordAttackCoroutine()
    {
        yield return new WaitForSeconds(_waitTime);
        AudioManager.Instance.PlaySound2D("BossSpinSword", 0, false, SoundType.SfX);
        if (_spinEffect != null)
            _spinEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(_waitTime);
        OnExplosion?.Invoke();
        yield return new WaitForSeconds(_returnDelay);
        PoolManager.Instance.Push(this);
    }

    public void ResetItem()
    {
        if (_spinEffect != null)
            _spinEffect.gameObject.SetActive(false);
        _attackCoroutine = null;
    }

    protected override void HandleHit()
    {
    }

    protected override void HandleDead()
    {
    }
}
}







