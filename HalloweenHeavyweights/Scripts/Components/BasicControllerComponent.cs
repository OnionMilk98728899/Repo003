using Godot;
using System;

public partial class BasicControllerComponent : Node2D
{
	[Export] CharacterBody2D characterBodyComponent;
	[Export] float speed, acceleration, deceleration;
	//[Export] private string headDownAnimPath, headRightAnimPath, headUpAnimPath, headLeftAnimPath;
	//[Export] private Sprite2D headTexture;

	private Vector2 myVelocity, direction;

	private Label debugLabel;
	public override void _Ready()
	{
		GlobalSignals.Instance.AdjustCharPosition += ManualAdjustCharacterPosition;
	}

	public override void _PhysicsProcess(double delta)
	{

		Vector2 inputDirection = GetInputDirection();
		inputDirection = inputDirection.Normalized();

		if (inputDirection.Length() > 0)
		{
			// Accelerate towards input direction
			myVelocity = myVelocity.Lerp(inputDirection * speed, (float)delta * acceleration);
		}
		else
		{
			// Decelerate when no input
			myVelocity = myVelocity.Lerp(Vector2.Zero, (float)delta * deceleration);
		}

		characterBodyComponent.Velocity = myVelocity;
		characterBodyComponent.MoveAndSlide();

	}

	public void ManualAdjustCharacterPosition(Vector2 newPos){
		characterBodyComponent.GlobalPosition = newPos;
	}

	private Vector2 GetInputDirection(){

		Vector2 direction = Vector2.Zero;

		if (Input.IsActionPressed("move_forward")){
			direction.Y -= 1;
			//headTexture.Frame = 2;
		} 

		if (Input.IsActionPressed("move_backward")){
			direction.Y += 1;
			//headTexture.Frame = 0;
		} 

		if (Input.IsActionPressed("move_left")){
			direction.X -= 1;
			//headTexture.Frame = 3;
		} 

		if (Input.IsActionPressed("move_right")){
			direction.X += 1;
			//headTexture.Frame = 1;
		} 

		
		return direction;
	}

}
