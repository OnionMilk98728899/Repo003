using Godot;
using System;
using System.Drawing;

public partial class FlyingEnemy : Enemy
{
    // [Export] private float moveSpeed, attackSpeed, vertRange, horizRange;
    // [Export] private AnimationPlayer enemyAnim;
    // [Export] private Label debugLabel;
    // [Export] private Sprite2D enemySprite;
    // private float distance;
    // private Vector2 randomTarget, playerTarget;
    // private Rect2 enemyBounds;
    // private bool isMoving, movingToB, isPreparing, isAttacking, targetSet;
    // public override void _Ready()
    // {
    //     base._Ready();
    //     SetEnemyBounds();
    // }

    // public override void _PhysicsProcess(double delta)
    // {
    //     DetermineBehavior();
    //     if (enemyVelocity.X > 0) { enemySprite.FlipH = true; }
    //     else { enemySprite.FlipH = false; }
    //     Velocity = enemyVelocity;
    //     MoveAndSlide();
    //     AnimateEnemy();
    //     //debugLabel.Text = enemyVelocity.ToString();
    // }

    // private void SetEnemyBounds()
    // {
    //     Vector2 topLeft = new Vector2(GlobalPosition.X - horizRange * 16, GlobalPosition.Y - vertRange * 16);
    //     Vector2 bottomRight = new Vector2(GlobalPosition.X + horizRange * 16, GlobalPosition.Y + vertRange * 16);

    //     enemyBounds = new Rect2(topLeft, bottomRight - topLeft);
    // }

    // private void DetermineBehavior()
    // {
    //     switch (currentMoveState)
    //     {
    //         case enemyMoveState.move:
    //             Fly(moveSpeed, randomTarget);
    //             break;
    //         case enemyMoveState.prepare:
    //             enemyVelocity = Vector2.Zero;
    //             break;
    //         case enemyMoveState.attack:
    //             Fly(attackSpeed, playerTarget);
    //             break;
    //         case enemyMoveState.hurt:
    //             break;
    //         case enemyMoveState.dying:
    //             break;
    //     }

    //     Velocity = enemyVelocity;
    //     MoveAndSlide();
    // }
    // private Vector2 GenerateRandomTarget()
    // {
    //     return randomTarget = new Vector2(
    //     (float)GD.RandRange(enemyBounds.Position.X, enemyBounds.End.X),
    //         (float)GD.RandRange(enemyBounds.Position.Y, enemyBounds.End.Y)
    //     );
    // }

    // private void Fly(float speed, Vector2 target)
    // {
    //     if (currentMoveState == enemyMoveState.move )
    //     {
    //         if(distance <= 1.0f || randomTarget == Vector2.Zero)
    //         {
    //              randomTarget = GenerateRandomTarget();
    //         }
           
    //     }else if(currentMoveState == enemyMoveState.attack && !targetSet)
    //     {
    //         playerTarget = myPlayer.GlobalPosition;
    //         targetSet = true;
    //     }

    //     if(currentMoveState == enemyMoveState.attack && distance <= 1.0f)
    //     {
    //         currentMoveState = enemyMoveState.move;
    //         targetSet = false;
    //         randomTarget = GenerateRandomTarget();
    //     }

    //     float direction = Mathf.Sign(distance);
    //     enemyVelocity = GlobalPosition.DirectionTo(target) * speed;
        

    //     Vector2 toTarget = target - GlobalPosition;
    //     distance = toTarget.Length();

    // }
    // private void AnimateEnemy()
    // {
    //     enemyAnim.Play(currentMoveState.ToString());
    // }



    // // private void HandleSwoopingMovement(double delta)
    // // {

    // // }


    // // private void OnPlayerDetectorBodyEntered(Node2D body)
    // // {
    // //     if (body.IsInGroup("Player") && !isPreparing && !isSwooping)
    // //     {
    // //         currentMoveState = enemyMoveState.prepare;
    // //         enemyAnim.Play("prepare");
    // //         isPreparing = true;
    // //     }
    // // }

    // private void OnPrepareAnimationFinished()
    // {
    //     currentMoveState = enemyMoveState.attack;
    //     isPreparing = false;
    //     isAttacking = true;
    // }


    // private void OnEdibleEnemyConsumed()    /////////////FOR EDIBLE ENEMIES
    // {
    //     QueueFree();
    // }


}
