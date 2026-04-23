using UnityEngine;

public class CounterAttackState : EntityState
{
    private Player _player;
    private PlayerAttackCompo _attackCompo;
    private EntityMover _mover;

    private float _counterTimer;
    private bool _counterSuccess;

    private EntityHealth _health;

    public CounterAttackState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
    {
        _player = entity as Player;
        _attackCompo = entity.GetCompo<PlayerAttackCompo>();
        _mover = entity.GetCompo<EntityMover>();
        _health = entity.GetCompo<EntityHealth>();
    }

    public override void Enter()
    {
        base.Enter();
        _mover.StopImmediately(false);
        _counterTimer = _attackCompo.counterAttackDuration;
        _renderer.SetParam(_attackCompo.successCounterParam, false);
        _counterSuccess = false;
        _mover.EffectorPlayer.PlayEffect("CounterWait");
        _health.IsCounter = true;
        AudioManager.Instance.PlaySound2D("Parring", 0,false,SoundType.SfX);
    }

    public override void Update()
{
    base.Update();
    _counterTimer -= Time.deltaTime;
    if (_counterSuccess == false)
        CheckCounter();

    if (_counterTimer < 0 || _isTriggerCall)
    {
            _mover.EffectorPlayer.StopEffect("CounterWait");
        _player.ChangeState("IDLE");
        _health.IsCounter = false;
        }
    }

    private void CheckCounter()
    {
        ICounterable countable = _attackCompo.GetCounterableTargetInRadius();

        if (countable is { CanCounter: true })
        {
            _mover.EffectorPlayer.StopEffect("CounterWait");
            _mover.EffectorPlayer.PlayEffect("CounterEffect", true);
            AudioManager.Instance.PlaySound2D("CounterAttack", 0, false, SoundType.SfX);
            _counterSuccess = true;
            AttackDataSO attackData = _attackCompo.GetAttackData("PlayerCounterAttack");
            float damage = attackData.attackDamage; //하드코딩
            Vector2 attackDirection = new Vector2(_renderer.FacingDirection, 0);
            Vector2 knockBackForce = attackData.knockBackForce;
            knockBackForce.x *= _renderer.FacingDirection;

            countable.ApplyCounter(damage, attackDirection, knockBackForce, attackData.isPowerAttack, _player);
            _renderer.SetParam(_attackCompo.successCounterParam, true);

            //카운터 성공메시지 보낸다.
            CounterSuccessEvent counterEvt = PlayerEvents.CounterSuccessEvent;
            counterEvt.target = countable.TargetTrm;
            _player.PlayerChannel.RaiseEvent(counterEvt);
            _health.IsCounter = false;
        }
    }
}