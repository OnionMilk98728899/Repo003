using Godot;
using System;

public partial class Meanie : Node2D
{
	[Export] private EnemyMovement _enemyMovement;
	public void SetBoardSquare((int,int) square)
	{
		_enemyMovement.SetCurrentBoardSquare(square);
	}

	private void OnEnemyDestroyed()
	{
		QueueFree();
	}
}


