using Godot;
using System;

public enum moveType
{
    walk, swim, fly, run
}
public partial class GroundPatrolAction : BossAction
{
    private BossController boss;
    [Export] float walkSpeed, swimSpeed, runSpeed, acceleration, slowDownDistance;
    [Export] private Node2D leftBoundaryNode, rightBoundaryNode;
    [Export] private Node2D bossSpriteRoot;
    [Export] public moveType myMoveType;
    private Vector2 leftBoundary, rightBoundary, target;
    //private BossController myBoss;
    private bool isFinished, movingToB, facingLeft, facingRight;
    private float distance;

    public override void _Ready()
    {
        leftBoundary = leftBoundaryNode.GlobalPosition;
        rightBoundary = rightBoundaryNode.GlobalPosition;

    }

    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        actionAnim.Play("walk");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isActive)
        {
            myBoss.FlipBossHorizontally(bossVelocity);
            Patrol(walkSpeed, delta);
            ApplyGravity(myBoss);

        }
    }
    private void Patrol(float speed, double delta)
    {
        target = movingToB ? rightBoundary : leftBoundary;
        distance = target.X - GlobalPosition.X;

        if (Mathf.Abs(distance) <= 0.5f)
        {
            target = new Vector2(target.X, GlobalPosition.Y);
            GlobalPosition = target;
            bossVelocity = Vector2.Zero;
            movingToB = !movingToB;
            return;
        }

        float direction = Mathf.Sign(distance);

        if (myMoveType == moveType.walk || myMoveType == moveType.run)
        {
            bossVelocity = new Vector2(direction * speed, bossVelocity.Y);
        }
        else if (myMoveType == moveType.swim || myMoveType == moveType.fly)
        {

            float speedFactor = Mathf.Min(1.0f, Mathf.Abs(distance) / slowDownDistance);
            float targetSpeed = direction * speed * speedFactor;

            bossVelocity.X = Mathf.MoveToward(bossVelocity.X, targetSpeed, acceleration * (float)delta);
        }

    }


}