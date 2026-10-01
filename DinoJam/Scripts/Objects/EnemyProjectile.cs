using Godot;
using System;

public enum projectileType { spear, ball, rock, bomb }
public partial class EnemyProjectile : CharacterBody2D
{
    public projectileType myProjType;
    [Export] private Sprite2D projSprite;
    [Export] private Label debugLabel;
    [Export] private Timer destroyTimer;
    private float moveSpeed, elapsedTime, ascentDuration = 40, descentDuration = 40, arcHeight = 25, arcDuration = .5f, gravity = 30;
    private Vector2 target, projVelocity, direction, originPosition, myPosition;

    public override void _Ready()
    {
        myProjType = projectileType.spear;
        // Engine.TimeScale = .2;


    }

    public override void _PhysicsProcess(double delta)
    {
        ConfigureFlightBehavior(delta);
        Rotate();
        Velocity = projVelocity;
        MoveAndSlide();
        if (IsOnFloor())
        {
            destroyTimer.Start();
        }
        if (target.X - GlobalPosition.X <= .2 && target.Y - GlobalPosition.Y <= .2)
        {
            destroyTimer.Start();
        }

    }


    public void SetStats(Vector2 t, float speed)
    {
        target = t;
        moveSpeed = speed;
        direction = (target - GlobalPosition).Normalized();
        originPosition = GlobalPosition;
    }

    private void ConfigureFlightBehavior(double delta)
    {
        switch (myProjType)
        {
            case projectileType.spear:
                FlyInArc(delta);
                break;
            case projectileType.ball:
                break;
            case projectileType.rock:
                break;
            case projectileType.bomb:
                break;
        }
    }

    private void FlyStraight()
    {
        if (target != Vector2.Zero)
        {
            projVelocity = direction * moveSpeed;
        }
        else
        {
            debugLabel.Text = "Not found!";
        }
    }

    private void FlyInArc(double delta)
    {
        elapsedTime += (float)delta;

        float t = elapsedTime / arcDuration;

        //float t = Mathf.Clamp(elapsedTime / arcDuration, 0.0f, 1.0f);

        Vector2 desiredPosition = originPosition.Lerp(target, t);

        desiredPosition.Y -= 4.0f * arcHeight * t * (1.0f - t);

        projVelocity = (desiredPosition - GlobalPosition) / (float)delta;
    }

    private void Rotate()
    {
        if (myProjType == projectileType.spear)
        {
            if (projVelocity.LengthSquared() > 0)
            {
                Rotation = projVelocity.Angle();
            }
        }
    }

    private void OnDestroyTimerTimeout()
    {
        QueueFree();
    }

}
