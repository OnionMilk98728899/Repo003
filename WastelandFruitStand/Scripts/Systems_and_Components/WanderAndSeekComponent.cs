using Godot;
using System;

public partial class WanderAndSeekComponent : Node2D
{
	[Signal] public delegate void ChangeToWanderStateEventHandler();
	[Signal] public delegate void ChangeToIdleStateEventHandler();
	[Signal] public delegate void FoundTargetEventHandler();
	[Signal] public delegate void GetTargetPositionFromParentEventHandler();
	[Export] private CharacterBody2D charBody;
	[Export] public Timer moveTimer;
	[Export] private int moveSpeed;
	[Export] private Label debugLabel;
	private int region;
	public Vector2 seekTarget;
	private float moveAngle;
	public enum moveMode
	{
		wandering,
		seeking,
		idle
	}
	public moveMode currentMode;

	private Vector2 movementTarget, monsterVelocity;
	public bool isTargeting, isHurt;

	public override void _Ready()
	{
		DetermineWanderDirection();
	}


	public override void _PhysicsProcess(double delta)
	{
		if (currentMode == moveMode.wandering || currentMode == moveMode.seeking)
		{
			if (Mathf.Abs(movementTarget.X - charBody.GlobalPosition.X) < 3 && MathF.Abs(movementTarget.Y - charBody.GlobalPosition.Y) < 3)
			{
				currentMode = moveMode.idle;
				EmitSignal(SignalName.ChangeToIdleState);
			}
		}
	}


	public Vector2 CalculateMovement()
	{
		if (currentMode == moveMode.wandering || currentMode == moveMode.seeking)
		{
			return (movementTarget - charBody.GlobalPosition).Normalized() * moveSpeed;
		}
		else
		{
			return Vector2.Zero;
		}

	}

	public void CalculateDirection()
	{

		if (currentMode == moveMode.wandering)
		{
			DetermineWanderDirection();
		}
	}

	private void DetermineWanderDirection()
	{
		region = GD.RandRange(0, 3);

		switch (region)
		{
			case 0:
				moveAngle = (float)GD.RandRange(0, Mathf.Pi / 4f);
				break;
			case 1:
				moveAngle = (float)GD.RandRange(3f * Mathf.Pi / 4f, 5f * Mathf.Pi / 4f);
				break;
			case 2:
				moveAngle = (float)GD.RandRange(7f * Mathf.Pi / 4f, Mathf.Tau);
				break;
			default:
				moveAngle = 0f; // Fallback, shouldn't occur
				break;
		}

		movementTarget = new Vector2(Mathf.Cos(moveAngle), Mathf.Sin(moveAngle)) * 100;
		movementTarget += charBody.GlobalPosition;
	}


	public Vector2 GetTarget()
	{
		return movementTarget;
	}

	public void SetTarget(Vector2 target)
	{
		movementTarget = target;
	}

	private void OnMoveTimerTimeout()
	{
		if (!isHurt)
		{
			if (currentMode == moveMode.wandering || currentMode == moveMode.idle && !isTargeting)
			{
				int moveDecider = GD.RandRange(0, 10);
				if (moveDecider <= 1)
				{
					currentMode = moveMode.idle;
					EmitSignal(SignalName.ChangeToIdleState);
				}
				else
				{
					currentMode = moveMode.wandering;
					EmitSignal(SignalName.ChangeToWanderState);
				}
			}
			if (currentMode == moveMode.seeking)
			{
				EmitSignal(SignalName.GetTargetPositionFromParent);
			}

			CalculateDirection();
			moveTimer.WaitTime = GD.RandRange(2, 6);
			moveTimer.Start();
		}
	}
}
