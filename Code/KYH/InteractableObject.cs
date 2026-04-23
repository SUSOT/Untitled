using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using _01.Script.LCM.Item;

public class InteractableObject : Entity, ITakeable
{

    [SerializeField] private float _playerCheckRadius;

    [SerializeField] private LayerMask _whatIsPlayer;

    [SerializeField] private GameObject _interactionKey;
    private Rigidbody2D RbCompo;


    [Header("앱 설정값")]
    public string appName;
    public string appDescript;
    public UnityEvent<string,string> OnMessageInteractable;
    public UnityEvent OnDontMessageInteractable;

    public bool isDontKnockBack;

    protected override void Awake()
    {
        base.Awake();
        RbCompo = GetComponent<Rigidbody2D>();
    }

    protected override void AfterInitialize()
    {
        base.AfterInitialize();
        if(!isDontKnockBack)
        GetCompo<EntityHealth>().OnKnockback += HandleKnockBack;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (!isDontKnockBack)
            GetCompo<EntityHealth>().OnKnockback -= HandleKnockBack;
    }

    private void HandleKnockBack(Vector2 knockBackForce)
    {
        float knockBackTime = 0.5f;
        KnockBack(knockBackForce, knockBackTime);
    }

    public void AddForceToEntity(Vector2 force)
            => RbCompo.AddForce(force, ForceMode2D.Impulse);
   
    private void KnockBack(Vector2 knockBackForce, float knockBackTime)
    {
        AddForceToEntity(knockBackForce);
    }


    public void TakeItem()
    {
        _interactionKey.SetActive(false);
        OnMessageInteractable?.Invoke(appName,appDescript);
        OnDontMessageInteractable?.Invoke();
    }

    public void ShowInteraction()
    {
        if (CanTakeItem())
        {
            _interactionKey.SetActive(true);
        }
        else
        {
            _interactionKey.SetActive(false);
        }
    }

    private bool CanTakeItem()
    {
        return Physics2D.OverlapCircle(transform.position, _playerCheckRadius, _whatIsPlayer);
    }

    private void Update()
    {
        if (CanTakeItem() && Keyboard.current.fKey.wasPressedThisFrame)
        {
            TakeItem();
        }
        ShowInteraction();
    }

    protected override void HandleHit()
    {
    }

    protected override void HandleDead()
    {
        print("");
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerCheckRadius);
    }
#endif
}
