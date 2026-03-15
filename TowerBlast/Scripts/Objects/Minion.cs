using Godot;
using System;

public partial class Minion : Node2D
{
	[Signal] public delegate void OnMinionDiedEventHandler();
	[Export] private MinionMovement _myMovement;
	public override void _Ready()
	{
		GlobalSignals.Instance.RemoveEnemyFromArea += OnEnemyKilled;
	}


	public void SetMinionSpawnPosition(Vector2 position)
	{
		_myMovement.SetMinionPosition(position);
	}

	public void SetMinionType(StaticTrap._minionType minType)
	{
		_myMovement.SetMinionType(minType);
	}

	private void OnEnemyKilled(BasicEnemyMovement enemyBody, Vector2 position)
	{
	   _myMovement.OnEnemyKilled(enemyBody);
	}

	private void OnKillMinion()
	{
		QueueFree();
		EmitSignal("OnMinionDied");
	}
}
