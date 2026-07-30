using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] Sprite2D playerSprite;
    [Export] private float moveSpeed, maxMoveSpeed, jumpPower;
    private Vector2 inputDirection, playerVelocity;
    private bool isJumping;


    public override void _PhysicsProcess(double delta)
    {
        HandleDirectionalInput(delta);
        Velocity = playerVelocity;
        MoveAndSlide();
    }


    private void HandleDirectionalInput(double delta)
    {
        if (Input.IsActionPressed("ui_left")) inputDirection.X = -1;
        else if (Input.IsActionPressed("ui_right")) inputDirection.X = 1;
        else inputDirection.X = 0;

        if (inputDirection.X != 0)
        {
            playerVelocity.X += inputDirection.X * moveSpeed;
            playerVelocity.X = Mathf.Clamp(playerVelocity.X, -maxMoveSpeed, maxMoveSpeed);
            if (inputDirection.X > 0)
            {
                playerSprite.FlipH = false;
            }
            else
            {
                playerSprite.FlipH = true;
            }
        }
        else
        {
            playerVelocity.X = 0;
        }
        
    }


    private void HandleJumpInput(double delta)
    {
        if(IsOnFloor() && Input.IsActionPressed("ActionZ"))
        {
            playerVelocity.Y -= jumpPower;
        }
    }

}
