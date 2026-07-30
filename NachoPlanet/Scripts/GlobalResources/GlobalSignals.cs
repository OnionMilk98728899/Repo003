using Godot;
using System;

public partial class GlobalSignals : Node2D
{
	public static GlobalSignals Instance { get; private set; }
	[Signal] public delegate void MainSceneLoadedEventHandler();
	[Signal] public delegate void EnemyKilledEventHandler();
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
