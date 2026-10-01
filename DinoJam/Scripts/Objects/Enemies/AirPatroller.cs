using Godot;
using System;
using System.Reflection.Metadata;

public partial class AirPatroller : Enemy
{
    [Export] public float moveSpeed, leftDistance, rightDistance, acceleration, frequency, amplitude, waveDepth;
    //[Export] private float  moveRadius;
    [Export] Label debugLabel;
    private Vector2 target, targetY, originPosition, targetVelocity, leftBoundary, rightBoundary, lowerBoundary, upperBoundary;
    private Rect2 moveBounds;
    //private float xDistance, yDistance, xDirection, yDirection;
    private int directionalMult = 1;

    private float  waveOffset, distance, distanceY;
    private bool movingToB, movingToX, hasTarget;
    public override void _Ready()
    {
        base._Ready();
        originPosition = GlobalPosition;
        leftBoundary = new Vector2(GlobalPosition.X - leftDistance * 16, GlobalPosition.Y);
        rightBoundary = new Vector2(GlobalPosition.X + rightDistance * 16, GlobalPosition.Y);
        upperBoundary = new Vector2(GlobalPosition.X , GlobalPosition.Y - waveDepth);
        lowerBoundary = new Vector2(GlobalPosition.X , GlobalPosition.Y + waveDepth);
        attacker.AttackFinished += OnAttackFinished;

        //target = GetRandomPosition();
    }

    public override void _PhysicsProcess(double delta)
    {
        DetermineBehavior(delta);
        Velocity = enemyVelocity;
        MoveAndSlide();
        debugLabel.Text = currentMoveState.ToString();
    }

    private void DetermineBehavior(double delta)
    {
        switch (currentMoveState)
        {
            case enemyMoveState.move:
                HandleMovement(delta);
                break;
            case enemyMoveState.prepare:
                HandleMovement(delta);
                break;
            case enemyMoveState.stun:
                enemyVelocity = Vector2.Zero;
                break;
            case enemyMoveState.attack:
                HandleMovement(delta);
                attacker.Attack(myPlayer.GlobalPosition, enemyVelocity);
                break;
            case enemyMoveState.hurt:

                break;
            case enemyMoveState.dying:

                break;
        }
    }

    private void HandleMovement(double delta)
    {
        // if(Mathf.Abs(GlobalPosition.X - originPosition.X) > 60 && Mathf.Abs(GlobalPosition.Y - originPosition.Y) > 60)
        // {
        //     target = originPosition;
        //     enemyVelocity = enemyVelocity.MoveToward(originPosition, acceleration * (float)delta);
        //     return;

        // }

        target = movingToB ? rightBoundary : leftBoundary;
        distance = target.X - GlobalPosition.X;

        if (Mathf.Abs(distance) <= 1)
        {
            target = new Vector2(target.X, GlobalPosition.Y);
            GlobalPosition = target;
            enemyVelocity.X = 0;
            movingToB = !movingToB;
            return;
        }
             
        float targetVelocity = (float)delta *distance * moveSpeed;
        enemyVelocity.X = Mathf.MoveToward(enemyVelocity.X, targetVelocity, acceleration * (float)delta);

        // targetY = movingToX ? lowerBoundary : upperBoundary;
        // distanceY = target.Y - GlobalPosition.Y;

        // if(Mathf.Abs(distanceY) <= 1)
        // {
        //     targetY = new Vector2(GlobalPosition.X, targetY.Y);
        //     GlobalPosition = target;
        //     enemyVelocity.Y = 0;
        //     movingToX = !movingToX;
        //     return;
        // }
        
        //  float targetAmplitude = (float)delta *distanceY * moveSpeed;
        // enemyVelocity.Y = Mathf.MoveToward(enemyVelocity.Y, targetAmplitude, acceleration * (float)delta);

        if (enemyVelocity.Y < -waveDepth)
        {
            directionalMult = 1;
            enemyVelocity. Y = -waveDepth;
        }
        if(enemyVelocity.Y > waveDepth)
        {
            directionalMult = -1;
            enemyVelocity.Y = waveDepth;
        }
        enemyVelocity.Y += (float)delta *  directionalMult * amplitude;
    }

    private void OnAttackFinished()
    {
        
        if(currentMoveState != enemyMoveState.hurt && currentMoveState != enemyMoveState.dying)
        {
            currentMoveState = enemyMoveState.move;
        }
       
    }
}

