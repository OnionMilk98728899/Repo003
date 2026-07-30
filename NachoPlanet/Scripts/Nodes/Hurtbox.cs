using Godot;
using System;

public partial class Hurtbox : Node2D
{
	[Signal] public delegate void KnockbackEnemyEventHandler(Vector2 direction, Vector2 velocity);
	[Signal] public delegate void UnitDamagedEventHandler(float damage);
	[Signal] public delegate void UnitKilledEventHandler();
	[Export] public float health;
	private float incomingDamage;
	public enum unitType { player, enemy }
	[Export] public unitType myUnitType;
	private Projectile damagingProjectile;

	private void PollHealth(float dmg)
	{
		if (health <= 0)
		{
			health = 0;
			EmitSignal(SignalName.UnitKilled);
		}
		else
		{
			EmitSignal(SignalName.UnitDamaged, dmg);
		}
	}
	private void OnHurtAreaEntered(Node2D body)
	{
		if (myUnitType == unitType.enemy)
		{
			if (body.IsInGroup("Projectile"))
			{
				incomingDamage = body.GetNode<Projectile>("..").GetDamage();
				health -= incomingDamage;
				PollHealth(incomingDamage);
			}

			if (body.IsInGroup("Melee"))
			{
				incomingDamage = 500;
				health -= incomingDamage;
				PollHealth(incomingDamage);
			}

		}
	}



}



