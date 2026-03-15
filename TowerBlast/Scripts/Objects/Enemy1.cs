using Godot;
using System;

public partial class Enemy1 : Node2D
{

	private void OnUnitDestroyed()
	{
		QueueFree();
		GlobalSignals.Instance.EmitSignal("RemoveEnemyFromArea", GetNode<BasicEnemyMovement>("BasicEnemyMovement"), 
		GetNode<BasicEnemyMovement>("BasicEnemyMovement").GetEnemyPosition());
	}
}


