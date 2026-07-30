using Godot;
using System;

public partial class EnemyAttack : Node2D
{
	[Signal] public delegate void AttackFinishedEventHandler();
	[Export]CharacterBody2D enemyBody;
	[Export] private Sprite2D shadowSprite, enemySprite;
	public enum attackMove { leap }
	[Export] public attackMove currentAttackMove;
	private Vector2 startPosition, targetPosition;
	private float elapsedTime;
	[Export] private float travelTime, arcHeight;
	[Export] bool hasSpin;
	private bool hasBegun;

	// public override void _PhysicsProcess(double delta)
	// {
	//     Attack();
	// }

	public void SetTarget(Vector2 t)
	{
		targetPosition = t;
	}

	public void Attack(Vector2 t, double delta)
	{
		switch (currentAttackMove)
		{
			case attackMove.leap:
				Leap(t, delta);
				break;
		}
	}

	private void Leap(Vector2 t, double delta)
	{
		if (!hasBegun)
		{
				startPosition = enemyBody.GlobalPosition;
				targetPosition = t;
				elapsedTime = 0.0f;
				hasBegun = true;
		}

	elapsedTime += (float)delta;

	float time = Mathf.Clamp(elapsedTime / travelTime, 0.0f, 1.0f);

	// Ground position (shadow moves here)
	Vector2 groundPosition = startPosition.Lerp(targetPosition, time);

	// Simple parabola (0 at start/end, 1 at midpoint)
	float height = 4.0f * arcHeight * time * (1.0f - time);

	shadowSprite.GlobalPosition = groundPosition;
	enemyBody.GlobalPosition = groundPosition + Vector2.Up * height;

		if (hasSpin)
		{
			enemySprite.Rotate(.2f);
		}

	if (time >= 1.0f)
	{
		EmitSignal(SignalName.AttackFinished);
		enemySprite.RotationDegrees = 0;
		hasBegun = false;
	}

	}

}
