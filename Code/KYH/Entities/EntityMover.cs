using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class EntityMover : MonoBehaviour, IEntityComponent
{
    public UnityEvent<Vector2> OnVelocity;
    public UnityEvent<float> OnXInput;

    [Header("Collision detection")]
    [SerializeField] private Transform groundCheckTrm;
    [SerializeField] private Transform wallCheckTrm;
    [SerializeField] private float groundCheckDistance, groundBoxWidth, wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWallSlide;

    #region Member field

    [field : SerializeField] public float _moveSpeed { get; private set; }
    [field: SerializeField] public float _jumpPower { get; private set; }

    private float _movementX;
    private float _moveSpeedMultiplier;
    private float _originalGravityScale;
    private float _limitYSpeed = 40f;

    private Rigidbody2D _rbCompo;
    private Vector2 _colliderOffset, _colliderSize;

    [field : SerializeField] public EffectorPlayer EffectorPlayer { get; private set; }

    #endregion

    public bool CanManualMove { get; set; } = true; //�˹���ϰų� ������ �̵��Ұ�
    public CapsuleCollider2D BodyCollider { get; private set; }

    public void Initialize(Entity entity)
    {
        _rbCompo = entity.GetComponent<Rigidbody2D>();
        _moveSpeedMultiplier = 1f;
        _originalGravityScale = _rbCompo.gravityScale;
        BodyCollider = entity.GetComponent<CapsuleCollider2D>();
        _colliderOffset = BodyCollider.offset;
        _colliderSize = BodyCollider.size;
    }

    public void Jump() => AddForceToEntity(new Vector2(0, _jumpPower));

    public void AddForceToEntity(Vector2 force)
        => _rbCompo.AddForce(force, ForceMode2D.Impulse);

    public void SetMoveSpeedMultiplier(float value)
        => _moveSpeedMultiplier = value;
    public void SetGravityScale(float value)
        => _rbCompo.gravityScale = _originalGravityScale * value;
    public void SetLimitYSpeed(float value)
        => _limitYSpeed = value;

    public void SetColliderSize(Vector2 size, Vector2 offset)
    {
        BodyCollider.size = size;
        BodyCollider.offset = offset;
    }

    public void ResetColliderSize()
    {
        BodyCollider.size = _colliderSize;
        BodyCollider.offset = _colliderOffset;
    }


    private void FixedUpdate()
    {
        if (CanManualMove)
            _rbCompo.linearVelocityX = _movementX * _moveSpeed * _moveSpeedMultiplier;

        _rbCompo.linearVelocityY = Mathf.Clamp(_rbCompo.linearVelocityY, -_limitYSpeed, _limitYSpeed);
        OnVelocity?.Invoke(_rbCompo.linearVelocity);
    }

    public void SetMovementX(float xMovement)
    {
        _movementX = Mathf.Abs(xMovement) > 0 ? Mathf.Sign(xMovement) : 0;
        OnXInput?.Invoke(_movementX);
    }

    public void StopImmediately(bool isYAxisToo)
    {
        if (isYAxisToo)
            _rbCompo.linearVelocity = Vector2.zero;
        else
            _rbCompo.linearVelocityX = 0;

        _movementX = 0;
    }

    public void KnockBack(Vector2 force, float time)
    {
        CanManualMove = false;
        StopImmediately(true);
        AddForceToEntity(force);
        DOVirtual.DelayedCall(time, () => CanManualMove = true);
    }

    #region Check Collision

    public bool IsGroundDetected()
    {
        
        float boxHeight = 0.05f;
        Vector2 boxSize = new Vector2(groundBoxWidth, boxHeight);
        return Physics2D.BoxCast(groundCheckTrm.position, boxSize, 0, Vector2.down, groundCheckDistance, whatIsGround);
    }

    public bool IsWallDetected(float facingDirection)
        => Physics2D.Raycast(wallCheckTrm.position, Vector2.right * facingDirection, wallCheckDistance, whatIsWallSlide);

    public bool CheckColliderInFront(Vector2 dashDirection, float maxDistance, out float distance)
    {
        Bounds colliderBound = BodyCollider.bounds;
        Vector2 center = colliderBound.center;
        Vector2 size = colliderBound.size;
        size.y -= 0.2f; //�ٴڿ� ���� �ʵ��� ����� �ٿ��ش�.

        RaycastHit2D hit = Physics2D.BoxCast(center, size, 0, dashDirection, maxDistance, whatIsGround);

        distance = hit ? hit.distance : maxDistance;
        return hit;
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (groundCheckTrm != null)
        {
            Gizmos.DrawWireCube(groundCheckTrm.position - new Vector3(0, groundCheckDistance * 0.5f),
                            new Vector3(groundBoxWidth, groundCheckDistance, 1f));
        }

        if (wallCheckTrm != null)
            Gizmos.DrawLine(wallCheckTrm.position, wallCheckTrm.position + new Vector3(wallCheckDistance, 0));
    }
#endif


}

