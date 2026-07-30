using Godot;
using System;
using Game.ProjectileStats;

public partial class GunArm : Node2D
{

	[Export] private Sprite2D gunArmSprite;
	[Export] private PackedScene projectileScene;
	[Export] private Node2D spawnPoint;
	[Export] private Timer coolDownTimer;
	private Projectile myProjectile;
	public ProjectileType projectileType;
	private Vector2 projTarget;
	private bool isCooling, isEnabled;
	public override void _PhysicsProcess(double delta)
	{
		GenerateProjectile();
	}

	public void SetEnabled(bool enabled)
	{
		isEnabled = enabled;
	}

	private void GenerateProjectile()
	{
		if (Input.IsActionPressed("Fire") && !isCooling && isEnabled)
		{
			projTarget = GetGlobalMousePosition();
			myProjectile = projectileScene.Instantiate<Projectile>();
			myProjectile.GlobalPosition = spawnPoint.GlobalPosition;
			myProjectile.SetProjectileStats(new ProjectileStats
			{
				projType = ProjectileType.bullet,
				damage = 1000,
				speed = 500,
				target = projTarget
			});

			ProjectileManager.Instance.AddChild(myProjectile);
			isCooling = true;
			coolDownTimer.Start();

		}
	}

	// private void GenerateProjectileII()
	// {

	// 	if (Input.IsActionPressed("Fire") && !isCooling)
	// 	{
	// 		var proj = projectileScene.Instantiate<Projectile>();
	// 		proj.GlobalPosition = GlobalPosition;
	// 		GetTree().CurrentScene.AddChild(proj);
	// 		proj.Launch(GetGlobalMousePosition());
	// 		isCooling = true;
	// 		coolDownTimer.Start();
	// 	}
	// }

	private void OnCooldownTimerTimeout()
	{
		isCooling = false;
	}

}
