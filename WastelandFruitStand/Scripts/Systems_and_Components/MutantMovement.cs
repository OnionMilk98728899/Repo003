using Godot;
using System;

public partial class MutantMovement : Node2D
{
	[Signal] public delegate void MoveStateChangedEventHandler();
	[Signal] public delegate void DirectionChangedEventHandler(bool isLeft);
	[Export] private Node2D mutant;
	[Export] private WanderAndSeekComponent wanderAndSeek;
	[Export] private HurtboxSystem hurtBox;
	[Export] private CharacterBody2D mutantBody;
	[Export] private Timer preattackTimer, attackTimer, hurtTimer, deathTimer, destroyTimer;
	[Export] private Label debugLabel;
	[Export] private int hitPoints;

	public enum direction
	{
		left, right, none
	}
	public enum moveState
	{
		spawning,
		lookAround,
		wandering,
		seeking,
		preattacking,
		attacking,
		hurt,
		dying,
		dead,
		destroying
	}
	private CharacterBody2D playerBody;
	private Vector2 mutantVelocity, playerPosition;
	public moveState currentMoveState, nextMoveState;
	private direction currentDirection, newDirection;
	private bool isFacingLeft, isHurt, isDying;

	public override void _Ready()
	{
		nextMoveState = moveState.wandering;
		currentMoveState = moveState.lookAround;
		wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.wandering;

		hurtBox.OnHurt += HandleHurt;
	}

	public override void _PhysicsProcess(double delta)
	{
		ManuallySortZIndex();

		if (currentMoveState != nextMoveState)
		{
			EmitSignal(SignalName.MoveStateChanged, (int)nextMoveState);
		}
		currentMoveState = nextMoveState;
		CalculateMovement();
		ManuallySortZIndex();
		DetermineSpriteDirection();
		mutantBody.Velocity = mutantVelocity;
		mutantBody.MoveAndSlide();

		//debugLabel.Text = currentMoveState.ToString() + "/ " + wanderAndSeek.currentMode + "\n" + mutantVelocity;
	}

	private void DetermineSpriteDirection()
	{

		if (mutantVelocity.X > 0)
		{
			newDirection = direction.right;
			isFacingLeft = false;
		}
		if (mutantVelocity.X < 0)
		{
			newDirection = direction.left;
			isFacingLeft = true;
		}

		if (currentDirection != newDirection)
		{
			EmitSignal(SignalName.DirectionChanged, isFacingLeft);

		}

		currentDirection = newDirection;

	}


	private void CalculateMovement()
	{
		if (!isHurt)
		{
			if (currentMoveState == moveState.lookAround || currentMoveState == moveState.wandering || currentMoveState == moveState.seeking ||
				currentMoveState == moveState.preattacking || currentMoveState == moveState.attacking)
			{
				mutantVelocity = wanderAndSeek.CalculateMovement();
			}

			if (currentMoveState == moveState.seeking)
			{
				if (Mathf.Abs(wanderAndSeek.GetTarget().X - mutantBody.GlobalPosition.X) < 4 && MathF.Abs(wanderAndSeek.GetTarget().Y - mutantBody.GlobalPosition.Y) < 4)
				{
					nextMoveState = moveState.preattacking;
					wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.idle;
					if (preattackTimer.IsStopped())
					{
						preattackTimer.Start();
					}
				}
			}
		}
		if (isDying)
		{
			mutantVelocity = wanderAndSeek.CalculateMovement();
		}

	}

	private void ManuallySortZIndex()
	{
		int newZ = (int)mutantBody.GlobalPosition.Y;
		if (newZ != mutant.ZIndex)
		{
			mutant.ZIndex = newZ;
		}
	}

	private void OnChangeToIdle()
	{
		nextMoveState = moveState.lookAround;
	}

	private void ChangeToWanderState()
	{
		nextMoveState = moveState.wandering;
	}

	private void OnGetTargetPositionFromParent()
	{
		wanderAndSeek.seekTarget = playerBody.GlobalPosition;
	}

	private void OnPlayerDetectorBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D character)
		{
			if (character.GetNode<CharacterBody2D>(".").IsInGroup("Player"))
			{
				if (!isHurt)
				{
					nextMoveState = moveState.seeking;
					wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.seeking;
					wanderAndSeek.isTargeting = true;
					playerBody = character.GetNode<CharacterBody2D>(".");
					playerPosition = playerBody.GlobalPosition;
					wanderAndSeek.SetTarget(playerPosition);

				}

			}
		}
	}

	private void OnPlayerAttackerBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D character)
		{
			if (character.GetNode<CharacterBody2D>(".").IsInGroup("Player"))
			{
				if (!isHurt)
				{
					nextMoveState = moveState.preattacking;
					wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.idle;
					if (preattackTimer.IsStopped())
					{
						preattackTimer.Start();
					}
				}

			}
		}

	}

	private void HandleHurt(CharacterBody2D body, int multiplier)
	{
		if (!isHurt)
		{
			isHurt = true;
			wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.idle;
			// if (hitPoints > 0)
			// {
			GD.Print("Hurting!");

			nextMoveState = moveState.hurt;
			hitPoints--;
			hurtTimer.Start();
			// }
			// else
			// {
			// 	GD.Print("Dying!");
			// 	nextMoveState = moveState.dying;
			// 	wanderAndSeek.isHurt = true;
			// 	hurtTimer.WaitTime = .8f;
			// 	hurtTimer.Start();
			// }

		}

	}
	private void OnPreAttackTimerTimeout()
	{
		if (!isHurt)
		{
			nextMoveState = moveState.attacking;
			wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.idle;
			attackTimer.Start();
		}
	}

	private void OnAttackTimerTimeout()
	{
		if (!isHurt)
		{
			nextMoveState = moveState.lookAround;
			wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.idle;
		}

	}

	private void OnHurtTimerTimeout()
	{
		GD.Print("Hurt Timer timed out and hit points  = " + hitPoints);
		if (hitPoints > 0)
		{
			isHurt = false;
			nextMoveState = moveState.wandering;
			wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.wandering;
		}
		if (isDying)
		{
			GD.Print("DIED!");
			nextMoveState = moveState.dead;
			deathTimer.Start();

		}
		if (hitPoints <= 0 && !isDying)
		{
			GD.Print("Death State ACTIVATED");
			nextMoveState = moveState.dying;
			wanderAndSeek.isHurt = true;
			hurtTimer.WaitTime = .8f;
			hurtTimer.Start();
			isDying = true;
			wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.idle;
		}


	}

	private void OnDeathTimerTimeout()
	{
		GD.Print("Death timer timed out");
		nextMoveState = moveState.destroying;
		destroyTimer.Start();
	}


	private void OnDestroyTimerTimeout()
	{
		GD.Print("FREED!");
		mutant.QueueFree();
	}

}
