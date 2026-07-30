using Godot;
using System;

public partial class ProjectileManager : Node2D
{
	public static ProjectileManager Instance { get; private set; }

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
