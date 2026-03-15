using Godot;
using System;

public partial class basic_2D_enemy_001 : CharacterBody2D
{

	private BasicControllerComponent myPlayer;
	private Vector2 myVelocity, playerPosition, targetPosition, direction;
	private float distance;
	[Export]private float stopRadius, followSpeed;
	public override void _Ready()
	{
		myPlayer = GetNode<BasicControllerComponent>("../2DTestChar/BasicControllerComponent");
	}

	public override void _PhysicsProcess(double delta)
	{
		FollowPlayer(delta);
		Velocity = myVelocity;
		MoveAndSlide();
	}

	private void FollowPlayer(double delta){

		direction = myPlayer.GlobalPosition - GlobalPosition;
		distance = direction.Length();

		if (distance > stopRadius){

			direction = direction.Normalized();
			myVelocity = direction * followSpeed;
		}
		else{
			myVelocity = Vector2.Zero;
		}

	}
}
