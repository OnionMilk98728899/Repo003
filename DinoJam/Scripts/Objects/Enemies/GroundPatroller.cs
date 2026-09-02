using Godot;
using System;

public partial class GroundPatroller : Enemy
{
    [Export] public float moveSpeed, gravity, maxGravity;
    [Export] private float leftDistance, rightDistance;
    [Export] public AnimationPlayer enemyAnim;
    [Export] private Sprite2D enemySprite;
    [Export] private Label debugLabel;
    private Vector2 target, leftBoundary, rightBoundary;
    private float distance;
    private bool movingToB;
    public override void _Ready()
    {
        base._Ready();
        leftBoundary = new Vector2(GlobalPosition.X - leftDistance * 16, GlobalPosition.Y);
        rightBoundary = new Vector2(GlobalPosition.X + rightDistance * 16, GlobalPosition.Y);
        attacker.AttackFinished += OnAttackFinished;
    }

    public override void _PhysicsProcess(double delta)
    {
        DetermineBehavior();
        ApplyGravity();
        Velocity = enemyVelocity;
        MoveAndSlide();
        debugLabel.Text = currentMoveState.ToString();
    }

    private void DetermineBehavior()
    {
        switch (currentMoveState)
        {
            case enemyMoveState.move:
                HandleMovement();
                break;
            case enemyMoveState.prepare:
                enemyVelocity = Vector2.Zero;
                break;
            case enemyMoveState.attack:
                enemyVelocity = attacker.Attack(myPlayer.GlobalPosition, enemyVelocity);
                break;
            case enemyMoveState.hurt:

                break;
            case enemyMoveState.dying:

                break;
        }
    }

    private void DampXMovement()
    {
        if (enemyVelocity.X > 0)
        {
            enemyVelocity.X -= .1f * moveSpeed;
        }
        else
        {
            enemyVelocity.X += .1f * moveSpeed;
        }
    }
    private void HandleMovement()
    {
        target = movingToB ? rightBoundary : leftBoundary;
        distance = target.X - GlobalPosition.X;

        if (Mathf.Abs(distance) <= 0.5f)
        {
            target = new Vector2(target.X, GlobalPosition.Y);
            GlobalPosition = target;
            enemyVelocity = Vector2.Zero;
            movingToB = !movingToB;
            return;
        }

        float direction = Mathf.Sign(distance);
        enemyVelocity = new Vector2(direction * moveSpeed, enemyVelocity.Y);
    }

    private void ApplyGravity()
    {
        if (!IsOnFloor())
        {
            if (enemyVelocity.Y < maxGravity)
            {
                enemyVelocity.Y += gravity;
            }
        }
        else
        {
            if (!isBounced && enemyVelocity.Y != 0)
            {
                enemyVelocity.Y = 0;
            }

        }
    }

    private void OnAttackFinished()
    {
        currentMoveState = enemyMoveState.move;
    }
}
