using System;
using UnityEngine;
using _01.Script.LCM.Enemy.Combat.Projectiles;

namespace _01.Script.LCM.Misc
{
public class MarksmanBullet : VerticalBullet
{
    [SerializeField] float totalFlightTime = 1.0f;
    public override void ThrowObject(Vector2 targetPosition)
    {
        Vector2 startPosition = transform.position;
        float distanceX = targetPosition.x - startPosition.x;
        float distanceY = targetPosition.y - startPosition.y;

        float effectiveGravity = Mathf.Abs(Physics2D.gravity.y * _rbCompo.gravityScale);

        float velocityX = distanceX / totalFlightTime;
        float velocityY = (distanceY + 0.5f * effectiveGravity * Mathf.Pow(totalFlightTime, 2)) / totalFlightTime;

        _rbCompo.linearVelocity = new Vector2(velocityX, velocityY);
    }
    protected override void RotateAlongTrajectory()
    {
        float angle;
        if (_rbCompo.linearVelocity != Vector2.zero)
        {
            angle = Mathf.Atan2(_rbCompo.linearVelocityY, _rbCompo.linearVelocityX) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.gameObject.GetComponentInChildren<EntityHealth>()
                .ApplyDamage(_damage, transform.position, _knockbackForce, false, null);

            PoolManager.Instance.Push(this);
        }
        else
        {
            PoolManager.Instance.Push(this);
        }
    }
}
}





