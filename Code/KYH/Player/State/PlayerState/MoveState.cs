using UnityEngine;

public class MoveState : PlayerGroundState
{
    private float _stateTimer;

    public MoveState(Entity entity, AnimParamSO animParam) : base(entity, animParam)
    {
    }
    public override void Enter()
    {
        base.Enter();
        _stateTimer = Time.time;
    }

    public override void Update()
    {
        base.Update();

        float xInput = _player.PlayerInput.InputDirection.x;

        if(_mover.CanManualMove && !_player.isDialogue)
            _mover.SetMovementX(xInput);

        if (Mathf.Approximately(xInput, 0) || _mover.IsWallDetected(_renderer.FacingDirection) || !_mover.CanManualMove)
        {
            _player.ChangeState("IDLE");
        }
    }

    protected override void HandleAttackKeyPress()
    {
        float overDashTime = 0.3f;
        if (_stateTimer + overDashTime < Time.time && !_player.isBannedAttack && !_player.isDialogue && !_player.IsCount())
        {
            _player.ChangeState("DASH_ATTACK");
        }
        else
        {
            base.HandleAttackKeyPress();
        }
    }
}
