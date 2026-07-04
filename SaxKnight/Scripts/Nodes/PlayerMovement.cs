using Godot;
using Game.MagicTypes;
using System;
using System.Data;

public partial class PlayerMovement : Node2D
{
	[Export] private Sprite2D playerSprite;
	[Export] private AnimationPlayer playerAnim;
	[Export] private CharacterBody2D playerBody;
	[Export] private CollisionShape2D playerShape, dodgeShape;
	[Export] private Timer landTimer, jumpTimer, dodgeTimer, rollTimer, dodgeRollRecoverTimer;

	[Export] private float moveSpeed, maxMoveSpeed, climbSpeed, maxClimbSpeed, jumpPower, gravity;
	[Export] private PlayerAttacks playerAttacks;
	private Vector2 playerVelocity, inputDirection;
	private bool isTouchingLadder, isClimbingUp, isClimbingDown, isJumping, isGravityReset, isDodging, isRecovering;
	private int ceilingCounter;


	public enum moveState
	{
		Idle, Walk, Jump, Fall, Land, Power1, Climb, ClimbIdle, Hurt, Dying, Dead, Dive, Roll, Recover
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
			HandleInstrumentInput();
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
				if (!isDodging && !isRecovering)
				{
					currentState = moveState.Walk;
				}
			}
		}
	}

	private void HandleInstrumentInput()
	{
		if (Input.IsActionJustPressed("Red"))
		{
			playerAttacks.FireProjectile(ProjectileColor.red, inputDirection);
		}
		else if (Input.IsActionJustPressed("Orange"))
		{
			playerAttacks.FireProjectile(ProjectileColor.orange, inputDirection);
		}
		else if (Input.IsActionJustPressed("Green"))
		{
			playerAttacks.FireProjectile(ProjectileColor.green, inputDirection);
		}
		else if (Input.IsActionJustPressed("Blue"))
		{
			playerAttacks.FireProjectile(ProjectileColor.blue, inputDirection);
		}
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
						currentState = moveState.Dive;
						isDodging = true;
					}
				}
			}

		}
		if (!playerBody.IsOnFloor())
		{
			playerVelocity.Y += gravity;
			isGravityReset = false;
			isDodging = false;
			if (playerVelocity.Y < 0)
			{
				currentState = moveState.Jump;
			}
			if (playerVelocity.Y > 0)
			{
				currentState = moveState.Fall;
			}
		}

		if (playerBody.IsOnFloor())
		{
			if (!isGravityReset)
			{
				playerVelocity.Y = 0;
				isGravityReset = true;
				currentState = moveState.Land;
				landTimer.Start();
			}

		}
		// if(isDodgeRollRecovering && ceilingCounter > 0)
		// {
		// 	rollTimer.Start();
		// 	isDodging = true;
		// 	currentState = moveState.Roll;
		// }

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
		isDodging = false;
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

	public void OnDiveAnimFinished()
	{
		currentState = moveState.Roll;
	}

	public void OnRollAnimFinished()
	{
		if (ceilingCounter > 0)
		{
			currentState = moveState.Roll;
		}
		else
		{
			GD.Print("Finished and moving to recover state");
			currentState = moveState.Recover;
			isDodging = false;
			isRecovering = true;
		}
	}

	public void OnRecoverAnimationFinished()
	{
		isRecovering = false;
		currentState = moveState.Walk;
	}
	private void OnCeilingSensorBodyEntered(Node2D body)
	{
		ceilingCounter++;
	}

	private void OnCeilingSensorBodyExited(Node2D body)
	{
		ceilingCounter--;
	}

	private void OnLandTimerTimeout()
	{
		if (!isDodging)
		{
			currentState = moveState.Walk;
		}
	}


}
