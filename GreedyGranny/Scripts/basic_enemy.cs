using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public partial class basic_enemy : CharacterBody2D
{
	[Export] private string movingAnim, attackAnim, upsideDownAnim, frozenAnim, burningAnim, burntAnim, explodingAnim, deadAnim;
	
	public enum State{
		Moving,
		Attack,
		UpsideDown,
		Carried,
		FrozenCarried,
		Bounced,
		Thrown,
		Frozen, 
		Burning, 
		Burnt,
		Exploding,
		Dead
	}

	[Export] private int hitPoints;
	[Export] private PackedScene coinToGenerate;
	[Export] public int coinQuantity;
	//[Export] private PlayerCarryable.CarryableType carryableType;
	private Damageable damageable;
	private ItemManager itemMan;
	private State currentState, nextState;
	private EnemyPatrol enemyPatrol;
	private carryable_item carryableItem;
	private Label stateLabel;
	private AnimationPlayer myAnim;
	private Timer attackTimer, hitColorTimer, deathTimer, animTimer;
	private RayCast2D takeDmgSensor;
	private Node2D damageSensor;
	private int dmgSensorsTriggered = 0;
	private List<RayCast2D> dmgSensorList;
	private Sprite2D mySprite;
	//private Area2D myPlayerSensor;
	private PlayerMovement pMove;
	private PlayerCarryable playerCarryable;
	private PlayerProjectile myBall;
	private PlayerManager playerMan;
	private CollisionShape2D myColl, myPlayerSensor;
	private bool isMoving, isAttacking, attackTriggered, colorChangeTriggered, isUpsideDown, isBounced, isCarryable, isCarried, isDead, alreadyTakingDmg,
	canDamagePlayer, isFrozen, isBurning, isBurnt, isShattering, isFrozenCarried,  coinReleased;
	public bool spawnedByBoss, isExploding;


	public override void _Ready(){

		enemyPatrol = GetNode<EnemyPatrol>("EnemyPatrol");
		carryableItem = GetNode<carryable_item>("CarryableItem");
		carryableItem.containedItemQuan = coinQuantity;
		stateLabel = GetNode<Label>("StateLabel");
		myAnim = GetNode<AnimationPlayer>("EnemyAnimator");
		damageable = GetNode<Damageable>("Damageable");
		mySprite = GetNode<Sprite2D>("EnemySprite");
		hitColorTimer = GetNode<Timer>("HitColorTimer");
		attackTimer = GetNode<Timer>("AttackTimer");
		animTimer = GetNode<Timer>("AnimationTimer");
		deathTimer = GetNode<Timer>("DeathTimer");
		myColl = GetNode<CollisionShape2D>("BodyCollider");
		myPlayerSensor = GetNode<CollisionShape2D>("Damageable/PlayerSensor/PlayerSensorCollider");
		playerMan = GetNode<PlayerManager>("/root/Scene/PlayerManager");
		itemMan = GetNode<ItemManager>("../ItemManager");
		

	}

	public override void _PhysicsProcess(double delta){

		
		DetermineState();
		DetermineBehavior();
		if(!IsOnFloor() && currentState != State.Carried && currentState != State.Bounced && enemyPatrol.myVelocity.Y < enemyPatrol.MAX_GRAVITY){
			enemyPatrol.ApplyGravity(delta);
		}
		if(IsOnFloor()){
			enemyPatrol.isOnFloor = true;
		}else{
			enemyPatrol.isOnFloor = false;
		}

		// if(enemyPatrol.hasBegunBouncing && IsOnFloor() && enemyPatrol.myVelocity.Y > 0){
		// 	enemyPatrol.finishedBouncing = true;
		// 	enemyPatrol.myVelocity.Y = 0;
		// }
		
		if(myColl.Disabled){
			enemyPatrol.myVelocity.Y = 0;
		}

		Velocity = enemyPatrol.myVelocity;
		MoveAndSlide();

		currentState = nextState;
		//stateLabel.Text = currentState.ToString() + "\n" +isCarried;

	}

	private void DetermineState()
	{
		if (isUpsideDown && !enemyPatrol.isBounced && !isCarried)
		{
			nextState = State.UpsideDown;

		}
		else
		{

			if (IsOnFloor() && !isAttacking && !enemyPatrol.isBounced)
			{
				isMoving = true;
				nextState = State.Moving;
			}
			else if (IsOnFloor() && isAttacking && !enemyPatrol.isBounced)
			{
				nextState = State.Attack;
				if (!attackTriggered)
				{
					attackTimer.Start();
					attackTriggered = true;
				}

			}
		}

		if(damageable.isUpsideDown){isUpsideDown = true;}
		else{isUpsideDown = false;}

		
		if(playerMan.carryTriggered && isCarryable && !playerMan.currentlyCarrying){
			if(currentState == State.UpsideDown ){
				nextState = State.Carried;
			}else if(currentState == State.Frozen){
				nextState = State.FrozenCarried;
			}
		}
		

		if(isCarried){
			
			nextState = State.Carried;
			isUpsideDown = false;
		}

		if(isFrozenCarried){
			nextState = State.FrozenCarried;
			isUpsideDown = false;
		}

		if(enemyPatrol.isBounced){
			nextState = State.Bounced;
			
		}else{
			carryableItem.bounced = false;
		}
		if(isBurning){nextState = State.Burning;}
		if(isBurnt){nextState = State.Burnt;}
		if(isFrozen && !isFrozenCarried){nextState = State.Frozen;
		carryableItem.frozen = true;}
		if(isExploding){nextState = State.Exploding;}

	}

	private void DetermineBehavior(){
		switch(currentState){

			case State.Moving:
			myAnim.Play(movingAnim);
			break;
			case State.Attack:
			myAnim.Play(attackAnim);
			break;
			case State.UpsideDown:

			enemyPatrol.isMotionless = true;
			myAnim.Play(upsideDownAnim);
			carryableItem.carryableEnemy = true;
			
			break;

			case State.Carried:
			
			GetCarriedByPlayer();
			carryableItem.carryableEnemy = true;
			break;

			case State.FrozenCarried:
			GetCarriedByPlayer();
			carryableItem.carryableEnemy = true;
			break;

			case State.Bounced:

			if(IsOnFloor()){
				isUpsideDown = true;
			}

			break;

			case State.Thrown:
			mySprite.Visible = false;
			myColl.Disabled = true;
			carryableItem.Visible = true;
			if(isFrozen){carryableItem.frozen = true;}
			break;

			case State.Frozen:
			myAnim.Play(frozenAnim);
			enemyPatrol.isMotionless = true;
			carryableItem.carryableEnemy = true;
			//isFrozenCarried = false;
			
			break;

			case State.Burning:
			myAnim.Play(burningAnim);
			enemyPatrol.myVelocity.X = 0;
			break;

			case State.Burnt:
			myAnim.Play(burntAnim);
			enemyPatrol.myVelocity.X = 0;
			break;

			case State.Exploding:
			myAnim.Play(explodingAnim);
			enemyPatrol.myVelocity.X = 0;
			break;
		}
	}

	private void GetCarriedByPlayer(){

		mySprite.Visible = false;
		myColl.Disabled = true;
		myPlayerSensor.Disabled = true;
		carryableItem.Visible = true;
			
	}

	private void DeactivateAndDropEnemy(){

		//carryableItem.carryable = false;
		enemyPatrol.isDead = true;
		SetCollisionMaskValue(1, false);
		deathTimer.Start();

	}

	private void BurnEnemy(){
		isBurning = true;
		animTimer.Start();
	}
	private void ShatterFrozenEnemy(){

	}

	public void ExplodeEnemy(){
		ZIndex = 8;
		isExploding = true;
		if(spawnedByBoss){
			int randNum1 = (int)(GD.Randi() % 15);
			float randNum2 = randNum1/10;
			animTimer.WaitTime = randNum2;
		}else{
			animTimer.WaitTime = .8;
		}
		
		animTimer.Start();

	}
	private void ReleaseCoin(){

		if(!coinReleased){
			
			coinReleased = true;
			Vector2 coinPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y -15);
			itemMan.ReleaseContentsOfContainer(coinQuantity, collectible_item.ItemType.Coin, coinPosition);
			
		}
	}

	private void OnFreezeEnemy()
	{
		isFrozen = true;
		
	}

	private void OnCarryableItemItemDestroyed(){
		QueueFree();
	}

	private void CallMethodByName(string methodName)
	{
		if (methodName != "null")
		{
			Call(methodName);
		}
	}

	private void OnCarryableStatusChanged(bool carryableStatus)
	{
		isCarryable = carryableStatus;
	}
	private void OnCarriedStatusChanged(bool carriedStatus)
	{
		if(!isFrozen){
			isCarried = carriedStatus;
		}else{
			isFrozenCarried = carriedStatus;
		}
		
	}

	private void OnEnemyCarried()
	{
		GetCarriedByPlayer();
		
	}

	private void OnKillEnemy(string method1, string method2)
	{

		CallMethodByName(method1);
		CallMethodByName(method2);
	
	}
	private void OnHitColorTimerTimeout(){
		mySprite.Modulate = new Color (1,1,1,1);
	}

	private void OnAttackTimerTimeout(){
		isAttacking = false;
		attackTriggered = false;
	}

	private void OnDeathTimerTimeout(){
		QueueFree();
	}

	
	private void OnAnimTimerTimeout()
	{
		if(isBurnt){
			QueueFree();
		}
		if(isBurning){
			isBurning = false;
			animTimer.Start();
			isBurnt = true;
		}
		if(isExploding){
			QueueFree();
		}


	}
	

}





















