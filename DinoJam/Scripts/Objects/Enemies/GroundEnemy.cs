using Godot;
using System;

public partial class GroundEnemy : Enemy
{
    [Export] private float moveSpeed, gravity, leftDistance, rightDistance;
    [Export] private AnimationPlayer enemyAnim;
    [Export] private Sprite2D enemySprite;
    private float distance;
    public enum moveState { walk, prepare, attack, hurt, dying }
    public moveState currentMoveState;
    private Vector2 enemyVelocity, target, initialPosition, leftBoundary, rightBoundary;
    private bool isPatrolling, movingToB;
    public override void _Ready()
    {
        base._Ready();
        movingToB = true;
        leftBoundary = new Vector2(GlobalPosition.X - leftDistance, GlobalPosition.Y);
        rightBoundary = new Vector2(GlobalPosition.X + rightDistance, GlobalPosition.Y);
    }

    public override void _PhysicsProcess(double delta)
    {

        HandleMovement(delta);
        ApplyGravity();
        Velocity = enemyVelocity;
        MoveAndSlide();
    }

    private void HandleMovement(double delta)
    {
        target = movingToB ? rightBoundary : leftBoundary;
        distance = target.X - GlobalPosition.X;

        if (Mathf.Abs(distance) <= 0.5f)
        {
            GlobalPosition = target;
            enemyVelocity = Vector2.Zero;
            movingToB = !movingToB;
            return;
        }

        float direction = Mathf.Sign(distance);
        enemyVelocity = new Vector2(direction * moveSpeed, 0);
    }

    private void ApplyGravity()
    {
        if (!IsOnFloor())
        {
            enemyVelocity.Y += gravity;
        }
        else
        {
            enemyVelocity.Y = 0;
        }
    }

    private void AnimateEnemy()
    {

    }

}
