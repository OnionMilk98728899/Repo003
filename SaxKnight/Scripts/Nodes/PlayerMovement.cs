using Godot;
using System;
using System.Data;

public partial class PlayerMovement : Node2D
{
	[Export] private Sprite2D playerSprite;
	[Export] private AnimationPlayer playerAnim;
	[Export] private CharacterBody2D playerBody;
	[Export] private CollisionShape2D playerShape, dodgeShape;
	[Export] private Timer landTimer, jumpTimer, dodgeTimer;

	[Export] private float moveSpeed, maxMoveSpeed, climbSpeed, maxClimbSpeed, jumpPower, gravity;
	private Vector2 playerVelocity, inputDirection;
	private bool isTouchingLadder, isClimbingUp, isClimbingDown, isJumping, isGravityReset, isDodging;


	public enum moveState
	{
		Idle, Walk, Jump, Fall, Land, Jam, Power1, Climb, ClimbIdle, Hurt, Dying, Dead, Dodge, Roll
	}

	public moveState currentState;

	public override void _Ready()
	{
		GameManager.playerBody = playerBody;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentState != moveState.Dying && currentState != moveState.Dead)
		{
			HandleDirectionalInput(delta);
			HandleJumpDodgeInput();
			HandleClimbModeInput(delta);
		}
		else
		{
			CalculateDeathMovement();
		}

		playerBody.Velocity = playerVelocity;
		AnimatePlayer();
		playerBody.MoveAndSlide();
	}

	private void HandleDirectionalInput(double delta)
	{
		if (Input.IsActionPressed("ui_left")) inputDirection.X = -1;
		if (Input.IsActionPressed("ui_right")) inputDirection.X = 1;
		//if (Input.IsActionPressed("ui_up")) inputDirection.Y -= 1;
		//if (Input.IsActionPressed("ui_down")) inputDirection.Y += 1;

		if (inputDirection.X != 0)
		{
			playerVelocity.X += inputDirection.X * moveSpeed;
			playerVelocity.X = Mathf.Clamp(playerVelocity.X, -maxMoveSpeed, maxMoveSpeed);
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

		if (!playerBody.IsOnFloor() && isTouchingLadder && Input.IsActionPressed("ui_up"))
		{
			isClimbingUp = true;
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
				if (!isDodging)
				{
					currentState = moveState.Walk;
				}

			}

		}

	}

	private void HandleInstrumentInput()
	{
		if (Input.IsActionJustPressed("red"))
		{

		}
		else if (Input.IsActionJustPressed("orange"))
		{

		}
		else if (Input.IsActionJustPressed("green"))
		{

		}
		else if (Input.IsActionJustPressed("blue"))
		{

		}
		else if (Input.IsActionJustPressed("purple"))
		{

		}
	}

	private void FireProjectile()
	{

	}

	private void HandleClimbModeInput(double delta)
	{
		// if (isInClimbMode)
		// {
		// 	if (inputDirection.X != 0)
		// 	{
		// 		playerVelocity.X += inputDirection.X * (float)delta * climbSpeed;
		// 		playerVelocity.X = Mathf.Clamp(playerVelocity.X, -maxClimbSpeed, maxClimbSpeed);
		// 		currentState = moveState.Climb;

		// 	}
		// 	if (inputDirection.Y != 0)
		// 	{
		// 		playerVelocity.Y += inputDirection.Y * (float)delta * climbSpeed;
		// 		playerVelocity.Y = Mathf.Clamp(playerVelocity.Y, -maxClimbSpeed, maxClimbSpeed);
		// 		//_currentAnim = "Climb";
		// 		currentState = moveState.Climb;
		// 	}
		// 	if (inputDirection.X == 0 && !isJumping)
		// 	{
		// 		playerVelocity.X = 0;

		// 	}
		// 	if (inputDirection.Y == 0 && !isJumping)
		// 	{
		// 		playerVelocity.Y = 0;
		// 	}

		// 	if (inputDirection.X == 0 && inputDirection.Y == 0)
		// 	{
		// 		//_currentAnim = "Climb_Idle";
		// 		currentState = moveState.Climbidle;
		// 		//GD.Print("Climb_Idle");
		// 	}

		// }
		// if (inputDirection.Y > 0)
		// {
		// 	_lowerLadderColl.Disabled = false;
		// }
		// else
		// {
		// 	_lowerLadderColl.Disabled = true;
		// }

	}
	private void HandleJumpDodgeInput()
	{
		if (jumpTimer.IsStopped())
		{
			if (Input.IsActionPressed("ui_up"))
			{
				if (playerBody.IsOnFloor() || isClimbingUp || isClimbingDown && !isDodging)
				{

					if (RhythmManager.Instance.CheckInputForRhythm())
					{
						playerVelocity.Y -= jumpPower;
						isJumping = true;
						jumpTimer.Start();
						//isInClimbMode = false;
					}


				}
			}
			if (Input.IsActionPressed("ui_down") && !isDodging)
			{
				if (playerBody.IsOnFloor() && !isDodging)
				{
					if (RhythmManager.Instance.CheckInputForRhythm())
					{
						currentState = moveState.Dodge;
						isDodging = true;
						dodgeTimer.Start();
					}
				}
			}

		}
		if (!playerBody.IsOnFloor())
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

	public void EnableDodgeCollision()
	{
		dodgeShape.Disabled = false;
		playerShape.Disabled = true;
	}
	public void DisableDodgeCollision()
	{
		dodgeShape.Disabled = true;
		playerShape.Disabled = false;
		if (playerBody.IsOnCeiling())
		{
			
		}
	}

	private void CalculateDeathMovement()
	{
		playerVelocity = Vector2.Zero;
	}

	private void AnimatePlayer()
	{
		playerAnim.Play(currentState.ToString());
	}

	private void OnSpikesDetectorBodyEntered(Node2D body)
	{
		currentState = moveState.Dying;
	}

	public void OnDyingAnimFinished()
	{
		currentState = moveState.Dead;
	}

	private void OnDodgeTimerTimeout()
	{
		isDodging = false;
	}

}
