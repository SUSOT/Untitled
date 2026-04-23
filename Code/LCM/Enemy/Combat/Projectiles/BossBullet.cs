using EnemyCore = _01.Script.LCM.Enemy.Core.Enemy;
using UnityEngine.Events;
using UnityEngine;
using _01.Script.LCM.Enemy.Core;

namespace _01.Script.LCM.Enemy.Combat.Projectiles
{
public class BossBullet : MonoBehaviour, IPoolable
{
    [SerializeField] private string _poolName;
    public string PoolName => _poolName;
    public GameObject ObjectPrefab => gameObject;

    private Rigidbody2D _rigidbody2D;
    [SerializeField] private float _speed;

    [SerializeField] private float _bulletLifeTime;
    private float _curTime;

    [SerializeField] private float _damage;
    [SerializeField] private Vector2 _knockbackForce;

    [SerializeField] private string _bulletAudio;

    private Vector2 _moveDir;
    private bool _isReturned;

    public UnityEvent OnDeadEvent;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 moveDir)
    {
        _moveDir = moveDir;
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = _moveDir * _speed;
    }

    private void Update()
    {
        _curTime += Time.deltaTime;
        if (_curTime >= _bulletLifeTime)
            ReturnToPool();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            EntityHealth health = other.gameObject.GetComponentInChildren<EntityHealth>();
            if (health != null)
                health.ApplyDamage(_damage, transform.position, _knockbackForce, false, null);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_isReturned)
            return;

        _isReturned = true;

        OnDeadEvent?.Invoke();
        if (!string.IsNullOrEmpty(_bulletAudio))
            AudioManager.Instance.PlaySound2D(_bulletAudio, 0, false, SoundType.SfX);

        PoolManager.Instance.Push(this);
    }

    public void ResetItem()
    {
        _curTime = 0;
        _isReturned = false;
    }
}
}







