using Godot;
using System;

public partial class BatArm : Node2D
{
	[Signal] public delegate void ChargingBatEventHandler();
	[Signal] public delegate void SwingingBatEventHandler();

	[Export] private Sprite2D batArmSprite;
	private bool isCharging, isAttacking;
	public override void _PhysicsProcess(double delta)
	{
		//ChargeBat();
	}

	// public void SetEnabled(bool charging)
	// {
	//     isCharging = charging;
	// }

	public void ChargeBat()
	{

		batArmSprite.Visible = true;
		
	}

	public void HideBat()
	{
		batArmSprite.Visible = false;
	}

	public void SwingBat()
	{
		
	}

}
