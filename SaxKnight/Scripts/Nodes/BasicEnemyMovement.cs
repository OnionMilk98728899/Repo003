using Godot;
using System;

public partial class BasicEnemyMovement : Node2D
{
	[Signal] public delegate void AttackReadyModeEventHandler(bool ready, Vector2 attackDirection);
	[Export] private CharacterBody2D enemyBody;

	[Export] private AnimationPlayer enemyAnim;
	[Export] private Sprite2D enemySprite;
	[Export] private float speed, maxSpeed;

	private CharacterBody2D targetBody;
	private Vector2 moveDirection, enemyVelocity;
	private enum direction { left, right }
	private direction currentDirection;
	private bool isAwareOfPlayer;
	private enum moveState
	{
		Walk, Attack, Hurt, Death
	}
	private moveState currentMoveState;


	public override void _Ready()
	{
		RandomizeInitialPath();
		currentMoveState = moveState.Walk;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentMoveState == moveState.Walk)
		{
			DetermineEnemyDirection();
			Patrol(delta);
			if (isAwareOfPlayer)
			{
				if (targetBody.GlobalPosition.X < enemyBody.GlobalPosition.X && currentDirection == direction.left ||
				targetBody.GlobalPosition.X > enemyBody.GlobalPosition.X && currentDirection == direction.right)
				{
					EmitSignal(SignalName.AttackReadyMode, true, moveDirection);
					GD.Print("Entered Attack Mode");
				}
			}
		}
		if (currentMoveState == moveState.Attack)
		{
			if (targetBody.GlobalPosition.X < enemyBody.GlobalPosition.X && currentDirection == direction.right||
			targetBody.GlobalPosition.X > enemyBody.GlobalPosition.X && currentDirection == direction.left)
			{
				currentMoveState = moveState.Walk;
				EmitSignal(SignalName.AttackReadyMode, false, moveDirection);
				GD.Print("Return To Walk");
			}
			DetermineEnemyDirection();
			Patrol(delta);
		}

		AnimateEnemy();
		enemyBody.MoveAndSlide();
	}

	private void RandomizeInitialPath()
	{
		int r = GD.RandRange(0, 100);
		if (r < 50)
		{
			currentDirection = direction.left;
		}
		else
		{
			currentDirection = direction.right;
		}
	}

	private void DetermineEnemyDirection()
	{
		if (currentDirection == direction.left)
		{
			moveDirection.X = -1;
		}
		else if (currentDirection == direction.right)
		{
			moveDirection.X = 1;
		}

		if (moveDirection.X > 0)
		{
			enemySprite.FlipH = false;
		}
		else
		{
			enemySprite.FlipH = true;
		}
	}

	private void Patrol(double delta)
	{
		enemyVelocity.X = moveDirection.X * speed;
		enemyBody.Velocity = enemyVelocity;
	}

	private void StopMoving()
	{
		enemyVelocity.X = 0;
		enemyBody.Velocity = enemyVelocity;
	}

	// private void BecomeAlertedToPlayer(CharacterBody2D targetBody)
	// {
	// 	currentMoveState = moveState.Attack;
	// }

	private void AnimateEnemy()
	{
		enemyAnim.Play(currentMoveState.ToString());
	}

	private void OnLeftPatrolBoundaryEntered(Node2D body)
	{
		currentDirection = direction.right;
		//enemyVelocity.X = 0;
	}

	private void OnRightPatrolBoundaryEntered(Node2D body)
	{
		currentDirection = direction.left;
		//enemyVelocity.X = 0;
	}

	private void OnPlayerDetectorBodyEntered(Node2D body)
	{
		targetBody = body.GetNode<CharacterBody2D>(".");
		isAwareOfPlayer = true;
		//BecomeAlertedToPlayer(targetBody);
	}

	private void OnPlayerDetectorExited(Node2D body)
	{
		currentMoveState = moveState.Walk;
	}

	// private void OnEnterAttackMode()
	// {
	// 	currentMoveState = moveState.Attack;
	// }


	private void OnAttackModeEntered()
	{
		currentMoveState = moveState.Attack;
		GD.Print("Attack Entered");
	}

}
