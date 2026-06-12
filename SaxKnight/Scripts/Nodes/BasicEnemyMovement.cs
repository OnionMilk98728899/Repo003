using Godot;
using System;

public partial class BasicEnemyMovement : Node2D
{
    [Signal] public delegate void EnterJamModeEventHandler(bool isInJamMode);
    [Export] private CharacterBody2D enemyBody;
    [Export] private AnimationPlayer enemyAnim;
    [Export] private Sprite2D enemySprite;
    [Export] private float speed, maxSpeed;
    private CharacterBody2D targetBody;
    private Vector2 moveDirection, enemyVelocity;
    private enum direction { left, right }
    private direction currentDirection;
    private enum moveState
    {
        Walk, Idle, Jam, Attack, Hurt, Death
    }
    private moveState currentMoveState;



    public override void _Ready()
    {
        RandomizeInitialPath();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (currentMoveState == moveState.Walk || currentMoveState == moveState.Idle)
        {
            DetermineEnemyDirection();
            Patrol(delta);
        }
        else if (currentMoveState == moveState.Jam || currentMoveState == moveState.Attack)
        {
            StopMoving();
            if(targetBody != null)
            {
                FaceTarget(targetBody.GlobalPosition);
            }
            
        }

        AnimateEnemy();
        enemyBody.MoveAndSlide();
    }

    private void RandomizeInitialPath()
    {
        int r = GD.RandRange(0, 100);
        if (r < 50)
        {
            currentDirection = direction.left;
        }
        else
        {
            currentDirection = direction.right;
        }
    }

    private void DetermineEnemyDirection()
    {
        if (currentDirection == direction.left)
        {
            moveDirection.X = -1;
        }
        else if (currentDirection == direction.right)
        {
            moveDirection.X = 1;
        }

        if (moveDirection.X > 0)
        {
            enemySprite.FlipH = false;
        }
        else
        {
            enemySprite.FlipH = true;
        }
    }

    private void Patrol(double delta)
    {
        enemyVelocity.X = moveDirection.X * speed;
        enemyBody.Velocity = enemyVelocity;
    }

    private void StopMoving()
    {
        enemyVelocity.X = 0;
        enemyBody.Velocity = enemyVelocity;
    }

    private void BecomeAlertedToPlayer(Node2D body)
    {
        currentMoveState = moveState.Jam;
        StopMoving();
        EmitSignal(SignalName.EnterJamMode, true);
    }

    private void FaceTarget(Vector2 t)
    {
        if (enemyBody.GlobalPosition.X - t.X < 0)
        {
            enemySprite.FlipH = false;
        }
        else
        {
            enemySprite.FlipH = true;
        }
    }

    private void AnimateEnemy()
    {
        enemyAnim.Play(currentMoveState.ToString());
    }

    private void OnLeftPatrolBoundaryEntered(Node2D body)
    {
        currentDirection = direction.right;
        enemyVelocity.X = 0;
    }

    private void OnRightPatrolBoundaryEntered(Node2D body)
    {
        currentDirection = direction.left;
        enemyVelocity.X = 0;
    }

    private void OnPlayerDetectorBodyEntered(Node2D body)
    {
        BecomeAlertedToPlayer(body);
        targetBody = body.GetNode<CharacterBody2D>(".");
    }

    private void OnPlayerDetectorExited(Node2D body)
    {
        currentMoveState = moveState.Walk;
    }

    private void OnEnterAttackMode()
    {
        currentMoveState = moveState.Attack;
    }

}
