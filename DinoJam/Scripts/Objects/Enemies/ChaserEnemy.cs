using Godot;
using System;

//public enum attackType{ranged, contact}
public partial class ChaserEnemy : GroundEnemy
{
    // [Export] private RayCast2D edgeCheckerLeft, edgeCheckerRight;
    // [Export] private Timer turnBuffer;
    // [Export] private float jumpPower;
    // [Export] private AnimationPlayer chaserAnim;
    
    // private bool isJumping,hasJumped,  isTargetLocked;

    // public override void _Ready()
    // {
    //     base._Ready();
    //     DetermineRandomDirection();
    // }

    // public override void HandleMovement()
    // {
    //     enemyVelocity = new Vector2(direction * moveSpeed, enemyVelocity.Y);
    //     if (turnBuffer.IsStopped())
    //     {
    //         if (IsOnWall())
    //         {
    //             direction *= -1;
    //             turnBuffer.Start();
    //         }
    //     }

    //     if (IsOnFloor())
    //     {
    //         if(enemyVelocity.X < 0 && !edgeCheckerLeft.IsColliding() || enemyVelocity.X > 0 && !edgeCheckerRight.IsColliding())
    //         {
    //             GD.Print("On floor and edge checker not colliding");
    //             enemyVelocity.Y -= jumpPower;
    //             isJumping = true;
    //             if (!hasJumped)
    //             {
    //                 currentMoveState = enemyMoveState.jump;
    //                 hasJumped = true;
    //                 GD.Print("Calling jump");
    //             }
                
    //         }
    //         else
    //         {
    //             isJumping = false;
    //         }
    //     }
    // }

    // private void DetermineRandomDirection()
    // {
    //     float rand = GD.Randf();

    //     if (rand > .5)
    //     {
    //         direction = -1;
    //     }
    //     else
    //     {
    //         direction = 1;
    //     }
    // }

    // public override void AttackPlayer()
    // {
    //     if(myPlayer != null)
    //     {
    //         if (!isTargetLocked)
    //         {
    //             target = myPlayer.GlobalPosition;
    //             isTargetLocked = true;
    //         }
    //     }
    // }

    // public override void DetermineBehavior()
    // {
    //     switch (currentMoveState)
    //     {
    //         case enemyMoveState.move:
    //             HandleMovement();
    //             break;
    //         case enemyMoveState.prepare:
    //             Prepare();
    //             break;
    //         case enemyMoveState.attack:
    //             AttackPlayer();
    //             break;
    //         case enemyMoveState.hurt:

    //             break;
    //         case enemyMoveState.dying:

    //             break;
    //     }
    // }

    // private void Prepare()
    // {
    //     if (IsOnFloor())
    //     {
    //         enemyVelocity.X = 0;
    //     }
    // }


    // public override void ApplyGravity()
    // {
    //     if (!IsOnFloor())
    //     {
    //         if (enemyVelocity.Y < maxGravity)
    //         {
    //             enemyVelocity.Y += gravity;
    //         }
    //     }
    //     else
    //     {
    //         if (!isJumping)
    //         {
    //             enemyVelocity.Y = 0;
    //             if(currentMoveState == enemyMoveState.jump){currentMoveState = enemyMoveState.move;}
    //         }
    //     }
    // }

    // public override void AnimateEnemy()
    // {
    //     if(currentMoveState != enemyMoveState.jump && currentMoveState != enemyMoveState.dying)
    //     {
    //         enemyAnim.Play(currentMoveState.ToString());
    //     }
    //     else
    //     {
    //         chaserAnim.Play(currentMoveState.ToString());
    //     }
        
    // }

    // private void OnJumpAnimationFinished()
    // {
        
    //     hasJumped = false;
    //     currentMoveState = enemyMoveState.move;
    // }

}
