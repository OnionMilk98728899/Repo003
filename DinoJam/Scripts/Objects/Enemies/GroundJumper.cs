using Godot;
using System;

public partial class GroundJumper : Enemy
{
    [Export] private RayCast2D frontFloorChecker, backFloorChecker, frontWallChecker, backWallChecker, frontLedgeChecker, backLedgeChecker;
    [Export] private Timer turnBuffer, jumpTimer, jumpBuffer;
    [Export] private float moveSpeed, jumpPower;
    [Export] private Label debugLabel;
    private int direction = 1;
    private bool isJumping, hasJumped, isReadyToAttack;
    public override void _Ready()
    {
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        DetermineBehavior();
        AnimateEnemy();
        ApplyGravity();
        Velocity = enemyVelocity;
        MoveAndSlide();
        //debugLabel.Text = currentMoveState.ToString();
    }

    private void DetermineBehavior()
    {
        switch (currentMoveState)
        {
            case enemyMoveState.move:
                HandleMovement();
                break;
            case enemyMoveState.jump:
                HandleJumping();
                break;
            case enemyMoveState.prepare:
                enemyVelocity.X = 0;
                break;
            case enemyMoveState.stun:
                enemyVelocity.X = 0;
                break;
            case enemyMoveState.attack:
                enemyVelocity = new Vector2(attacker.Attack(myPlayer.GlobalPosition, enemyVelocity).X, 0);
                break;
            case enemyMoveState.hurt:

                break;
            case enemyMoveState.dying:
                enemyVelocity = death.Death(enemyVelocity);
                break;
        }
    }

    private void HandleJumping()
    {
        if (!hasJumped)
        {
            enemyVelocity.Y -= jumpPower;
            hasJumped = true;
            jumpTimer.Start();
        }
        if (isReadyToAttack && IsOnFloor())
        {
            currentMoveState = enemyMoveState.attack;
            isReadyToAttack = false;
        }
        if (IsOnFloor() && jumpTimer.TimeLeft < jumpTimer.WaitTime * .8)
        {
            currentMoveState = enemyMoveState.move;
            isBounced = false;
            hasJumped = false;
        }
    }

    private void HandleMovement()
    {
        enemyVelocity = new Vector2(direction * moveSpeed, enemyVelocity.Y);

        if (turnBuffer.IsStopped())
        {
            if (frontWallChecker.IsColliding() && frontLedgeChecker.IsColliding() || backWallChecker.IsColliding() && backLedgeChecker.IsColliding())
            {
                direction *= -1;
                turnBuffer.Start();
            }
        }
        if (IsOnFloor() && jumpBuffer.IsStopped())
        {

            if (enemyVelocity.X < 0 && !frontFloorChecker.IsColliding() || enemyVelocity.X > 0 && !backFloorChecker.IsColliding()
            || frontWallChecker.IsColliding() && !frontLedgeChecker.IsColliding() || backWallChecker.IsColliding() && !backLedgeChecker.IsColliding())
            {
                currentMoveState = enemyMoveState.jump;
                isBounced = true;
                jumpBuffer.Start();
            }
        }
    }

    public override void InitiateAttack()
    {
        if (IsOnFloor())
        {
            currentMoveState = enemyMoveState.attack;
        }
        else
        {
            isReadyToAttack = true;
        }
    }

    private void OnJumpTimerTimeout()
    {
        // currentMoveState = enemyMoveState.move;
        // isBounced = false;
        // hasJumped = false;
    }

    private void OnAttackFinished()
    {
        currentMoveState = enemyMoveState.move;
    }

    private void OnEdibleEnemyConsumed()
    {
        QueueFree();
    }

}
