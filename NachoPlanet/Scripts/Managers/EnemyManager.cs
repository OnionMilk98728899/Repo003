using Godot;
using System;

public partial class EnemyManager : Node2D
{
	public int enemyCount;

	public override void _Ready()
	{
		GlobalSignals.Instance.EnemyKilled += OnEnemyKilled;
	}


	private void OnEnemyKilled()
	{
		enemyCount--;
	}
}
