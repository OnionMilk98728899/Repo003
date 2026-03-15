using Godot;
using System;
using System.Runtime.Serialization.Formatters;

public partial class PlayerMovement : Node2D
{
	public Vector2 myVelocity, direction;

	private Vector2 enemyPosition, chargedItemPosition;
	[Export] public int gravity, speed, acceleration, jumpPower, runSpeed, chargeSpeed, crouchSpeed;
	[Signal] public delegate void PlayerHurtEventHandler(Vector2 enemyPos);
	[Export] private PackedScene myProjectile;
	private ItemManager itemMan;
	private carryable_item destroyableItem;
	private int mySpeed, MAX_JUMP_POWER = 140;

	public bool pressedRight, pressedLeft, pressedDown, pressedUp, pressedJump, pressedAction1, pressedAction3, pressedAction4, heldJump, releasedAction3,
	underWater, charging, chargeCooldown, chargeIsHeld, onLadder, isClimbing, onFloor, isHurt, isChargingThrough, isStomping, stompingBox, throwingBall, 
	hasStomped, isStompBouncing, stuckToCeiling, canDestroyItem, isCrouched, doubleEffectPlay;
	private granny myGranny;
	public double stompCounter, stompBounceCounter;
	public Timer chargeTimer, chargeCooldownTimer, chargeThroughTimer, hurtTimer, throwTimer;
	private PlayerProjectile myBall;
	public override void _Ready(){

		myGranny = GetNode<granny>("..");
		chargeTimer = GetNode<Timer>("ChargeTimer");
		chargeThroughTimer = GetNode<Timer>("ChargeThroughTimer");
		hurtTimer = GetNode<Timer>("HurtTimer");
		chargeCooldownTimer = GetNode<Timer>("ChargeCooldownTimer");
		itemMan = GetNode<ItemManager>("/root/Scene/ItemManager");
		throwTimer = GetNode<Timer>("ThrowTimer");

	}

	public override void _PhysicsProcess(double delta)
	{   
		pressedRight = Input.IsActionPressed("ui_right");
		pressedLeft = Input.IsActionPressed("ui_left");
		pressedDown = Input.IsActionPressed("ui_down");
		pressedUp = Input.IsActionPressed("ui_up");


		pressedJump = Input.IsActionJustPressed("Action2");
		heldJump = Input.IsActionPressed("Action2");
		pressedAction1 = Input.IsActionPressed("Action1");
		pressedAction3 = Input.IsActionPressed("Action3");
		pressedAction4 = Input.IsActionPressed("Action4");
	  	

		ApplyGravity(delta);
		ApplyMovement(delta);
		Charge();
		Stomp(delta);
		ThrowProjectile();

	}

	private void ApplyGravity(double delta)
	{   
		if(isClimbing){return;}
		if (!underWater)
		{
			if(myGranny.IsOnCeiling() && heldJump && !myGranny.carrying && myGranny.currentHeadgear == Headgear.HeadgearType.Viking){
				stuckToCeiling = true;
			}else{stuckToCeiling = false;}

			if (!myGranny.IsOnFloor())
			{
				if(!myGranny.IsOnFloor() && heldJump && myGranny.currentHeadgear == Headgear.HeadgearType.Sunhat && myVelocity.Y > 0){
					myVelocity.Y += gravity/3.5f * (float)delta;
					myGranny.isFloating = true;
					if(myVelocity.Y > 50){myVelocity.Y = 50;}
				}else if(isStomping){

					myVelocity.Y = 300;
					myVelocity.X = 0;
				
				}else{

					myVelocity.Y += gravity * (float)delta;
					myGranny.isFloating = false;
					
				}
				if(myVelocity.Y > 250 && !isStomping){myVelocity.Y = 250;}

				
			}else{
				
				myGranny.isFloating = false;
			}
			//if(isStompBouncing){myVelocity.Y = -300* (float)stompBounceTimer.TimeLeft;}
		}
		else if (underWater)
		{
			if (!myGranny.IsOnFloor())
			{
				myVelocity.Y += .3f*gravity * (float)delta;
			}else{
				myVelocity.Y = 0;
			}
		}
		if(charging && myVelocity.Y < 0){
				myVelocity.Y = 0;
		}
		if(stuckToCeiling){
			myVelocity.Y = 0;
			myVelocity.X = 0;
		}	
		
	}

	private void ApplyMovement(double delta){

		if (!underWater)
		{
			if(pressedAction1 && !isCrouched){
				mySpeed = runSpeed;
			}else if(isCrouched){
				mySpeed = crouchSpeed;
			}else{
				mySpeed = speed;
			}
			if (pressedRight && !pressedLeft)
			{
				myVelocity.X = Mathf.MoveToward(myVelocity.X, mySpeed, acceleration * (float)delta);
				myGranny.currentDirection = granny.Direction.Right;
			}
			else if (pressedLeft && !pressedRight)
			{
				myVelocity.X = Mathf.MoveToward(myVelocity.X, -mySpeed, acceleration * (float)delta);
				myGranny.currentDirection = granny.Direction.Left;

			}

			else if(isHurt){

				myVelocity.X = Mathf.MoveToward(3* direction.X, mySpeed, acceleration * -(float)delta);
			}
			else
			{
				myVelocity.X = Mathf.MoveToward(MathF.Round(myVelocity.X / 3), 0, 0);
			}

			if (pressedJump && myGranny.IsOnFloor() && !myGranny.isInteractable && !isCrouched)
			{
				myVelocity.Y -= jumpPower;
			}
			if (myGranny.IsOnFloor() && !pressedJump && !isStompBouncing)
			{
				myVelocity.Y = 0;
			}

			if(stuckToCeiling){
				myVelocity.X = 0;
				myVelocity.Y = 0;
			}

			if(charging && myVelocity.Y < 0){
				myVelocity.Y = 0;
			}

		}else if (underWater){

			if (pressedRight && !pressedLeft)
			{
				myVelocity.X = Mathf.MoveToward(myVelocity.X, mySpeed*.5f, acceleration * (float)delta);
				myGranny.currentDirection = granny.Direction.Right;
			}
			else if (pressedLeft && !pressedRight)
			{
				myVelocity.X = Mathf.MoveToward(myVelocity.X, -mySpeed*.5f, acceleration * (float)delta);
				myGranny.currentDirection = granny.Direction.Left;
			}
			else
			{
				myVelocity.X = Mathf.MoveToward((myVelocity.X / 3), 0, 0);
			}

			if (pressedJump)
			{
				myVelocity.Y -= .4f*jumpPower;
			}

			if(myVelocity.Y < -.6f*jumpPower){
				myVelocity.Y = -.6f*jumpPower;
			}
			

		}
		if(myVelocity.Y < -MAX_JUMP_POWER){
			myVelocity.Y = -MAX_JUMP_POWER;
		}
		if(charging && myVelocity.Y < 0){myVelocity.Y = 0;}

		if (onLadder && !myGranny.carrying)
		{
			if (onFloor)
			{
				if (pressedUp)
				{
					myVelocity.Y  = Mathf.MoveToward(myVelocity.Y, -mySpeed, acceleration * (float)delta);
					isClimbing = true;
				}

			}
			else
			{

				if (pressedUp)
				{
					myVelocity.Y = Mathf.MoveToward(myVelocity.Y, -mySpeed, acceleration * (float)delta);
					isClimbing = true;
				}
				if (pressedDown)
				{
					myVelocity.Y = Mathf.MoveToward(myVelocity.Y, mySpeed, acceleration * (float)delta);
					isClimbing = true;
				}
				if (pressedRight && !pressedLeft)
				{
					myVelocity.X = Mathf.MoveToward(myVelocity.X, mySpeed , acceleration * (float)delta);
					myVelocity.Y = 0;
					isClimbing = true;
				}
				else if (pressedLeft && !pressedRight)
				{
					myVelocity.X = Mathf.MoveToward(myVelocity.X, -mySpeed , acceleration * (float)delta);
					myVelocity.Y = 0;
					isClimbing = true;
				}
				if (pressedJump)
				{
				myVelocity.Y -= jumpPower;
				isClimbing = false;
				onLadder = false;
				}
			}

			if(isClimbing){
				if(!pressedUp && !pressedDown && !pressedLeft && !pressedRight && !pressedAction1 && !pressedAction3){
					myVelocity.X = 0;
					myVelocity.Y = 0;
				}
			}


		}else{
			isClimbing = false;
		}

		if(isChargingThrough){myVelocity.X -= 10*(myGranny.GlobalPosition.X - chargedItemPosition.X); }

	}

	private void ThrowProjectile(){

		if (pressedAction3)
		{
			if (myGranny.currentState == granny.State.Idle || myGranny.currentState == granny.State.Walk || myGranny.currentState == granny.State.Run ||
			myGranny.currentState == granny.State.Jump || myGranny.currentState == granny.State.Fall){

				if (!throwingBall){

					if (myGranny.currentHeadgear == Headgear.HeadgearType.Firehat){
						myBall = myProjectile.Instantiate<PlayerProjectile>();
						myBall.startPosition = GlobalPosition;
						myBall.initialVelocity = new Vector2(myBall.directionMod*300, 300);
						
						if(myGranny.currentDirection == granny.Direction.Left){
							myBall.directionMod = -1;
							}else{
								myBall.directionMod = 1;
							}
						
						itemMan.AddChild(myBall);
						myBall.DetermineBallType(0);
						throwingBall = true;
						throwTimer.Start();
					}
					else if (myGranny.currentHeadgear == Headgear.HeadgearType.Winterhat){
						

						myBall = myProjectile.Instantiate<PlayerProjectile>();
						myBall.startPosition = GlobalPosition;
						myBall.initialVelocity = new Vector2(myBall.directionMod*300, 300);
						
						if(myGranny.currentDirection == granny.Direction.Left){
							myBall.directionMod = -1;
							}else{
								myBall.directionMod = 1;
							}
						
						itemMan.AddChild(myBall);
						myBall.DetermineBallType(1);
						throwingBall = true;
						throwTimer.Start();

					}
				}


			}
		}
	}

	private void Charge(){

		if(pressedAction3 && myGranny.IsOnFloor() && !underWater && !charging && !chargeCooldown && !chargeIsHeld && !myGranny.isLanding  && !myGranny.carrying 
		&& !isStomping && !isStompBouncing && myGranny.currentHeadgear != Headgear.HeadgearType.Visor && myGranny.currentHeadgear != Headgear.HeadgearType.Firehat &&
		myGranny.currentHeadgear != Headgear.HeadgearType.Winterhat){

			if(myGranny.currentHeadgear == Headgear.HeadgearType.BikeHelm){
				chargeTimer.WaitTime =.36f;
				myGranny.isRollerCharging = true;
				doubleEffectPlay = false;
			}
			chargeTimer.Start();
			charging = true;
			chargeCooldown = true;

		}

		if(pressedAction3 && underWater && !chargeCooldown && !chargeIsHeld && myGranny.currentHeadgear == Headgear.HeadgearType.Snorkel
		&& !isStomping && !isStompBouncing){
			
			chargeTimer.Start();
			charging = true;
			chargeCooldown = true;

		}

		if(charging){
			if(myGranny.currentDirection == granny.Direction.Left){
				myVelocity.X = -chargeSpeed;
			}
			if(myGranny.currentDirection == granny.Direction.Right){
				myVelocity.X = chargeSpeed;
			}
			// if(myVelocity.Y < 0){
			// 	myVelocity.Y = 0;
			// }
			myVelocity.Y = 100;
			if (canDestroyItem){
				destroyableItem.CheckIfChargedItemCanBeDestroyed();
			}
		}

		if(!pressedAction3){
			chargeIsHeld = false;
		}



	}

	public void ChargeThrough(Vector2 brokenItemPos){
		isChargingThrough = true;
		chargedItemPosition = brokenItemPos;

	}    

	private void Stomp(double delta){

		 if(pressedAction3 && !myGranny.IsOnFloor() && !underWater && myGranny.currentHeadgear == Headgear.HeadgearType.Hardhat){
			isStomping = true;
		 }
		 if(isStomping || isStompBouncing){

			if(stompingBox){
				stompCounter += delta;
			}
			if(stompCounter >= .05){
				stompingBox = false;
				stompCounter = 0;
			}
			
		 }
		 if(myGranny.IsOnFloor() && isStomping){

			stompCounter += delta;

			if(stompCounter >= .05 && !stompingBox){
				isStomping = false;
				myGranny.isBashing = false;
				isStompBouncing = true;
			}
			
		 }
		 if(isStompBouncing){
			stompBounceCounter += delta;
		 }
		if(stompBounceCounter >= .4){
			isStompBouncing = false;
			stompBounceCounter = 0;
		}


	}

	public void TakeDamage(Vector2 enemyPos){

		enemyPosition = enemyPos;
		direction = GlobalPosition - enemyPosition;
		isHurt = true;
		hurtTimer.Start();
	
	}

	private void OnWaterDetectorStateChanged(bool isInWater){

		underWater = isInWater;
		if(isInWater){myVelocity.Y /= 5;}
	}

	public void OnLadderEntered(){
		onLadder = true;
	}

	public void OnLadderExited(){
		onLadder = false;
	}
	public void BouncePlayer(){
		myVelocity.Y = -.75f*jumpPower;
	}

	private void OnChargeTimerTimeout(){
		
		
		if(!isChargingThrough){

			pressedAction3 = false;
			myVelocity.X = 0;
			myVelocity.Y = 0;
			charging = false;
			myGranny.isRollerCharging = false;
			chargeTimer.WaitTime = .2f;
			chargeCooldownTimer.Start();
			myGranny.isBashing = false;

		}else{

			chargeThroughTimer.Start();
		}   
	
	}
	private void OnChargeThroughTimerTimeout(){
		myVelocity.X = 0;
		myVelocity.Y = 0;
		charging = false;
		myGranny.isRollerCharging = false;
		isChargingThrough = false;
		doubleEffectPlay = true;
		chargeCooldownTimer.Start();
	}
	private void OnChargeCooldownTimerTimeout(){
		chargeCooldown = false;
		if(pressedAction3){
			chargeIsHeld = true;
		}
	}
	private void OnHurtTimerTimeout(){
		isHurt = false;
	}

	public void GetDestroyableItem(carryable_item item){

		destroyableItem = item;
		canDestroyItem = true;
	}


	private void OnThrowTimerTimeout()
	{
		throwingBall = false;
	}
}









