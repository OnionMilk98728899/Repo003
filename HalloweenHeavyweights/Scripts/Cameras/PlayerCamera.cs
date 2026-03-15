using Godot;
using System;

public partial class PlayerCamera : Camera2D
{
	[Export] CharacterBody2D myPlayer;
	private Vector2 playerPos;

	public override void _PhysicsProcess(double delta)
	{
		Position = myPlayer.GlobalPosition;
	}
}
