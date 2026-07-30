using Game.ProjectileStats;
using Godot;
using System;

public partial class Projectile : Node2D
{
	[Export] private CharacterBody2D projBody;
	[Export] private float lifetime;
	[Export] private Timer lifeTimer;
	[Export] private Sprite2D projSprite;
	private float speed, damage;
	public ProjectileType projType;
	private Vector2 projectileTarget, projVelocity;
	private bool isSplattered, isFading;

	public override void _Ready()
	{
		lifeTimer.WaitTime = lifetime;
		lifeTimer.Start();
		SetDirection();
	}


	public override void _PhysicsProcess(double delta)
	{
		// if (projType == ProjectileType.bullet)
		// {
		// 	FlyStraight();
		// }
		projBody.Velocity = projVelocity;
		projBody.MoveAndSlide();
	}

	public void SetTarget(Vector2 target)
	{

	}

	public float GetDamage()
	{
		return damage;
	}

	public void SetProjectileStats(ProjectileStats stats)
	{
		projType = stats.projType;
		speed = stats.speed;
		damage = stats.damage;
		projectileTarget = stats.target;
	}

	private void SetDirection()
	{
		Vector2 direction = (projectileTarget - projBody.GlobalPosition).Normalized();
		projVelocity = direction * speed;
	}

	private void FlyStraight()
	{
		

		if (!isSplattered)
		{
			SplatterOnContact();
		}
		else
		{
			projVelocity = Vector2.Zero;
		}
		if(isSplattered && !isFading && lifeTimer.TimeLeft < lifetime *.2)
		{
			FadeSplatter();
		}
		

	}

	private void OnLifeTimerTimeout()
	{
		QueueFree();
	}


	private void SplatterOnContact()
	{
		if (Mathf.Abs(projectileTarget.X - projBody.GlobalPosition.X) < 1 && Mathf.Abs(projectileTarget.Y - projBody.GlobalPosition.Y) < 1)
		{
			isSplattered = true;
			ZIndex = 1;
			int r = GD.RandRange(1, 4);
			projSprite.Frame = r;

		}
	}

	private void FadeSplatter()
	{
		isFading = true;
		projSprite.Frame += 4;
	}

}
