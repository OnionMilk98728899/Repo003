using Godot;
using System;
using System.Data;

public partial class EnemyMovement : Node2D
{
	[Signal] public delegate void DestroyEnemyEventHandler();
	[Export] private EnemyAttack enemyAttack;
	[Export] private EnemyData enemyData;
	[Export] private CharacterBody2D enemyBody;
	[Export] private AnimationPlayer enemyAnim;
	[Export] private Sprite2D enemyShadow;
	[Export] private float moveSpeed, playerAggression, attackRange;
	private Vector2 enemyVelocity;
	private CharacterBody2D playerBody;
	private Vector2 target, direction, targetOffset, knockbackDirection;
	private bool isTargetingPlayer, isHurt, isDying, isPreparing, isAttacking, hasTargetOffset;
	public enum enemyDirection { up, down, left, right }
	public enum enemyState { move, prepare, hurt, death, attack, leap}
	[Export] enemyState attackAnimState;
	public enum enemyTarget { player, wall, none }
	public enemyState currentState;
	public enemyDirection currentDirection;



	public override void _Ready()
	{
		DetermineTarget();
		DetermineEnemyDirection();
		AnimateEnemy();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAttacking)
		{
			MoveEnemy();
			DetermineEnemyDirection();
			AnimateEnemy();
			enemyBody.MoveAndSlide();
		}
		else
		{
			enemyAttack.Attack(playerBody.GlobalPosition, delta);
			AnimateEnemy();
		}

		
	}

	private void DetermineTarget()
	{
		float rand = GD.Randf() * 100;
		if (playerAggression > rand)
		{
			playerBody = GameManager.Instance.player.GetPlayerBody();
			isTargetingPlayer = true;
		}
	}

	private Vector2 GetRandomTargetOffset(float distance)
	{
		float angle = (float)GD.RandRange(0.0, Mathf.Tau);

		return new Vector2(
			Mathf.Cos(angle),
			Mathf.Sin(angle)
		) * distance;
	}

	private void MoveEnemy()
	{

		if (!hasTargetOffset)
		{
			targetOffset = GetRandomTargetOffset(50.0f);
			hasTargetOffset = true;
		}

		if (isTargetingPlayer && !isPreparing)
		{
			direction = (playerBody.GlobalPosition + targetOffset - enemyBody.GlobalPosition).Normalized();
			enemyVelocity = direction * moveSpeed;
			currentState = enemyState.move;

			if (isHurt)
			{
				enemyVelocity = knockbackDirection * moveSpeed / 2;
				currentState = enemyState.hurt;
			}
			if (isDying)
			{
				enemyVelocity = Vector2.Zero;
				currentState = enemyState.death;
			}
		}
		else   //// targeting wall
		{

		}

		if (Mathf.Abs(playerBody.GlobalPosition.X - enemyBody.GlobalPosition.X) < attackRange &&
		Mathf.Abs(playerBody.GlobalPosition.Y - enemyBody.GlobalPosition.Y) < attackRange)
		{
			enemyVelocity = Vector2.Zero;
			currentState = enemyState.prepare;
			isPreparing = true;

			if (isHurt)
			{
				currentState = enemyState.hurt;
			}

			if (isDying)
			{
				enemyVelocity = Vector2.Zero;
				currentState = enemyState.death;
			}
		}

		enemyBody.Velocity = enemyVelocity;
	}


	private void DetermineEnemyDirection()
	{
		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
		{
			// Horizontal movement dominates
			if (direction.X > 0)
			{
				currentDirection = enemyDirection.right;
			}
			else
			{
				currentDirection = enemyDirection.left;
			}
		}
		else
		{
			// Vertical movement dominates
			if (direction.Y > 0)
			{
				currentDirection = enemyDirection.down;
			}
			else
			{
				currentDirection = enemyDirection.up;
			}
		}
	}

	private void AnimateEnemy()
	{
		if (!isDying && !isAttacking)
		{
			enemyAnim.Play(currentState.ToString() + currentDirection.ToString());
		}
		if (isAttacking)
		{
			enemyAnim.Play(attackAnimState.ToString());
		}
		if(isDying)
		{
			enemyAnim.Play("death");
		}
		

	}

	private void OnHurtAnimFinished()
	{
		isHurt = false;
	}

	private void OnPrepareAnimFinished()
	{
		isPreparing = false;
		isAttacking = true;
	}

	private void OnEnemyDamaged(float damage)
	{
		isHurt = true;
	}

	private void OnEnemyKilled()
	{
		isDying = true;
		enemyShadow.Visible = false;
	}

	private void OnDeathAnimFinished()
	{
		EmitSignal(SignalName.DestroyEnemy);
	}

	private void OnEnemyAttackFinished()
	{
		isAttacking = false;
	}

}
