using Godot;
using System;

public partial class MonsterManager : Node2D
{
	[Export] private PackedScene monsterScene;
	[Export] private Timer monsterBufferTimer;
	private Monster myMonster;
	private double monsterWaitTime {get; set;}


	public override void _Ready()
	{
		monsterBufferTimer.Start();
		monsterWaitTime = 2.0;
	}
	public override void _PhysicsProcess(double delta)
	{
		monsterBufferTimer.WaitTime = monsterWaitTime;
	}

	private void GenerateMonster()
	{
		// myMonster = monsterScene.Instantiate<Monster>();
		// myMonster.GlobalPosition = new Vector2(50, 184);
		// AddChild(myMonster);
	}

	private void OnMonsterBufferTimerTimeout()
	{
		GenerateMonster();
	}
}
