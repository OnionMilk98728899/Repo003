using Godot;
using System;

public partial class Enemy1 : Node2D
{
	private void OnDestroyEnemy()
	{
		QueueFree();
	}
}


