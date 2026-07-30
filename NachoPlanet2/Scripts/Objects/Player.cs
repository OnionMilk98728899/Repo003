using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] private Sprite2D playerSprite, gunArmSprite, batArmSprite, leftArmSprite, rightArmSprite;
	[Export] private AnimationPlayer playerAnim;
	[Export] private Timer batChargeTimer;
	[Export] private float walkSpeed, acceleration, maxWalkSpeed, batChargeTime;
	private Vector2 playerVelocity, inputDirection, gunPositionAnchor;
	public enum playerState { walk, idle, attack, hurt }
	public enum playerDirection { up, down, left, right }
	public enum gunDirection { upleft, upright, downleft, downright }
	public enum playerWeapon { gun, bat, none }
	public playerWeapon currentWeapon;
	public gunDirection currentgunDirection;
	public playerState currentState;
	public playerDirection currentDirection;
	private bool isAttacking, isCharging;
    public override void _Ready()
    {
        GD.Print(GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        GetMouseDirection();

		GetMovementInput();
		AnimatePlayer();

		if (currentWeapon == playerWeapon.gun)
		{
			DetermineGunDirection();
		}
		else if (currentWeapon == playerWeapon.bat)
		{
			HoldBat();
		}
		MoveAndSlide();
    }

    private void GetMovementInput()
	{
		inputDirection = Vector2.Zero;

		if (Input.IsActionPressed("Left")) { inputDirection.X -= 1; }
		if (Input.IsActionPressed("Right")) { inputDirection.X += 1; }
		if (Input.IsActionPressed("Up")) { inputDirection.Y -= 1; }
		if (Input.IsActionPressed("Down")) { inputDirection.Y += 1; }

		inputDirection = inputDirection.Normalized();

		if (inputDirection.Length() > 0)
		{
			playerVelocity = playerVelocity.Lerp(inputDirection * walkSpeed, acceleration);
			if (!isAttacking) { currentState = playerState.walk; }
		}
		else
		{
			if (!isAttacking) { currentState = playerState.idle; }
		}

		playerVelocity = inputDirection * walkSpeed;

		Velocity = playerVelocity;
	}

	private void GetMouseDirection()
	{
		Vector2 direction = GetGlobalMousePosition() - GlobalPosition;
		float angle = Mathf.RadToDeg(direction.Angle());

		if (angle < 0) { angle += 360; }


		switch (angle)
		{
			case >= 0 and < 90f:
				currentgunDirection = gunDirection.downright;
				break;

			case >= 90f and < 180f:
				currentgunDirection = gunDirection.downleft;
				break;

			case >= 180f and < 270f:
				currentgunDirection = gunDirection.upleft;
				break;

			default:
				currentgunDirection = gunDirection.upright;
				break;
		}
		if (!isAttacking)
		{
			switch (angle)
			{
				case >= 315 or < 45:
					currentDirection = playerDirection.right;
					break;

				case >= 45 and < 135:
					currentDirection = playerDirection.down;
					break;

				case >= 135 and < 225:
					currentDirection = playerDirection.left;
					break;

				case >= 225 and < 315:
					currentDirection = playerDirection.up;
					break;
			}
		}
	}

	private void DetermineGunDirection()
	{
		gunArmSprite.LookAt(GetGlobalMousePosition());
		gunPositionAnchor = Vector2.Zero;
		switch (currentDirection)
		{
			case playerDirection.right:
				if (currentgunDirection == gunDirection.upright || currentgunDirection == gunDirection.upleft)
				{
					leftArmSprite.Visible = false;
					rightArmSprite.Visible = true;
					gunPositionAnchor = new Vector2(0, -2);
				}
				else
				{
					rightArmSprite.Visible = false;
					leftArmSprite.Visible = true;
					gunPositionAnchor = new Vector2(0, 8);
				}
				break;
			case playerDirection.down:
				if (currentgunDirection == gunDirection.downright || currentgunDirection == gunDirection.upright)
				{
					leftArmSprite.Visible = false;
					rightArmSprite.Visible = true;
					gunPositionAnchor = new Vector2(6, 2);
				}
				else
				{
					rightArmSprite.Visible = false;
					leftArmSprite.Visible = true;
					gunPositionAnchor = new Vector2(-6, 2);
				}
				break;
			case playerDirection.up:
				if (currentgunDirection == gunDirection.upleft || currentgunDirection == gunDirection.downleft)
				{
					leftArmSprite.Visible = false;
					rightArmSprite.Visible = true;
					gunPositionAnchor = new Vector2(-6, 2);
				}
				else
				{
					rightArmSprite.Visible = false;
					leftArmSprite.Visible = true;
					gunPositionAnchor = new Vector2(6, 2);
				}
				break;
			case playerDirection.left:
				if (currentgunDirection == gunDirection.upright || currentgunDirection == gunDirection.upleft)
				{
					leftArmSprite.Visible = true;
					rightArmSprite.Visible = false;
					gunPositionAnchor = new Vector2(0, -2);
				}
				else
				{
					rightArmSprite.Visible = true;
					leftArmSprite.Visible = false;
					gunPositionAnchor = new Vector2(0, 8);
				}
				break;
		}
		gunArmSprite.Position = gunPositionAnchor;
	}

	private void HoldBat()
	{
		rightArmSprite.Visible = true;
		//leftArmSprite.Visible = true;
		gunArmSprite.Visible = false;

		if (Input.IsActionPressed("Fire"))
		{
			if (batChargeTimer.IsStopped())
			{
				batChargeTimer.Start();
			}
			if (batChargeTimer.TimeLeft <= batChargeTime * .8)
			{
				//batArm.ChargeBat();
				leftArmSprite.Visible = false;
				isCharging = true;
			}

		}
		else if (Input.IsActionJustReleased("Fire"))
		{

			batChargeTimer.Stop();
			leftArmSprite.Visible = false;
			currentState = playerState.attack;
			isAttacking = true;
			isCharging = false;
		}
		if (!isAttacking && !isCharging) { leftArmSprite.Visible = true; }
	}

    	private void AnimatePlayer()
	{
		playerAnim.Play(currentState.ToString() + currentDirection.ToString());
	}

    public void ChargeBat()
	{
		batArmSprite.Visible = true;
	}

}
