using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityRenderer : MonoBehaviour, IEntityComponent
{
    public SpriteRenderer SpriteRenderer { get; private set; }

    [field: SerializeField] public float FacingDirection { get; private set; } = 1f;
    public event Action<bool> OnFlip;
    private Entity _entity;
    private Animator _animator;
    public bool isForceSwap { get; set; }

    public void Method()
    {
        int[] arr = { 11, 22, 33, 44, 55, 66, 77 };

        foreach(int haha in arr)
        {
            Debug.Log($"{haha}");
        }
    }

    public void Initialize(Entity entity)
    {
        _entity = entity;
        _animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetParam(AnimParamSO param, bool value) => _animator.SetBool(param.hashValue,value);
    public void SetParam(AnimParamSO param, int value) => _animator.SetInteger(param.hashValue, value);
    public void SetParam(AnimParamSO param, float value) => _animator.SetFloat(param.hashValue, value);
    public void SetParam(AnimParamSO param) => _animator.SetTrigger(param.hashValue);

    public void FlipController(float xVelocity)
    {
        float xMove = Mathf.Approximately(xVelocity, 0) ? 0 : Mathf.Sign(xVelocity);
        if(Mathf.Abs(xMove + FacingDirection) < 0.5f)
        {
            OnFlip?.Invoke(Mathf.Abs(xMove + FacingDirection) < 0.5f);
            Flip();
        }
    }

    public void SeeRightDirection()
    {
        FacingDirection = 1f;
        isForceSwap = true;
    }

    private void Flip()
    {
        FacingDirection *= -1;
        _entity.transform.Rotate(0,180f,0);
    }
}
