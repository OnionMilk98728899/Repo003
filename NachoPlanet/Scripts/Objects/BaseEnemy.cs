using Godot;
using System;

public partial class BaseEnemy : CharacterBody2D
{

	private void OnEnemyDestroyed()
	{
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.EnemyKilled);
		QueueFree();
	}


}



