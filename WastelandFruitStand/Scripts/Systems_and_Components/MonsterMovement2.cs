using Godot;
using System;

public partial class MonsterMovement2 : Node2D
{
	[Signal] public delegate void DirectionChangedEventHandler(bool isLeft);
	[Signal] public delegate void MoveStateChangedEventHandler();
	[Export] private Node2D monster;
	[Export] private WanderAndSeekComponent wanderAndSeek;
	[Export] private TalkBubbleComponent talkBubble;
	[Export] private CharacterBody2D monsterBody;
	[Export] private int medianZIndex;
	[Export] private float repelCoefficient;
	//[Export] private Timer moveTimer;
	[Export] private Label debugLabel;
	[Export] private Sprite2D targetSprite, monsterSprite;
	private Vector2 monsterVelocity, movementTarget, standPosition, repelVector, testVelocity;
	private float moveAngle, maxRepelForce, repelFactor;
	private int region, thirstIndex;
	private FruitStand myStand;
	private MonsterMovement2 neighborMonster;
	private CharacterBody2D neighborBody;
	private bool isBeingReppeled, isFacingLeft;
	public int myCustomerIndex;
	private enum direction
	{
		left, right, none
	}

	private direction currentDirection, newDirection;
	private enum moveState
	{
		idle,
		waiting,
		wandering,
		seeking,
		drinking,
		mutating

	}
	private moveState currentMoveState, nextMoveState;
	public override void _Ready()
	{
		nextMoveState = moveState.wandering;
		currentMoveState = moveState.idle;
		currentDirection = direction.none;
		wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.wandering;
		EmitSignal(SignalName.MoveStateChanged, (int)nextMoveState);

		GlobalSignals.Instance.SellCupOfJuice += BuyCupOfJuice;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentMoveState != nextMoveState)
		{

			EmitSignal(SignalName.MoveStateChanged, (int)nextMoveState);

		}
		currentMoveState = nextMoveState;
		ManuallySortZIndex();
		CalculateMovement();
		DetermineSpriteDirection();

		monsterBody.Velocity = monsterVelocity;
		monsterBody.MoveAndSlide();

		if (currentMoveState != moveState.drinking)
		{
			debugLabel.Text = myCustomerIndex.ToString();
		}
		else
		{
			debugLabel.Text = "DRINKING!";
		}
		
	}

	private void CalculateMovement()
	{
		targetSprite.GlobalPosition = wanderAndSeek.seekTarget;
		debugLabel.Text = MathF.Round(monsterVelocity.X).ToString();

		if (currentMoveState == moveState.idle || currentMoveState == moveState.wandering ||
		currentMoveState == moveState.seeking || currentMoveState == moveState.drinking)
		{
			monsterVelocity = wanderAndSeek.CalculateMovement();

			if (repelVector.LengthSquared() > 0.01f && isBeingReppeled)
			{
				monsterVelocity += repelVector.Normalized() * repelFactor;
			}

		}
		if (currentMoveState == moveState.seeking)
		{

			if (Mathf.Abs(wanderAndSeek.seekTarget.X - monsterBody.GlobalPosition.X) < 4 && MathF.Abs(wanderAndSeek.seekTarget.Y - monsterBody.GlobalPosition.Y) < 4)
			{
				nextMoveState = moveState.waiting;
				wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.idle;
			}
		}
		if (currentMoveState == moveState.waiting)
		{
			monsterVelocity = Vector2.Zero;
		}

	}

	private void DetermineSpriteDirection()
	{
		// if (!isBeingReppeled)
		// {
			if (monsterVelocity.X > 0)
			{
				newDirection = direction.right;
				isFacingLeft = false;
			}
			if (monsterVelocity.X < 0)
			{
				newDirection = direction.left;
				isFacingLeft = true;
			}

			if (currentDirection != newDirection)
			{
				EmitSignal(SignalName.DirectionChanged, isFacingLeft);
			}

			currentDirection = newDirection;
		// }
	}

	private void ManuallySortZIndex()
	{
		int newZ = (int)monsterBody.GlobalPosition.Y;
		if (newZ != monster.ZIndex)
		{
			monster.ZIndex = newZ;
		}
	}

	private void CalculateChanceToMoveToFruitStand(int visibility)
	{
		int probability = GD.RandRange(0, visibility);
		int probChecker = GD.RandRange(0, 100);

		if (probChecker < probability && myStand.customersInLine <= myStand.customerLimit)
		{

			nextMoveState = moveState.seeking;
			wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.seeking;
			wanderAndSeek.isTargeting = true;
			monsterSprite.Modulate = new Color(1, 0, 0, 1);
			myCustomerIndex = myStand.customersInLine;
			standPosition = myStand.GetPlaceInLine();
			wanderAndSeek.SetTarget(standPosition);
			myStand.IncreaseCustomerIndex();
			talkBubble.OnStandNoticed();
		}

	}


	public void UpdateLinePosition()
	{
		myCustomerIndex--;
		standPosition = myStand.GetNextSpotInLine(myCustomerIndex);
		nextMoveState = moveState.seeking;
		wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.seeking;
	}

	public bool GetSeekingStatus()
	{
		if (currentMoveState == moveState.waiting || currentMoveState == moveState.seeking)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void BuyCupOfJuice()
	{
		
		if (currentMoveState == moveState.waiting || currentMoveState == moveState.seeking || currentMoveState ==  moveState.idle)
		{
			if (myCustomerIndex == 0)
			{
				GD.Print("Bought the juice successfully!");
				nextMoveState = moveState.drinking;
				wanderAndSeek.currentMode = WanderAndSeekComponent.moveMode.wandering;
			}
			else
			{
				UpdateLinePosition();
			}
			wanderAndSeek.isTargeting = false;

		}
	}

	private void OnDetectorBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D character)
		{

			if (character.GetNode<CharacterBody2D>(".").IsInGroup("Stand"))
			{

				myStand = character.GetNode<FruitStand>("..");

				CalculateChanceToMoveToFruitStand(myStand.GetStandVisibility());

			}
		}
	}

	// private void OnRepellerBodyEntered(Node2D body)
	// {
	// 	if (body is CharacterBody2D character)
	// 	{
	// 		if (character.GetNode<CharacterBody2D>(".").IsInGroup("Monster"))
	// 		{
	// 			neighborMonster = character.GetNode<MonsterMovement2>("../MonsterMovement");
	// 			neighborBody = character.GetNode<CharacterBody2D>(".");

	// 			if (currentMoveState == moveState.wandering && neighborMonster.currentMoveState == moveState.wandering)
	// 			{
	// 				repelVector = monsterBody.GlobalPosition - neighborBody.GlobalPosition;
	// 				repelFactor = repelCoefficient * (1000 / (repelVector.LengthSquared() + 1));
	// 				isBeingReppeled = true;
	// 			}
	// 		}
	// 	}
	// }

	// private void OnRepellerBodyExited(Node2D body)
	// {
	// 	if (body is CharacterBody2D character)
	// 	{
	// 		if (character.GetNode<CharacterBody2D>(".").IsInGroup("Monster"))
	// 		{
	// 			neighborBody = character.GetNode<CharacterBody2D>(".");
	// 			isBeingReppeled = false;
	// 			repelVector = Vector2.Zero;
	// 			repelFactor = 0;

	// 		}
	// 	}

	// }

	private void OnChangeToIdleState()
	{
		nextMoveState = moveState.idle;
	}

	private void OnChangeToWanderState()
	{
		nextMoveState = moveState.wandering;
	}

	private void OnGetTargetPositionFromParent()
	{
		wanderAndSeek.seekTarget = standPosition;
	}
}
