using Godot;
using System;

public partial class PlayerCam : Camera2D
{
	[Export] public float FollowSpeed;
	[Export] private Player player;
	public Vector2 anchorPosition;
	public enum mode{following, locked}
	public mode currentMode;
	private bool isFollowingPlayer;

	public override void _Ready()
	{
		OnSceneLoaded();
	}

	public void OnSceneLoaded()
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null) { return; }
		else
		{
			if (isFollowingPlayer)
			{
				if(currentMode == mode.following)
				{
					FollowPlayer(delta);
				}else if(currentMode == mode.locked)
				{
					
				}
				
			}
			
		}

	}


	private void FollowPlayer(double delta)
	{
		Vector2 targetPosition = player.GlobalPosition;
		// Interpolate the camera's position towards the target

		targetPosition.X = Mathf.Round(targetPosition.X);
		targetPosition.Y = Mathf.Round(targetPosition.Y);


		anchorPosition = GlobalPosition.Lerp(targetPosition, FollowSpeed * (float)delta);
		GlobalPosition = anchorPosition;
	}
}
