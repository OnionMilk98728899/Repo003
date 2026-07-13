using Godot;
using System;

public partial class PlayerCam : Camera2D
{
	
	[Export] public CharacterBody2D player;
	[Export]public float followSpeed;

	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
		{
			return;
		}

		GlobalPosition = GlobalPosition.Lerp(
			player.GlobalPosition,
			followSpeed * (float)delta);
	}
}
