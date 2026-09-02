using Godot;
using System;

public partial class SceneCamera : Camera2D
{
    [Export] private Player player;
    [Export]private int screenSizeX, screenSizeY;
    public override void _PhysicsProcess(double delta)
    {
        ShiftScreen();

    }

    private bool IsObjectOnScreen(Node2D objectToCheck)
    {
        Vector2 screenPosition = GetCanvasTransform() * objectToCheck.GlobalPosition;
        return GetViewport().GetVisibleRect().HasPoint(screenPosition);
    }

    private Rect2 GetCameraBounds()
    {
        Rect2 viewport = GetViewport().GetVisibleRect();
        Transform2D inverse = GetCanvasTransform().AffineInverse();

        Vector2 topLeft = inverse * viewport.Position;
        Vector2 bottomRight = inverse * viewport.End;

        return new Rect2(topLeft, bottomRight - topLeft);
    }


    private void ShiftScreen()
    {

        if (!IsObjectOnScreen(player))
        {
            Rect2 bounds = GetCameraBounds();
            Vector2 newPos = Vector2.Zero;
            if (player.GlobalPosition.X < bounds.Position.X)
            {
                newPos = new Vector2(GlobalPosition.X - screenSizeX, GlobalPosition.Y);
            }
            else if (player.GlobalPosition.X > bounds.End.X)
            {
                newPos = new Vector2(GlobalPosition.X +screenSizeX, GlobalPosition.Y);
            }
            else if (player.GlobalPosition.Y < bounds.Position.Y)
            {
                newPos = new Vector2(GlobalPosition.X, GlobalPosition.Y - screenSizeY);
            }
            else if (player.GlobalPosition.Y > bounds.End.Y)
            {
                newPos = new Vector2(GlobalPosition.X, GlobalPosition.Y + screenSizeY);
            }
            GlobalPosition = newPos;
            EventBus.Instance.EmitSignal(EventBus.SignalName.RepositionPlayerOrigin);
        }
    }
}
