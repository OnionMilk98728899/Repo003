using Godot;
using System;

public partial class PlayerCam : Camera2D
{
	private granny myGranny;
	[Export]
	public float FollowSpeed;
	[Export]
	public Vector2 offset = new Vector2(0, 0), anchorPosition;

	public override void _Ready()
	{
		myGranny = GetNode<granny>("../Granny");
	}

	public override void _PhysicsProcess(double delta)
	{   
		if(myGranny == null){return;}else{



			Vector2 targetPosition = myGranny.GlobalPosition + offset;
			// Interpolate the camera's position towards the target

			targetPosition.X = Mathf.Round(targetPosition.X);
			targetPosition.Y = Mathf.Round(targetPosition.Y);

			
			anchorPosition = GlobalPosition.Lerp(targetPosition, FollowSpeed * (float)delta);
			GlobalPosition = anchorPosition;
		}

	}


}
