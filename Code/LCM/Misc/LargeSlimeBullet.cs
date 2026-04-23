using UnityEngine.Events;
using UnityEngine;
using _01.Script.LCM.Enemy.Combat.Projectiles;

namespace _01.Script.LCM.Misc
{
public class LargeSlimeBullet : VerticalBullet
{
    [SerializeField] private float _explosionRadius;
    
    private readonly int _animHash = Animator.StringToHash("yMove");
    
    public UnityEvent OnExplode;
    
    private Animator _animator;

    [SerializeField] protected float _maxHeight = 10f; 
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        _animator.SetFloat(_animHash, _rbCompo.linearVelocityY);
    }
    
    
    public override void ThrowObject(Vector2 targetPosition)
    {
        Vector2 startPosition = transform.position;
        float distanceX = targetPosition.x - startPosition.x;
        float distanceY = targetPosition.y - startPosition.y;

        float effectiveGravity = Mathf.Abs(Physics2D.gravity.y * _rbCompo.gravityScale);

        
        float timeToPeak = Mathf.Sqrt(2 * _maxHeight / effectiveGravity);
        float totalFlightTime = timeToPeak + Mathf.Sqrt(2 * (_maxHeight - distanceY) / effectiveGravity);

        float velocityX = distanceX / totalFlightTime;
        float velocityY = effectiveGravity * timeToPeak;

        _rbCompo.linearVelocity = new Vector2(velocityX, velocityY);
    }
    protected override void RotateAlongTrajectory()
    {
        float angle;
        if (_rbCompo.linearVelocity != Vector2.zero)
        {
            if (_rbCompo.linearVelocityY > 0)
            {
                angle = Mathf.Atan2(_rbCompo.linearVelocityY, _rbCompo.linearVelocityX) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }
            else
            {
                angle = Mathf.Atan2(_rbCompo.linearVelocityY, _rbCompo.linearVelocityX) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("異⑸룎");
            OnExplode?.Invoke();
            var player = Physics2D.OverlapCircle(transform.position, _explosionRadius, _whatIsPlayer);
            if (player != null)
            {
                player.GetComponentInChildren<EntityHealth>().ApplyDamage(_damage, transform.position, _knockbackForce,false,null);
            }
            AudioManager.Instance.PlaySound2D("LargeSlimeBullet",0,false,SoundType.SfX);
            PoolManager.Instance.Push(this);
        }
        
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
#endif
}
}





