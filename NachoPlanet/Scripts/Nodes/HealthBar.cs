using Godot;
using System;

public partial class HealthBar : Node2D
{
	[Export] private Hurtbox hurtBox;
	[Export] private TextureProgressBar healthBar;
	private float health;


	public override void _Ready()
	{
		health = hurtBox.health;
		healthBar.Value = health/3 +333;
	}

	private void OnHurtboxUnitDamaged(float damage)
	{
		if (!healthBar.Visible)
		{
			healthBar.Visible = true;
		}

		health -= damage;
		healthBar.Value = health/3 +333;
	}

	private void OnEnemyUnitKilled()
	{
		healthBar.Visible = false;
	}
}
