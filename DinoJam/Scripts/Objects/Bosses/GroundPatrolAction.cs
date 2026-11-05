using Godot;
using System;

public enum moveType
{
    walk, swim, fly, run
}
public partial class GroundPatrolAction : BossAction
{
    private BossController boss;
    [Export] float walkSpeed, swimSpeed, runSpeed, flySpeed, acceleration, slowDownDistance;
    [Export] private Node2D leftBoundaryNode, rightBoundaryNode;
    [Export] private Node2D bossSpriteRoot;
    [Export] public moveType myMoveType;
    [Export] public bool isSeekingPlayer;
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

        actionAnim.Play(myMoveType.ToString());
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isActive)
        {
            switch (myMoveType)
            {
                case moveType.walk:
                    Patrol(walkSpeed, delta);
                    break;
                case moveType.run:
                    Patrol(runSpeed, delta);
                    break;
                case moveType.swim:
                    Patrol(swimSpeed, delta);
                    break;
                case moveType.fly:
                    Patrol(flySpeed, delta);
                    break;
            }
            myBoss.FlipBossHorizontally(bossVelocity);
            //Patrol(walkSpeed, delta);
            ApplyGravity(myBoss);

        }
    }
    private void Patrol(float speed, double delta)
    {
        if (!isSeekingPlayer)
        {
            target = movingToB ? rightBoundary : leftBoundary;
        }
        else
        {
            target = GlobalStats.Instance.playerLocation;
        }

        distance = target.X - GlobalPosition.X;

        if (Mathf.Abs(distance) <= 0.5f)
        {
            if (!isSeekingPlayer)
            {
                target = new Vector2(target.X, GlobalPosition.Y);
                GlobalPosition = target;
                bossVelocity = Vector2.Zero;
                movingToB = !movingToB;
                return;
            }
            else
            {
                GD.Print("Initiate AttackMove!");
            }

        }

        float direction = Mathf.Sign(distance);

        if (myMoveType == moveType.walk || myMoveType == moveType.run)
        {
            bossVelocity = new Vector2(direction * speed, bossVelocity.Y);
        }
        else if (myMoveType == moveType.swim || myMoveType == moveType.fly)
        {
            bossVelocity = new Vector2(direction * speed, bossVelocity.Y);
            //bossVelocity = new Vector2(direction * direction * speed, bossVelocity.Y);
        }

    }


}