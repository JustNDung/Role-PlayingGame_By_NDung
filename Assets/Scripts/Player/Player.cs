using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{  
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public bool isBusy {get; private set;}

    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce = 16f;

    [Header("Dash info")]
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttackState { get; private set; }
    [SerializeField] private String idle = "Idle";
    [SerializeField] private String move = "Move"; 
    [SerializeField] private String jump = "Jump";
    [SerializeField] private String dash = "Dash";
    [SerializeField] private String wallSlide = "WallSlide";
    [SerializeField] private String primaryAttack = "Attack";

    #endregion
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, idle);
        moveState = new PlayerMoveState(this, stateMachine, move);
        jumpState = new PlayerJumpState(this, stateMachine, jump);
        airState = new PlayerAirState(this, stateMachine, jump);
        dashState = new PlayerDashState(this, stateMachine, dash);
        wallSlideState = new PlayerWallSlideState(this, stateMachine, wallSlide);
        wallJumpState = new PlayerWallJumpState(this, stateMachine, jump);

        primaryAttackState = new PlayerPrimaryAttackState(this, stateMachine, primaryAttack);
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CheckForDashInput();
    }
    public IEnumerator BusyFor(float duration) {
        isBusy = true;
        yield return new WaitForSeconds(duration);
        isBusy = false;
    }
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();
    private void CheckForDashInput() {
        if (IsWallDetected()) {
            return;
        }
        dashTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashTimer < 0) {
            dashTimer = dashCooldown;
            dashDir = Input.GetAxisRaw("Horizontal");
            if (dashDir == 0) {
                dashDir = facingDirection;
            }
            stateMachine.ChangeState(dashState);
        }
    }
}
