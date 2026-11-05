using Godot;
using System;

public partial class SceneCamera : Camera2D
{
    [Export] private Player player;
    [Export] private int screenSizeX, screenSizeY;
    [Export] private Timer shakeTimer;
    private int shakeIntensity;
    private Vector2 shakePosition, oldPosition, lockedPosition;
    private bool isShaking;
    private Rect2 camBounds;

    public override void _Ready()
    {
        EventBus.Instance.ScreenShake += OnScreenShake;
        //lockedPosition = Vector2.Zero;
        //ShiftScreen();
        GD.Print("Screen pos = " +GlobalPosition);
    }

    public override void _PhysicsProcess(double delta)
    {
        ShiftScreen();
    }

    private bool IsObjectOnScreen(Node2D objectToCheck)
    {
        Vector2 screenPosition = GetCanvasTransform() * objectToCheck.GlobalPosition;
        return GetViewport().GetVisibleRect().HasPoint(screenPosition);
    }

    private bool IsObjectOnScreen2(Node2D objectToCheck)
    {
        // Vector2 topLeft = GlobalPosition;
        // Vector2 bottomRight = new Vector2(GlobalPosition.X + screenSizeX, GlobalPosition.Y + screenSizeY);
        // Rect2 screen = new Rect2(topLeft, bottomRight - topLeft);

        // return screen.HasPoint(objectToCheck.GlobalPosition);

        // if(objectToCheck.GlobalPosition.X < topLeft.X || objectToCheck.GlobalPosition.Y < topLeft.Y ||
        // objectToCheck.GlobalPosition.X > bottomRight.X || objectToCheck.GlobalPosition.Y > bottomRight.Y)
        // {
        //     return false;
        // }
        // else
        // {
        //     return true;
        // }
         return camBounds.HasPoint(objectToCheck.GlobalPosition);

    }
    private Rect2 GetCameraBounds()
    {
        Rect2 viewport = GetViewport().GetVisibleRect();
        Transform2D inverse = GetCanvasTransform().AffineInverse();

        Vector2 topLeft = inverse * viewport.Position;
        Vector2 bottomRight = inverse * viewport.End;

        return new Rect2(topLeft, bottomRight - topLeft);
    }
    private Rect2 GetCameraBounds2()
    {
        Vector2 topLeft = GlobalPosition;
        Vector2 bottomRight = new Vector2(GlobalPosition.X + screenSizeX, GlobalPosition.Y + screenSizeY);
        return new Rect2(topLeft, bottomRight - topLeft);
    }


    private void ShiftScreen()
    {
        if (!IsObjectOnScreen2(player))
        {

            if (isShaking)
            {
                isShaking = false;
                //GlobalPosition = oldPosition;
            }

            //Rect2 bounds = GetCameraBounds2();
            GD.Print($"Player: {player.GlobalPosition} not on screen: {camBounds} and Screen shifted");
            Vector2 newPos = GlobalPosition;
            if (player.GlobalPosition.X < camBounds.Position.X)
            {
                newPos.X -= screenSizeX;
            }
            else if (player.GlobalPosition.X > camBounds.End.X)
            {
                newPos.X += screenSizeX;
            }
            else if (player.GlobalPosition.Y < camBounds.Position.Y)
            {
                newPos.Y -= screenSizeY;
            }
            else if (player.GlobalPosition.Y > camBounds.End.Y)
            {
                newPos.Y += screenSizeY;
            }
            GlobalPosition = newPos;
            camBounds = GetCameraBounds2();
            EventBus.Instance.EmitSignal(EventBus.SignalName.RepositionPlayerOrigin);
        }
        else
        {
            if (isShaking)
            {
                GlobalPosition = GlobalPosition.MoveToward(oldPosition, .1f * shakeIntensity);
            }
        }
    }

    private void OnScreenShake(int intensity, float duration)
    {
        if (!isShaking)
        {
            oldPosition = GlobalPosition;
        }
        isShaking = true;
        shakePosition = new Vector2(GlobalPosition.X, GlobalPosition.Y - intensity);
        GlobalPosition = shakePosition;
        shakeIntensity = intensity;
        shakeTimer.WaitTime = duration;
        shakeTimer.Start();

    }

    private void OnShakeTimerTimeout()
    {
        isShaking = false;
        GlobalPosition = oldPosition;
    }
}
