using Godot;
using System;

public partial class Abilities : Node
{
	private AnimationPlayer abilityAnim;
	public override void _Ready()
	{
		abilityAnim = GetNode<AnimationPlayer>("../Pumpkin/AbilityAnim");
	}

	public override void _PhysicsProcess(double delta)
	{   
		BasicPunches();

	}

	private void BasicPunches(){
		if (Input.IsActionPressed("mouse_click_left")) abilityAnim.Play("Left_Jab");
		if (Input.IsActionPressed("mouse_click_right")) abilityAnim.Play("Right_Jab");
	}
}
