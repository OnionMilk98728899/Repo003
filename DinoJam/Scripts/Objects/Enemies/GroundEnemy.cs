using Godot;
using System;

public partial class GroundEnemy : Enemy
{
    //[Export]protected EnemyAttacker attacker;
    // [Export]public float moveSpeed, gravity, maxGravity;
    // [Export] private float leftDistance, rightDistance;
    // [Export] public AnimationPlayer enemyAnim;
    // [Export] private Sprite2D enemySprite;
    // [Export] private Label debugLabel;
    // private float distance;
    // public float direction;
    // public Vector2 target;
    // private Vector2 initialPosition, leftBoundary, rightBoundary;
    // private bool isPatrolling, movingToB, hasTarget;
    // public override void _Ready()
    // {
    //     base._Ready();
    //     movingToB = true;
    //     leftBoundary = new Vector2(GlobalPosition.X - leftDistance, GlobalPosition.Y);
    //     rightBoundary = new Vector2(GlobalPosition.X + rightDistance, GlobalPosition.Y);
    // }

    // public override void _PhysicsProcess(double delta)
    // {
    //     DetermineBehavior();
    //     ApplyGravity();
    //     Velocity = enemyVelocity;
    //     MoveAndSlide();
    //     AnimateEnemy();
    //     if (currentMoveState != enemyMoveState.hurt)
    //     {
    //         if (enemyVelocity.X > 0) { enemySprite.FlipH = true; }
    //         else if (enemyVelocity.X < 0) { enemySprite.FlipH = false;}
    //     }
    //     debugLabel.Text = currentMoveState.ToString();
    // }

    // public virtual void DetermineBehavior()
    // {
    //     switch (currentMoveState)
    //     {
    //         case enemyMoveState.move:
    //             HandleMovement();
    //             break;
    //         case enemyMoveState.prepare:
    //             enemyVelocity = Vector2.Zero;
    //             break;
    //         case enemyMoveState.attack:
    //             AttackPlayer();
    //             break;
    //         case enemyMoveState.hurt:
    //             DampXMovement();
    //             break;
    //         case enemyMoveState.dying:
    //             DampXMovement();
    //             SpinEnemySprite();
    //             break;
    //     }
    // }

    // private void DampXMovement()
    // {
    //     if (enemyVelocity.X > 0)
    //     {
    //         enemyVelocity.X -= .1f * moveSpeed;
    //     }
    //     else
    //     {
    //         enemyVelocity.X += .1f * moveSpeed;
    //     }
    // }

    // private void SpinEnemySprite()
    // {
    //     if(enemyVelocity.X < 0)
    //     {
    //         enemySprite.RotationDegrees -=10;
    //     }
    //     else
    //     {
    //         enemySprite.RotationDegrees +=10;
    //     }
        
    // }
    // public virtual void HandleMovement()
    // {
    //     target = movingToB ? rightBoundary : leftBoundary;
    //     distance = target.X - GlobalPosition.X;

    //     if (Mathf.Abs(distance) <= 0.5f)
    //     {
    //         GlobalPosition = target;
    //         enemyVelocity = Vector2.Zero;
    //         movingToB = !movingToB;
    //         return;
    //     }

    //     float direction = Mathf.Sign(distance);
    //     enemyVelocity = new Vector2(direction * moveSpeed, enemyVelocity.Y);
    // }

    // public virtual void AttackPlayer()
    // {
    //     if (!hasTarget)
    //     {
    //         target = myPlayer.GlobalPosition;
    //         hasTarget = true;
    //     }

    //     if (Mathf.Abs(distance) <= 0.5f)
    //     {
    //         currentMoveState = enemyMoveState.move;
    //     }

    //     distance = target.X - GlobalPosition.X;
    //     direction = Mathf.Sign(distance);
    //     enemyVelocity = new Vector2(direction * moveSpeed, 0);
    // }

    // public virtual void ApplyGravity()
    // {
    //     if (!IsOnFloor())
    //     {
    //         if (enemyVelocity.Y < maxGravity)
    //         {
    //             enemyVelocity.Y += gravity;
    //             GD.Print("Applying Gravity and Y value is " + enemyVelocity.Y);
    //         }
    //     }
    //     else
    //     {
    //         if (!isBounced)
    //         {
    //             GD.Print("Ground Enemy Zeroing jump");
    //             enemyVelocity.Y = 0;
    //         }
            
    //     }
    // }
    // private void OnPrepareAnimationFinished()
    // {
    //     currentMoveState = enemyMoveState.attack;
    // }

    // private void OnAttackAnimationFinished()
    // {
    //     currentMoveState = enemyMoveState.move;
    // }

    // public virtual void AnimateEnemy()
    // {
    //     if (currentMoveState != enemyMoveState.dying)
    //     {
    //         enemyAnim.Play(currentMoveState.ToString());
    //     }
    //     else
    //     {
    //         enemyAnim.Play("hurt");
    //         SetCollisionMaskValue(1, false);
    //     }

    // }

}
