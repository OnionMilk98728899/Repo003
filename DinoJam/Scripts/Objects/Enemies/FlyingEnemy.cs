using Godot;
using System;
using System.Drawing;

public partial class FlyingEnemy : Enemy
{
    [Export] private float moveSpeed, acceleration, idleRange, swoopRange, swoopDepth, turnDistance;
    [Export] private AnimationPlayer enemyAnim;
    [Export] private Label debugLabel;
    [Export] private Sprite2D enemySprite;
    private float distance, swoopCount;
    private Vector2 enemyVelocity, pointA, pointB, pointC, pointD, target, toTarget, direction;
    public enum moveState { flyidle, prepare, swoopattack, hurt, dying }
    public moveState currentMoveState;
    private bool isFlyingIdle, movingToB, isPreparing, isSwooping;
    public override void _Ready()
    {
        base._Ready();
        currentMoveState = moveState.flyidle;
        isFlyingIdle = true;
        pointA = new Vector2(GlobalPosition.X , GlobalPosition.Y- idleRange);
        pointB = new Vector2(GlobalPosition.X , GlobalPosition.Y+ idleRange);
        pointC = new Vector2 (GlobalPosition.X -swoopRange , GlobalPosition.Y);
        pointD = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(currentMoveState == moveState.flyidle)
        {
            Fly(delta, pointA, pointB);
        }else if (currentMoveState == moveState.prepare)
        {
            enemyVelocity = Vector2.Zero;
        }
        else if(currentMoveState == moveState.swoopattack)
        {
            if(swoopCount < 2)
            {
                Fly(delta, pointD, pointC);
                HandleSwoopingMovement(delta);
                
            }
            else
            {
                swoopCount = 0;
                isSwooping = false;
                currentMoveState = moveState.flyidle;
            }
            
        }
        if(enemyVelocity.X > 0){enemySprite.FlipH = true;}
        else{enemySprite.FlipH = false;}
        Velocity = enemyVelocity;
        MoveAndSlide();
        AnimateEnemy();
        
    }


    private void Fly(double delta, Vector2 point1, Vector2 point2)
    {
        target = movingToB ? point2 : point1;

        toTarget = target - GlobalPosition;
        distance = toTarget.Length();

        if (distance <= 1.0f)
        {
            if(currentMoveState == moveState.swoopattack){swoopCount++;}
            GlobalPosition = target;
            enemyVelocity = Vector2.Zero;
            movingToB = !movingToB;
            return;
        }

        direction = toTarget.Normalized();

        float targetSpeed = moveSpeed;

        if (distance < turnDistance)
        {
            targetSpeed = moveSpeed * (distance / turnDistance);
        }

        Vector2 targetVelocity = direction * targetSpeed;


        enemyVelocity = enemyVelocity.MoveToward(
            targetVelocity,
            acceleration * (float)delta
        );
    }
    private void AnimateEnemy()
    {
        enemyAnim.Play(currentMoveState.ToString());
    }



    private void HandleSwoopingMovement(double delta)
    {
        enemyVelocity.Y += swoopDepth * distance * (float)delta;
    }


    private void OnPlayerDetectorBodyEntered(Node2D body)
    {
        if(body.IsInGroup("Player") && !isPreparing && !isSwooping)
        {
            currentMoveState = moveState.prepare;
            enemyAnim.Play("prepare");
            isPreparing = true;
        }
    }

    private void OnPrepareAnimationFinished()
    {
        currentMoveState = moveState.swoopattack;
        isPreparing = false;
        isSwooping = true;
    }


    private void OnEdibleEnemyConsumed()    /////////////FOR EDIBLE ENEMIES
    {
        QueueFree();
    }


}
