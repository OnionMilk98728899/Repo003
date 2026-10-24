using Godot;
using System;

public partial class RageAction : BossAction
{
    [Export] RayCast2D leftCeilingChecker, rightCeilingChecker;
    [Export] Node2D leftCeilingLimit, rightCeilingLimit;
    [Export] private Timer screenShakeTimer;
    [Export] private HurtBox mouthHurtbox;

    private Vector2 leftCeilingBoundary, rightCeilingBoundary;

    public override void _Ready()
    {
        // leftCeilingBoundary = leftCeilingChecker.GetCollisionPoint();
        // rightCeilingBoundary = rightCeilingChecker.GetCollisionPoint();
        leftCeilingBoundary = leftCeilingLimit.GlobalPosition;
        rightCeilingBoundary = rightCeilingLimit.GlobalPosition;
        GD.Print("Left: " +leftCeilingBoundary + " right : "+ rightCeilingBoundary);
    }
    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        actionAnim.Play("rage");
        ExposeMouthHurtbox(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isActive)
        {
            // if (leftCeilingChecker.IsColliding())
            // {
            //     leftCeilingBoundary = leftCeilingChecker.GetCollisionPoint();
            // }
            // if (rightCeilingChecker.IsColliding())
            // {
            //     rightCeilingBoundary = rightCeilingChecker.GetCollisionPoint();

            // }
            // if (leftCeilingChecker.IsColliding() && rightCeilingChecker.IsColliding())
            // {
                if (screenShakeTimer.IsStopped())
                {
                    screenShakeTimer.Start();
                    EventBus.Instance.EmitSignal(EventBus.SignalName.ScreenShake, 3, 3);
                    GenerateCeilingProjectile();
                }

            //}
        }
    }


    private void GenerateCeilingProjectile()
    {
        double difference = Mathf.Abs(leftCeilingBoundary.X - rightCeilingBoundary.X);
        double randSpot = GD.RandRange(0, difference);
        Vector2 position = new Vector2(leftCeilingBoundary.X + (float)randSpot, leftCeilingBoundary.Y);
        
        ProjectileManager.Instance.GenerateCeilingCrumble(position, 2);
    }

    private void ExposeMouthHurtbox(bool exposed)
    {
        mouthHurtbox.SetCollisionMaskValue(6,exposed);
    }

    public override void OnActionTimerTimeout()
    {
        base.OnActionTimerTimeout();
        ExposeMouthHurtbox(false);
    }
}