using System;
using Godot;

public partial class PlayerMovement : CharacterBody2D
{
    [Export]
    private float moveSpeed;

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement();
    }


    private void HandleMovement()
    {
        Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        Vector2 isoDirection = new Vector2(
            inputDirection.X - inputDirection.Y,
            (inputDirection.X + inputDirection.Y) * 0.5f
        ).Normalized();

        Velocity = isoDirection * moveSpeed;
        MoveAndSlide();
    }
}
