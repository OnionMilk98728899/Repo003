using Godot;
using System;

public partial class SnapCamera : Camera2D
{
    [Export] private Node2D player;
    [Export] private Vector2 screenSize;
    [Export] private Vector2 backGroundOffset;
    [Export] private Texture2D eggValleyBackGround;
    [Export] private Sprite2D backGroundSprite;

    public override void _Ready()
    {
        screenSize = GetViewportRect().Size / Zoom;
    }

    public override void _PhysicsProcess(double delta)
    {
        SnapCameraToScene();
    }

    private void SnapCameraToScene()
    {
        Vector2 playerPosition = player.GlobalPosition;
        Vector2 cameraPosition = GlobalPosition;

        if (playerPosition.X < cameraPosition.X)
        {
            GlobalPosition = new Vector2(
                GlobalPosition.X - screenSize.X,
                GlobalPosition.Y);
        }
        else if (playerPosition.X > cameraPosition.X + screenSize.X)
        {
            GlobalPosition = new Vector2(
                GlobalPosition.X + screenSize.X,
                GlobalPosition.Y);
        }

        if (playerPosition.Y < cameraPosition.Y)
        {
            GlobalPosition = new Vector2(
                GlobalPosition.X,
                GlobalPosition.Y - screenSize.Y);
        }
        else if (playerPosition.Y > cameraPosition.Y + screenSize.Y)
        {
            GlobalPosition = new Vector2(
                GlobalPosition.X,
                GlobalPosition.Y + screenSize.Y);
        }

        backGroundSprite.GlobalPosition = GlobalPosition + backGroundOffset;
    }
}
