using Godot;
using System;

public partial class PlayerMovement : Node2D
{
    [Export] private Sprite2D playerSprite;
    [Export] private AnimationPlayer playerAnim;
    [Export] private CharacterBody2D playerBody;
    [Export] private Timer landTimer, jumpTimer;

    [Export] private float moveSpeed, maxMoveSpeed, climbSpeed, maxClimbSpeed, jumpPower, gravity;
    private Vector2 playerVelocity, inputDirection;
    private bool isTouchingLadder, isInClimbMode, isJumping, isGravityReset;
    

    public enum moveState
    {
        Idle, Walk, Jump, Fall, Land, Jam, Blow, Climb, Climbidle
    }

    public moveState currentState;

    public override void _Ready()
    {
        GameManager.playerBody = playerBody;
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleDirectionalInput(delta);
        HandleJumpInput();
        HandleClimbModeInput(delta);
        playerBody.Velocity = playerVelocity;
        AnimatePlayer();
        playerBody.MoveAndSlide();
    }

    private void HandleDirectionalInput(double delta)
	{
		if (Input.IsActionPressed("ui_left")) inputDirection.X -= 1;
		if (Input.IsActionPressed("ui_right")) inputDirection.X += 1;
		if (Input.IsActionPressed("ui_up")) inputDirection.Y -= 1;
		if (Input.IsActionPressed("ui_down")) inputDirection.Y += 1;
		if (Input.IsActionJustReleased("ui_left") || Input.IsActionJustReleased("ui_right")) inputDirection.X = 0;
		if (Input.IsActionJustReleased("ui_up") || Input.IsActionJustReleased("ui_down")) inputDirection.Y = 0;

		if (inputDirection.X != 0)
		{
			playerVelocity.X += inputDirection.X * (float)delta * moveSpeed;
			playerVelocity.X = Mathf.Clamp(playerVelocity.X, - maxMoveSpeed, maxMoveSpeed);
			if (inputDirection.X > 0)
			{
				playerSprite.FlipH = false;
			}
			else
			{
				playerSprite.FlipH = true;
			}
		}
		else
		{
			playerVelocity.X = 0;
		}

		if (inputDirection.Y != 0 & isTouchingLadder)
		{
			isInClimbMode = true;
		}

		if (playerBody.IsOnFloor() && landTimer.IsStopped())
		{
			if (inputDirection.X == 0)
			{
				//_currentAnim = "Idle";
				currentState = moveState.Idle;

			}
			else
			{
				//_currentAnim = "Run";
				currentState = moveState.Walk;
			}

		}

	}

    private void HandleClimbModeInput(double delta)
	{
		if (isInClimbMode)
		{
			if (inputDirection.X != 0)
			{
				playerVelocity.X += inputDirection.X * (float)delta * climbSpeed;
				playerVelocity.X = Mathf.Clamp(playerVelocity.X, -maxClimbSpeed, maxClimbSpeed);
				currentState = moveState.Climb;

			}
			if (inputDirection.Y != 0)
			{
				playerVelocity.Y += inputDirection.Y * (float)delta * climbSpeed;
				playerVelocity.Y = Mathf.Clamp(playerVelocity.Y, -maxClimbSpeed, maxClimbSpeed);
				//_currentAnim = "Climb";
				currentState = moveState.Climb;
			}
			if (inputDirection.X == 0 && !isJumping)
			{
				playerVelocity.X = 0;

			}
			if (inputDirection.Y == 0 && !isJumping)
			{
				playerVelocity.Y = 0;
			}

			if (inputDirection.X == 0 && inputDirection.Y == 0)
			{
				//_currentAnim = "Climb_Idle";
				currentState = moveState.Climbidle;
				//GD.Print("Climb_Idle");
			}

		}
		// if (inputDirection.Y > 0)
		// {
		// 	_lowerLadderColl.Disabled = false;
		// }
		// else
		// {
		// 	_lowerLadderColl.Disabled = true;
		// }

	}
	private void HandleJumpInput()
	{
		if (Input.IsActionPressed("action_X") && jumpTimer.IsStopped())
		{
			if (playerBody.IsOnFloor() || isInClimbMode)
			{
				playerVelocity.Y -= jumpPower;
				isJumping = true;
				jumpTimer.Start();
				isInClimbMode = false;

			}

		}
		if (!playerBody.IsOnFloor() && !isInClimbMode)
		{
			playerVelocity.Y += gravity;
			isGravityReset = false;
			if (playerVelocity.Y < 0)
			{
				//currentAnim = "Jump";
				currentState = moveState.Jump;
			}
			if (playerVelocity.Y > 0)
			{
				//currentAnim = "Fall";
				currentState = moveState.Fall;
			}
		}

		if (playerBody.IsOnFloor())
		{
			if (!isGravityReset)
			{
				playerVelocity.Y = 0;
				isGravityReset = true;
				//currentAnim = "Land";
				currentState = moveState.Land;
				landTimer.Start();
			}
		}
	}

    private void AnimatePlayer()
    {
        playerAnim.Play(currentState.ToString());
    }

}
