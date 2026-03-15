using Game.Damage;
using Game.Upgrades;
using Godot;
using System;
using System.ComponentModel;

public partial class GlobalSignals : Node
{
	public static GlobalSignals Instance { get; private set; }
	[Signal] public delegate void EnterExitBuildModeEventHandler(bool isInBuildMode);
	[Signal] public delegate void RemoveEnemyFromAreaEventHandler(BasicEnemyMovement enemy, Vector2 position);
	[Signal] public delegate void DisableTrapMenuEventHandler(bool isActive);
	[Signal] public delegate void CoinFellFlatEventHandler(Vector2 coinPos, Gold coin);
	[Signal] public delegate void PlayerAttackEventHandler(bool isWeapon1);
	[Signal] public delegate void PlayerDeathEventHandler();

	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}

}
