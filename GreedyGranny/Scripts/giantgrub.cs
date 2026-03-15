using Godot;
using System;

public partial class giantgrub : CharacterBody2D
{
	[Export] private int walkSpeed, scurrySpeed, MAX_WALK_SPEED, MAX_SCURRY_SPEED;
	[Export] private PackedScene myProjectile;
	private PlayerProjectile mySlime, myEgg;
	private ItemManager itemMan;
	private int determineAttackType;
	private Sprite2D grubSprite;
	private AnimationPlayer grubAnim;
	private Area2D grubArea;
	private Timer idleTimer;
	private Vector2 myVelocity;
	private RayCast2D  leftWallCheck, rightWallCheck;
	private CollisionShape2D leftPlayerDetect, rightPlayerDetect;
	private Label stateLabel;
	private BossDamageable bossDmg;
	private bool isHurt, isScurrying, isMoving, isShooting, hasThrownProjectile, isDying, isDead, canDamage;
	private Timer scurryTimer, shootTimer, shootDelayTimer, dyingTimer;
	private Color none = new Color("#ffffff"), yellow = new Color("#ffff00"), red = new Color("#ff0000"), purple =  new Color("#ff00ff");

	private enum Direction{
		Left,
		Right
	}

	private enum State{
		Move, 
		Shoot, 
		Scurry,
		Hurt,
		Death, 
		Dead
	}

	private State currentState;
	private Direction currentDirection;
	private int directionMod;

	public override void _Ready()
	{
		itemMan = GetNode<ItemManager>("../ItemManager");
		grubSprite = GetNode<Sprite2D>("GrubSprite");
		grubAnim = GetNode<AnimationPlayer>("BigGrubAnim");
		leftWallCheck = GetNode<RayCast2D>("LeftWallDetector");
		rightWallCheck = GetNode<RayCast2D>("RightWallDetector");
		stateLabel = GetNode<Label>("StateLabel");
		bossDmg = GetNode<BossDamageable>("BossDamageable");
		scurryTimer = GetNode<Timer>("ScurryTimer");
		shootTimer = GetNode<Timer>("ShootTimer");
		shootDelayTimer = GetNode<Timer>("ShootDelayTimer");
		dyingTimer = GetNode<Timer>("DyingTimer");
		leftPlayerDetect = GetNode<CollisionShape2D>("PlayerDetectArea/PlayerDetectLeft");
		rightPlayerDetect = GetNode<CollisionShape2D>("PlayerDetectArea/PlayerDetectRight");

		currentDirection = Direction.Left;
		currentState = State.Move;
	}

	public override void _PhysicsProcess(double delta)
	{   
		DetermineDirection();
		DetermineState();
		DetermineBehavior(delta);
		Velocity = myVelocity;
		MoveAndSlide();

		//stateLabel.Text = D.ToString();
	}

	private void DetermineState(){

		if(isScurrying){currentState = State.Scurry;}
		if(isShooting){currentState = State.Shoot;}
		if(isHurt){currentState = State.Hurt;}
		if(!isScurrying && !isHurt && !isShooting){currentState = State.Move;}
		if(isDying){currentState = State.Death;}
		if(isDead){currentState = State.Dead;}
	}

	private void DetermineBehavior(double delta){

		switch(currentState){
			case State.Move:
			hasThrownProjectile = false;
			grubAnim.Play("Move");
			if(MathF.Abs(myVelocity.X) < MAX_WALK_SPEED){
				myVelocity.X += walkSpeed*directionMod * (float)delta;
			}
			if(MathF.Abs(myVelocity.X) >= MAX_WALK_SPEED){
				myVelocity.X = directionMod*MAX_WALK_SPEED;
			}
			WallCheck();

			break;
			case State.Hurt:
			myVelocity.X =0;
			grubAnim.Play("Hurt");
			break;
			case State.Shoot:
			myVelocity.X = 0;
			grubAnim.Play("Shoot");
			if(!hasThrownProjectile){
				//ThrowProjectile();
				shootDelayTimer.Start();
				hasThrownProjectile = true;
			}
			break;
			case State.Scurry:
			grubAnim.Play("Scurry");
			if (MathF.Abs(myVelocity.X) < MAX_SCURRY_SPEED){
				myVelocity.X += scurrySpeed * directionMod * (float)delta;
			}
			WallCheck();
			break;
			case State.Death:
			grubAnim.Play("Death");
			break;
			case State.Dead:
			grubAnim.Play("Dead");
			break;
		}
	}

	private void DetermineDirection()
	{
		if (currentDirection == Direction.Left){
			directionMod = -1;
			grubSprite.FlipH = false;
			rightPlayerDetect.Disabled = true;
			leftPlayerDetect.Disabled = false;
		}
		else{
			directionMod = 1;
			grubSprite.FlipH = true;
			rightPlayerDetect.Disabled = false;
			leftPlayerDetect.Disabled = true;
		}
	}

	private void ThrowProjectile()
	{
		myEgg = myProjectile.Instantiate<PlayerProjectile>();
		myEgg.startPosition = new Vector2(GlobalPosition.X -(25*(directionMod)), GlobalPosition.Y - 45);
		myEgg.initialVelocity = new Vector2(directionMod * 20, 200);
		myEgg.gravity = 4;
		myEgg.speed = 100;

		if (currentDirection == Direction.Left){
			myEgg.directionMod = -1;
		}
		else{
			myEgg.directionMod = 1;
		}

		itemMan.AddChild(myEgg);
		myEgg.DetermineBallType(3);

	}

	private void WallCheck()
	{
		if (leftWallCheck.IsColliding() && myVelocity.X < 0)
		{
			myVelocity.X = 30;
			currentDirection = Direction.Right;
			leftWallCheck.Enabled = false;
			rightWallCheck.Enabled = true;
		}

		if (rightWallCheck.IsColliding() && myVelocity.X > 0)
		{
			myVelocity.X = -30;
			currentDirection = Direction.Left;
			leftWallCheck.Enabled = true;
			rightWallCheck.Enabled = false;
		}
	}

	private void DetermineNextAttackType(){
		determineAttackType = (int)(GD.Randi() % 2);

	}

	private void DestroyAllGrubSpawn()
	{

		Node2D sceneNode = GetNode<Node2D>("..");
		foreach (Node child in sceneNode.GetChildren())
		{
			if (child is basic_enemy grub)
			{
				GD.Print("Grub Found");
				if (grub.spawnedByBoss)
				{
					grub.ExplodeEnemy();
				}
			}

		}
	}

	private void OnCanDamagePlayerAgain()
	{
		grubSprite.Modulate = none;
	}

	private void OnBossHurt(bool hurt)
	{
		isHurt = hurt;	
		grubSprite.Modulate = red;
	}

	private void OnPlayerDetectBodyEntered(Node2D body)
	{

		if(body.IsInGroup("Player")){

			DetermineNextAttackType();

			if (determineAttackType == 0)
			{
				isScurrying = true;
				scurryTimer.Start();
			}
			if (determineAttackType == 1)
			{
				isShooting = true;
				shootTimer.Start();
			}
		}
	}


	private void OnScurryTimerTimeout()
	{
		isScurrying = false;
	}

	private void OnShootDelayTimerTimeout()
	{
		ThrowProjectile();
	}

	private void OnShootTimerTimeout()
	{
		isShooting = false;
	}

	private void OnBossDying(bool dying)
	{
		dyingTimer.Start();
		isScurrying = false;
		isMoving = false;
		isShooting = false;
		isDying = true;
	}


	private void OnDyingTimerTimeout()
	{
		isDying= false;
		isDead = true;
		DestroyAllGrubSpawn();
	}

}


















