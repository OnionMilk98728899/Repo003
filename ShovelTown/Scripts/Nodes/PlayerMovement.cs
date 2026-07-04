using Godot;
using System;
using System.Data;

public partial class PlayerMovement : Node2D
{

    [Export] private CharacterBody2D playerBody;
    [Export] private AnimationPlayer playerAnim;
    [Export] private float walkSpeed;
    [Export] private int digPower;
    [Export] private CollisionShape2D snowDetectDown, snowDetectUp, snowDetectRight, snowDetectLeft;
    private Vector2 playerVelocity, inputDirection;
    private Snow targetSnow;

    public enum moveState { idle, walk, dig }
    public enum direction { left, right, up, down }
    private int digState;
    private float digDelay;

    public moveState currentMoveState;
    public direction currentDirection;
    private string animationString;

    public override void _Ready()
    {
        currentMoveState = moveState.idle;
        currentDirection = direction.down;
    }

    public override void _PhysicsProcess(double delta)
    {
        ApplyDirectionalMovement();
        ApplyShovelingMovement();
        AnimatePlayer();
        DetectSnow();
        playerBody.Velocity = playerVelocity;
        playerBody.MoveAndSlide();
    }

    private void ApplyDirectionalMovement()
    {

        if (currentMoveState != moveState.dig)
        {
            if (Input.IsActionPressed("ui_left"))
            {
                inputDirection.X = -1;
                currentDirection = direction.left;
            }
            else if (Input.IsActionPressed("ui_right"))
            {
                inputDirection.X = 1;
                currentDirection = direction.right;
            }
            else { inputDirection.X = 0; }

            if (Input.IsActionPressed("ui_down"))
            {
                inputDirection.Y = 1;
                currentDirection = direction.down;
            }
            else if (Input.IsActionPressed("ui_up"))
            {
                inputDirection.Y = -1;
                currentDirection = direction.up;
            }
            else { inputDirection.Y = 0; }
        }


        playerVelocity = inputDirection * walkSpeed;

        if (currentMoveState == moveState.dig)
        {
            playerVelocity = Vector2.Zero;
        }
        else
        {
            if (inputDirection.X != 0 || inputDirection.Y != 0)
            {
                currentMoveState = moveState.walk;
            }
            else
            {
                currentMoveState = moveState.idle;
            }
        }

    }

    private void ApplyShovelingMovement()
    {
        if (targetSnow != null)
        {
            if (Input.IsActionPressed("shovel"))
            {
                currentMoveState = moveState.dig;
                digState = 1;
            }
        }
    }

    private void AnimatePlayer()
    {
        animationString = currentMoveState.ToString() + currentDirection.ToString();

        if (currentMoveState == moveState.dig)
        {
            animationString = animationString = currentMoveState.ToString() + digState + currentDirection.ToString();
        }
        playerAnim.Play(animationString);
    }

    private void DetectSnow()
    {
        snowDetectDown.Disabled = true;
        snowDetectUp.Disabled = true;
        snowDetectLeft.Disabled = true;
        snowDetectRight.Disabled = true;

        switch (currentDirection)
        {
            case direction.down:
                snowDetectDown.Disabled = false;
                break;
            case direction.up:
                snowDetectUp.Disabled = false;
                break;
            case direction.left:
                snowDetectLeft.Disabled = false;
                break;
            case direction.right:
                snowDetectRight.Disabled = false;
                break;
        }
    }

    public void OnDigModeFinished()
    {
        if(digState < 3)
        {
           digState ++;
           GD.Print ($"dig mode is {digState} and move state is {currentMoveState}");
        }
        else
        {
            targetSnow.ScoopSnow(digPower);
            GD.Print("Back to idle");
            currentMoveState = moveState.idle;
        }
        
    }
    private void OnSnowDetectorBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Snow"))
        {
            targetSnow = body.GetNode<Snow>("..");
        }
    }

    private void OnSnowDetectorBodyExited(Node2D body)
    {

    }

    private void OnDigDelayTimerTimeout()
    {

    }

}
