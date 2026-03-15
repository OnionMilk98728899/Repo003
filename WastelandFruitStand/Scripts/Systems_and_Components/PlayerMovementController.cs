using Godot;
using System;

public partial class PlayerMovementController : Node2D
{
	[Signal] public delegate void StateChangedEventHandler();
	[Export] private Node2D player;
	[Export] private CharacterBody2D shadowBody, playerBody, slashBody;
	[Export] private HurtboxSystem hurtBox;
	[Export] private AnimationPlayer playerAnim;
	[Export] private Sprite2D playerSprite, shadowSprite;
	[Export] private Label debugLabel;
	[Export] private int moveSpeed, jumpPower, deceleration, maxRunSpeed, maxVerticalSpeed, verticalSpeed, weight, terminalVelocity;
	[Export] private Timer landTimer, hurtTimer;
	private Vector2 playerVelocity, shadowVelocity, lateralVelocity, jumpVelocity, inputDirection, playerJumpSpot, wallNormal, originalShadowPosition;
	private int newZ, hitPoints = 5;
	private bool isJumping, isFalling, isLanding, isAirborne, isIdle, isMoving, isHurt, isPlayerMovementActive;
	public enum moveState
	{
		idle, run, jump, fall, land, hurt, recovering
	}
	public moveState currentMoveState, nextMoveState;

	public override void _Ready()
	{

		GlobalSignals.Instance.ActivateFruitStand += FruitStandGUIisActive;
		isPlayerMovementActive = true;
		currentMoveState = moveState.idle;

		hurtBox.OnHurt += HandleHurt;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isPlayerMovementActive)
		{
			if (currentMoveState == moveState.idle && !isAirborne)
			{
				ResetShadowPosition();
			}
			ManuallySortZIndex();

			CalculateLateralMovement(delta);
			CalculateJumpMovement(delta);
			playerBody.Velocity = playerVelocity;
			shadowBody.Velocity = shadowVelocity;
			shadowBody.MoveAndSlide();
			playerBody.MoveAndSlide();

		}
		else
		{
			playerVelocity = Vector2.Zero;
			lateralVelocity = Vector2.Zero;
			jumpVelocity = Vector2.Zero;
			inputDirection = Vector2.Zero;
			playerBody.Velocity = playerVelocity;

		}

		if (currentMoveState != nextMoveState)
		{
			EmitSignal(SignalName.StateChanged, (int)nextMoveState);
			//ResetShadowPosition();
		}
		currentMoveState = nextMoveState;
		//debugLabel.Text = GetNode<PlayerAnimator>("../PlayerAnimator").attackTimer.TimeLeft.ToString();
		//debugLabel.Text = currentMoveState.ToString();

	}

	private void CalculateLateralMovement(double delta)
	{
		if (Input.IsActionPressed("ui_left")) inputDirection.X -= 1;
		if (Input.IsActionPressed("ui_right")) inputDirection.X += 1;
		if (Input.IsActionPressed("ui_up")) inputDirection.Y -= 1;
		if (Input.IsActionPressed("ui_down")) inputDirection.Y += 1;
		if (Input.IsActionJustReleased("ui_left") || Input.IsActionJustReleased("ui_right")) inputDirection.X = 0;
		if (Input.IsActionJustReleased("ui_up") || Input.IsActionJustReleased("ui_down")) inputDirection.Y = 0;

		// Normalize to avoid faster diagonal movement
		inputDirection = inputDirection.Normalized();

		// Apply horizontal movement

		if (!isHurt)
		{
			if (inputDirection.X != 0)
			{
				lateralVelocity.X += inputDirection.X * (float)delta * moveSpeed;
				lateralVelocity.X = Mathf.Clamp(lateralVelocity.X, -maxRunSpeed, maxRunSpeed);
				playerSprite.FlipH = inputDirection.X < 0;
				shadowSprite.FlipH = inputDirection.X < 0;

				if (playerSprite.FlipH)
				{
					slashBody.Scale = new Vector2(-1, 1);
				}
				else
				{
					slashBody.Scale = new Vector2(1, 1);
				}


			}

			// Apply vertical movement

			if (inputDirection.Y != 0 && !isAirborne)
			{
				lateralVelocity.Y += inputDirection.Y * (float)delta * verticalSpeed;
				lateralVelocity.Y = Mathf.Clamp(lateralVelocity.Y, -maxVerticalSpeed, maxVerticalSpeed);
			}
			else if (inputDirection.Y != 0 && isAirborne)
			{
				lateralVelocity.Y += inputDirection.Y * (float)delta * .4f * verticalSpeed;
				lateralVelocity.Y = Mathf.Clamp(lateralVelocity.Y, .5f * -maxVerticalSpeed, .5f * maxVerticalSpeed);
			}

			if (inputDirection.X == 0)
			{
				lateralVelocity.X = Mathf.MoveToward(lateralVelocity.X, 0, deceleration * (float)delta);
			}

			if (inputDirection.Y == 0)
			{
				lateralVelocity.Y = Mathf.MoveToward(lateralVelocity.Y, 0, deceleration * (float)delta);
			}
			if (inputDirection.X == 0 && inputDirection.Y == 0 && !isAirborne && landTimer.IsStopped())
			{
				nextMoveState = moveState.idle;
				isMoving = false;
			}
			else
			{
				isMoving = true;
				if (!isAirborne && landTimer.IsStopped())
				{
					nextMoveState = moveState.run;
				}
			}
		}
		else
		{
			lateralVelocity.Y = Mathf.MoveToward(lateralVelocity.Y, 0, deceleration * (float)delta);
			lateralVelocity.X = Mathf.MoveToward(lateralVelocity.X, 0, deceleration * (float)delta);
		}

		/// Stop the player when they come in contact with an environmental obstruction.

		if (playerBody.IsOnCeiling() || shadowBody.IsOnCeiling())
		{
			lateralVelocity.Y = Mathf.Clamp(lateralVelocity.Y, 0, maxVerticalSpeed);
		}
		if (shadowBody.IsOnFloor())
		{
			lateralVelocity.Y = Mathf.Clamp(lateralVelocity.Y, -maxVerticalSpeed, 0);
		}
		if (shadowBody.IsOnWall())
		{
			wallNormal = shadowBody.GetWallNormal();

			if (wallNormal.X > 0)
			{
				lateralVelocity.X = Mathf.Clamp(lateralVelocity.X, 0, maxRunSpeed);
			}

			if (wallNormal.X < 0)
			{
				lateralVelocity.X = Mathf.Clamp(lateralVelocity.X, -maxRunSpeed, 0);
			}
		}

		shadowVelocity = lateralVelocity;

	}

	private void CalculateJumpMovement(double delta)
	{
		if (Input.IsActionJustPressed("Action1") && !isAirborne && !isHurt)
		{
			nextMoveState = moveState.jump;
			isAirborne = true;
			jumpVelocity.Y = -jumpPower;

		}
		if (isAirborne)
		{
			jumpVelocity.Y = Mathf.MoveToward(jumpVelocity.Y, terminalVelocity, weight * (float)delta);
			playerBody.SetCollisionMaskValue(1, false);
		}
		if (isAirborne && jumpVelocity.Y >= 0)
		{
			nextMoveState = moveState.fall;
		}

		if (playerBody.GlobalPosition.Y >= shadowBody.GlobalPosition.Y && currentMoveState == moveState.fall)
		{
			isAirborne = false;
			nextMoveState = moveState.land;
			landTimer.Start();
			jumpVelocity.Y = 0;
			playerBody.SetCollisionMaskValue(1, true);
		}

		playerVelocity = jumpVelocity + lateralVelocity;

	}

	private void ManuallySortZIndex()
	{
		newZ = (int)playerBody.GlobalPosition.Y;
		if (newZ != player.ZIndex)
		{
			player.ZIndex = newZ;
		}
	}

	private void FruitStandGUIisActive(bool isActive)
	{
		isPlayerMovementActive = !isActive;
	}

	private void ResetShadowPosition()
	{
		shadowBody.GlobalPosition = playerBody.GlobalPosition;
	}

	private void HandleHurt(CharacterBody2D body, int multiplier)
	{
		hitPoints--;
		nextMoveState = moveState.hurt;
		isHurt = true;
		hurtTimer.Start();

	}

	private void OnHurtTimerTimeout()
	{
		isHurt = false;
		//nextMoveState = moveState.recovering;
	}

	private void OnMoveTimerTimeout()
	{
		// if (currentMoveState == moveState.land)
		// {

		// }else if (currentMoveState)
	}

}
