using Godot;
using System;

public partial class PlayerMovement : Node2D
{

	[Export] private CharacterBody2D playerBody;
	[Export] private Sprite2D playerSprite;
	[Export] private AnimationPlayer playerAnim, dustAnim;
	[Export] private float moveSpeed, acceleration, brakeStrength, maxSpeed, maxReverseSpeed, turnSpeed, drag;
	private float forwardSpeed, turnInput, movementAngle = 0.0f;

	private Vector2 playerVelocity, inputDirection;

	public enum direction { right, downRight, down, downLeft, left, upLeft, up, upRight }

	public direction currentDirection;


	public override void _PhysicsProcess(double delta)
	{
		ApplyAcceleration(delta);
		GetDirection(movementAngle);
        AnimateDust();
		playerBody.Velocity = playerVelocity;
		playerBody.MoveAndSlide();
	}

	private void ApplyAcceleration(double delta)
	{

		if (Mathf.Abs(moveSpeed) > 1.0f)
		{
			turnInput = Input.GetAxis("ui_left", "ui_right");
			movementAngle += turnInput * turnSpeed * (float)delta;
		}


		if (Input.IsActionPressed("ui_up"))
		{
			moveSpeed += acceleration * (float)delta;
		}
		else if (Input.IsActionPressed("ui_down"))
		{
			moveSpeed -= brakeStrength * (float)delta;
		}
		else
		{
			moveSpeed = Mathf.MoveToward(moveSpeed, 0.0f, drag * (float)delta);
		}

		moveSpeed = Mathf.Clamp(moveSpeed, -maxReverseSpeed, maxSpeed);

		playerVelocity = Vector2.Right.Rotated(movementAngle) * moveSpeed;

	}


	private void GetDirection(float angleRadians)
	{
		float angle = Mathf.RadToDeg(angleRadians);

		angle = Mathf.PosMod(angle + 360.0f, 360.0f);

		int sector = (int)Mathf.Floor((angle + 22.5f) / 45.0f) % 8;

		switch (sector)
		{
			case 0:
				currentDirection = direction.right;
				playerSprite.Frame = 4;
				break;

			case 1:
				currentDirection = direction.downRight;
				playerSprite.Frame = 3;
				break;

			case 2:
				currentDirection = direction.down;
				playerSprite.Frame = 2;
				break;

			case 3:
				currentDirection = direction.downLeft;
				playerSprite.Frame = 1;
				break;

			case 4:
				currentDirection = direction.left;
				playerSprite.Frame = 0;
				break;

			case 5:
				currentDirection = direction.upLeft;
				playerSprite.Frame = 7;
				break;

			case 6:
				currentDirection = direction.up;
				playerSprite.Frame = 6;
				break;

			case 7:
				currentDirection = direction.upRight;
				playerSprite.Frame = 5;
				break;

			default:
				currentDirection = direction.right;
				playerSprite.Frame = 4;
				break;
		}

	}

    private void AnimateDust()
    {
        if (moveSpeed > 0 && turnInput == 0)
        {
            dustAnim.Play(currentDirection.ToString());
        }
        if(turnInput > 0 || turnInput < 0)
        {
            dustAnim.Play(currentDirection.ToString()+turnInput);
        }
        if(moveSpeed == 0)
        {
            dustAnim.Play("idle");
        }
        
    }

}
