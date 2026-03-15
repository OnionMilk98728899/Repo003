using Godot;
using System;

public partial class FindPlayerAI : CharacterBody3D
{
	private BasicMovement myChar;
	private Vector3 targetPosition, distanceToTarget, myVelocity = new Vector3(0,0,0);

	public override void _Ready()
	{
		myChar = GetNode<BasicMovement>("../MyChar");
		if(myChar == null ){GD.Print("My Character not found by Zombie!");}
	}

	public override void _PhysicsProcess(double delta)
	{
		GetPlayerPosition();
		Velocity = myVelocity;
		MoveAndSlide();
	}

	private void GetPlayerPosition(){

		targetPosition = myChar.GlobalPosition;
		distanceToTarget = targetPosition - Position;

		myVelocity += (.005f)*distanceToTarget;
		LookAt(targetPosition);
		
	}
}
