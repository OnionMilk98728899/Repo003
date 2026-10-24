using Godot;
using System;

public partial class GroundPatrolAction : BossAction
{
    private BossController boss;
    [Export] float moveSpeed;
    [Export] private Node2D leftBoundaryNode, rightBoundaryNode;
    [Export] private Node2D bossSpriteRoot;
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

            // if(bossVelocity.X < 0 && facingRight || bossVelocity.X > 0 && !facingRight)
            // {
            //    bossSpriteRoot.Scale = new Vector2(facingRight ? 1 : -1, 1); 
            //    facingRight = bossSpriteRoot.Scale.X < 0;
            // }
            myBoss.FlipBossHorizontally(bossVelocity);
            Patrol();
            ApplyGravity(myBoss);

        }
    }


    // public override bool IsFinished()
    // {
    //     return isFinished;
    // }

    private void Patrol()
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
        bossVelocity = new Vector2(direction * moveSpeed, bossVelocity.Y);
    }


}