using Godot;
using System;
using Game.MagicTypes;

public partial class EnemyAttacks : Node2D
{
	[Signal] public delegate void EnterAttackModeEventHandler();
	[Export] private PackedScene enemyMagicScene;
	[Export] private Timer decisionTimer;
	[Export] private float agression, offsetNum;
	private Vector2 target, attackDirection;
	private Magic enemyMagic;



	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(double delta)
	{

	}
	private void OnAttackReadyModeEntered(bool entered, Vector2 direction)
	{
		if (entered)
		{
			if (decisionTimer.IsStopped())
			{
				decisionTimer.Start();
				attackDirection = direction;
			}
			
		}
		else
		{
			decisionTimer.Stop();
		}
	}

	private void OnDecisionTimerTimeout()
	{
		GD.Print("Decided!");
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
		enemyMagic = enemyMagicScene.Instantiate<Magic>();
        RhythmManager.Instance.AddChild(enemyMagic);
		enemyMagic.SetNoteType();
        enemyMagic.SetMagicStats(ProjectileColor.red, attackDirection, Magic.unitType.enemy, 90);
		Vector2 offset = new Vector2 (GlobalPosition.X + 8, GlobalPosition.Y);
        enemyMagic.GlobalPosition = offset;
        GD.Print("MagicMade");
	}


}
