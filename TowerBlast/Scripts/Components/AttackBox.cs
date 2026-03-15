using Godot;
using System;
using Game.Upgrades;

public partial class AttackBox : Node2D
{
	[Signal] public delegate void StopToAttackEventHandler();
	[Export] private AnimationPlayer _attackAnim;
	[Export] int _attackPropensity;
	int _attackChance;
	public enum _moveMode
	{
		normal, climbing
	}

	public _moveMode _currentMode;

	private void OnPlayerDetectorBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			_attackChance = GD.RandRange(1, 100);

			if (_attackPropensity > _attackChance)
			{
				EmitSignal(SignalName.StopToAttack);
				Attack();
			}
		}
		if (body.IsInGroup("Weapon"))
		{
			if(body.GetNode<PlayerProjectile>("..").GetWeaponType() == WeaponType.shield)
			{
				GD.Print("Stop To attack signal emitted!");
				EmitSignal(SignalName.StopToAttack);
				Attack();
			}
		}

	}

	private void Attack()
	{
		_attackAnim.Play("Swing");
	}
}
