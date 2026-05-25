using Godot;
using System;

public partial class PlayerCam : Camera2D
{
    //public CharacterBody2D myPlayerBody;
    [Export] public float FollowSpeed;
    public Vector2 anchorPosition;

    public override void _Ready()
    {
        OnSceneLoaded();
    }

    public void OnSceneLoaded()
    {
       // myPlayerBody = GetNode<CharacterBody2D>("../../Player/PlayerMovement/PlayerBody");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GameManager.playerBody == null) { return; }
        else
        {
            Vector2 targetPosition = GameManager.playerBody.GlobalPosition;
            // Interpolate the camera's position towards the target

            targetPosition.X = Mathf.Round(targetPosition.X);
            targetPosition.Y = Mathf.Round(targetPosition.Y);


            anchorPosition = GlobalPosition.Lerp(targetPosition, FollowSpeed * (float)delta);
            GlobalPosition = anchorPosition;
        }

    }
}
