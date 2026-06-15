using Godot;
using System;

public partial class EnemyAttacks : Node2D
{
	[Signal] public delegate void EnterAttackModeEventHandler();
	[Export] private Timer decisionTimer;
	[Export] private float agression;
	private Vector2 target;



	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(double delta)
	{

	}


	private void OnJamModeEntered(bool isInJamMode)
	{
		if (isInJamMode)
		{
			decisionTimer.Start();
		}
		else
		{
			decisionTimer.Stop();
		}
	}

	private void OnDecisionTimerTimeout()
	{
		float randAggro = GD.RandRange(0, 100);
		if (randAggro < agression)
		{
			EmitSignal(SignalName.EnterAttackMode);
			EmitAttack();
		}
		else
		{
			decisionTimer.Start();
		}
	}

	private void EmitAttack()
	{
		
	}


}
