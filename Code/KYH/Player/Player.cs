using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using _01.Script.LCM.Item;

public class Player : Entity
{
    [field: SerializeField] public GameEventChannelSO PlayerChannel { get; private set; }
    [field: SerializeField] public InputReader PlayerInput { get; private set; }
    private StateMachine _stateMachine;

    [field: SerializeField] public AnimParamSO ComboCounterParam { get; private set; }

    [SerializeField] private StateListSO playerFSM;
    private EntityMover _mover;

    public GameObject attackLockIcon;

    [field: SerializeField] public bool isBannedAttack { get; set; } = false;
    [field: SerializeField] public bool isLockedWindow { get; set; } = false;
    [field: SerializeField] public bool isDialogue { get; set; } = false;

    [field: SerializeField] public int MaxJumpCount { get; set; }
    private int _currentJumpCount;
    public bool CanJump => _currentJumpCount > 0;

    public int TipCount { get; set; }

    [field: SerializeField] public float MaxDashCoolTime { get; set; }

    [SerializeField] private CanvasGroup dashCanvasGroup;
    [SerializeField] private CanvasGroup hpBarCanvasGroup;


    private EntityHealth _healthCompo;

    protected override void Awake()
    {
        base.Awake();
        _stateMachine = new StateMachine(this, playerFSM);
        _healthCompo = GetCompo<EntityHealth>();
    }

    private void Start()
    {
        _stateMachine.ChangeState("IDLE");
    }

    public void BannedAttack(bool isValue)
    {
        print("占쏙옙占쏙옙");
        isBannedAttack = isValue;
    }

    private void Update()
    {
        _stateMachine.UpdateStateMachine();
    }

    private void HandleAnimationEnd()
    {
        _stateMachine.CurrentState.AnimationEndTrigger();
    }

    public void ChangeState(string newState) => _stateMachine.ChangeState(newState);

    public bool MoveStopOrGo(bool isMove) => _mover.CanManualMove = isMove;

    protected override void OnDestroy()
    {
        GetCompo<EntityAnimationTrigger>().OnAnimationEnd -= HandleAnimationEnd;
        GetCompo<EntityHealth>().OnKnockback -= HandleKnockBack;
        PlayerInput.ClearSubscription();
        GetCompo<EntityRenderer>().OnFlip -= Flip;
    }

    public void DecreaseJumpCount() => _currentJumpCount--;
    public void ResetJumpCount() => _currentJumpCount = MaxJumpCount;

    protected override void AfterInitialize()
    {
        base.AfterInitialize();
        _mover = GetCompo<EntityMover>();
        GetCompo<EntityHealth>().OnKnockback += HandleKnockBack;
        GetCompo<EntityAnimationTrigger>().OnAnimationEnd += HandleAnimationEnd;
        GetCompo<EntityRenderer>().OnFlip += Flip;
        _currentJumpCount = MaxJumpCount;
    }

    private void Flip(bool LeftOrRight)
    {
        if (LeftOrRight)
        {
            hpBarCanvasGroup.transform.Rotate(0, 180f, 0);
            dashCanvasGroup.transform.Rotate(0, 180f, 180f);
        }
        else
        {
            hpBarCanvasGroup.transform.Rotate(0, 0, 0);
            dashCanvasGroup.transform.Rotate(0, 0, 0);
        }
    }

    protected override void HandleHit()
    {
    }

    private void HandleKnockBack(Vector2 knockBackForce)
    {
        float knockBackTime = 0.5f;
        _mover.KnockBack(knockBackForce, knockBackTime);
    }


    protected override void HandleDead()
    {
        if (IsDead) return;
        gameObject.layer = DeadBodyLayer;
        IsDead = true;
        print("占쏙옙 占쏙옙占쏙옙");
        _stateMachine.ChangeState("DEAD");
    }

    #region 占쎈쉬占쏙옙타占쏙옙

    private bool isCounting;

    private float targetTime;

    [SerializeField] private Slider _dashSlider;
    [SerializeField] private Slider _backSlider;

    public void SetCountTime(int time)
    {
        MaxDashCoolTime = time;
    }

    public void TurnOnTimer()
    {
        dashCanvasGroup.DOFade(1, 0.5f);
        isCounting = true;
    }

    private void FixedUpdate()
    {
        if (isCounting)
        {
            float diffSec = targetTime += Time.deltaTime;
            if (diffSec < MaxDashCoolTime)
            {
                CoolTimeSecondValueChangeSlider(diffSec);
            }
            else
            {
                targetTime = 0f;
                isCounting = false;
                dashCanvasGroup.DOFade(0, 0.5f);
            }
        }
    }

    public bool IsCount()
    {
        return isCounting;
    }

    public void CoolTimeSecondValueChangeSlider(float second)
    {
        if (second <= 0f) return;

        _dashSlider.value = second;

        if (_backSlider != null && _backSlider.value > _dashSlider.value)
        {
            DOTween.Sequence()
                .AppendInterval(1f)
                .Append(_backSlider.DOValue(second, 0.5f).SetEase(Ease.OutCubic));
        }
        else if (_backSlider != null && _backSlider.value < _dashSlider.value)
        {
            _backSlider.value = second;
        }
    }

    #endregion

    private bool _isShield;
    private bool _isInvincibility;

    #region TakeItemFunc

    public void TakeItem(ItemType itemType, float healAmount)
    {
        if (itemType == ItemType.Heal)
        {
            _mover.EffectorPlayer.PlayEffect("GreenHealEffect", false);
            _healthCompo.TakeHeal(healAmount);
        }
        else if (itemType == ItemType.Shield)
        {
            if (!_isShield)
            {
                StartCoroutine(ShieldCoroutine());
            }
        }
        else if (itemType == ItemType.MoreHeal)
        {
            _mover.EffectorPlayer.PlayEffect("RedHealEffect", false);
            _healthCompo.TakeHeal(healAmount);
        }
        else
        {
            if (!_isInvincibility)
            {
                StartCoroutine(InvincibilityCoroutine());
            }
        }
    }


    private float _shieldCoolTime = 10f;

    private IEnumerator ShieldCoroutine()
    {
        _isShield = true;
        _mover.EffectorPlayer.PlayEffect("MagicShieldBlue");
        _healthCompo.IsShield = true;
        yield return new WaitForSeconds(_shieldCoolTime);
        _mover.EffectorPlayer.StopEffect("MagicShieldBlue");
        _healthCompo.IsShield = false;
        _isShield = false;
    }

    private float _invincibilityCoolTime = 5f;

    private IEnumerator InvincibilityCoroutine()
    {
        _isInvincibility = true;
        _mover.EffectorPlayer.PlayEffect("InvincibilityBuffEffect");
        _healthCompo.IsInvincibility = true;
        yield return new WaitForSeconds(_invincibilityCoolTime);
        _isInvincibility = false;
        _mover.EffectorPlayer.StopEffect("InvincibilityBuffEffect");
        _healthCompo.IsInvincibility = false;
    }

    #endregion
}
