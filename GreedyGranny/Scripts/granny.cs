using Godot;
using System;

public partial class granny : CharacterBody2D
{
	private Label myLabel;
	private State nextState;
	public State currentState;
	public Headgear.HeadgearType currentHeadgear;
	public Direction currentDirection;
	private Timer landTimer, throwTimer, laserTimer, laserCooldownTimer;
	public bool readyToCarry, carrying, throwing, isLanding , isIdle, isHurt, isFloating, isRollerCharging, isStomping, isBashing, isInteractable,
	isInteracting, isUnderwater;
	private bool onLadder, inWater, isClimbing,  isSitting, 
	wearingConstructionHelm, wearingBikeHelm, wearingSunHat, wearingSnowCap, wearingVisor, wearingFireHelm, wearingVikingHelm, wearingScuba, shootingLaser,
	laserCooldown;
	private PlayerMovement pMove;
	private Sprite2D mySprite, hatSprite;
	public double idleCounter;
	private PlayerCarryable playerCarryable;
	private PlayerAnimationTree playerAnimationTree;
	private HatManager hatMan;
	private ScoreManager scoreMan;
	private EffectsManager fxMan;
	private SlotInterface slotInterface;
	public enum State {
		Idle, Walk, Run, Jump, Fall, Land, Climb, ClimbIdle,
		CarryIdle, CarryRun, CarryJump, CarryFall, CarryLand,
		Crouch, CrouchWalk, Swim, SwimIdle, Charge, Hurt, Interact, Stomp, RollerCharge,
		LaserIdle, LaserWalk, Throw, Float, Dead, Sitting, Knitting, Hang, SwimCharge, StompBounce
	}
	public enum Direction{
		Left, Right
	}

	[Export] private Color Red, White;	
	private PlayerManager playerMan;

	
	public override void _Ready()
	{
		//Engine.TimeScale = .2f;
		myLabel = GetNode<Label>("StateLabel");
		landTimer = GetNode<Timer>("LandTimer");
		throwTimer = GetNode<Timer>("ThrowTimer");
		pMove = GetNode<PlayerMovement>("PlayerMovement");
		playerCarryable = GetNode<PlayerCarryable>("PlayerCarryable");
		mySprite = GetNode<Sprite2D>("GrannySprite");
		hatSprite = GetNode<Sprite2D>("HatManager/HatSprite");
		playerAnimationTree = GetNode<PlayerAnimationTree>("PlayerAnimationTree");
		hatMan = GetNode<HatManager>("HatManager");
		fxMan = GetNode<EffectsManager>("/root/Scene/EffectsManager");
		playerMan = GetNode<PlayerManager>("/root/Scene/PlayerManager");
		scoreMan = GetNode<ScoreManager>("../GameManager/ScoreManager");
		slotInterface = GetNode<SlotInterface>("../GameManager/SlotInterface");
		laserTimer = GetNode<Timer>("LaserTimer");
		laserCooldownTimer = GetNode<Timer>("LaserCooldownTimer");

	}

	public override void _PhysicsProcess(double delta)
	{       
		CountIdleTime(delta);
		DetermineState();
		FlipSprite();
		//if(pMove.charging && pMove.myVelocity.Y < 0){pMove.myVelocity.Y = 0;}
		pMove.myVelocity.X = MathF.Round(pMove.myVelocity.X);
		pMove.myVelocity.Y = MathF.Round(pMove.myVelocity.Y);
		Velocity = pMove.myVelocity;
		MoveAndSlide(); 
		AnimatedEffects();
		if(currentState != nextState){

			playerAnimationTree.CallDeferred("AnimateGranny");

		}
	
		currentState = nextState;
		myLabel.Text = $"{currentState} ";
	}

	

	private void DetermineState(){

		if(pMove.pressedAction1 && currentState != State.Crouch && currentState != State.CrouchWalk){readyToCarry = true;}
		else{
			readyToCarry = false;
			carrying = false;
		}
		if(isInteractable && pMove.pressedJump && currentState != State.Fall && !isInteracting){
			ActivateInteractable(true);
			isInteracting = true;
			}
		if(!isInteractable){
			if(isInteracting){
				isInteracting = false;
				ActivateInteractable(false);
			}

			}

		if(pMove.pressedAction3 && currentHeadgear == Headgear.HeadgearType.Visor && !laserCooldown){

			shootingLaser = true;
			laserCooldown = true;
			laserTimer.Start();
		}

		if(IsOnFloor() && !pMove.underWater){

			pMove.onFloor = true;

			if(onLadder && pMove.pressedUp || pMove.pressedDown){
				nextState = State.Climb;
				isClimbing = true;
			}
			if(pMove.pressedLeft || pMove.pressedRight){
				
				if(carrying){

					nextState = State.CarryRun;
					JumpFallLand(State.CarryJump, State.CarryFall, State.CarryLand);

				}else{
					if(isInteracting){ nextState = State.Interact;}
					if(!pMove.isStompBouncing && !pMove.isStomping && !pMove.throwingBall && !isInteracting){
						nextState = State.Walk;}
					if(pMove.pressedDown && !pMove.isStompBouncing && !pMove.isStomping  && !pMove.throwingBall && !isInteracting){
						nextState = State.CrouchWalk;
						pMove.isCrouched = true;
						readyToCarry = false;
						}else{
							pMove.isCrouched = false;
						}
					JumpFallLand(State.Jump, State.Fall, State.Land);
					if(shootingLaser){nextState = State.LaserIdle;}
					if(throwing || pMove.throwingBall){nextState = State.Throw;}
					if(pMove.charging ){
					
						if(isRollerCharging){nextState = State.RollerCharge;}
						else{nextState = State.Charge;}

					}
					if(pMove.pressedAction1 && !pMove.pressedJump && !isLanding & !pMove.isStomping && !pMove.isStompBouncing && !pMove.throwingBall
						&& !isInteracting && !pMove.charging){
							nextState = State.Run;}
				}

			}else{

				if(carrying){
					
					nextState = State.CarryIdle;
					if(pMove.pressedJump && !throwing){nextState = State.CarryJump;}
					if(currentState == State.CarryFall && !throwing){
						isLanding = true;
						landTimer.Start();
					}
					if(isLanding && !pMove.pressedJump && !throwing){nextState = State.CarryLand;}
					if(throwing){nextState = State.Throw;}



				}else{

					if(pMove.pressedDown){
						nextState = State.Crouch;
						pMove.isCrouched = true;
						readyToCarry = false;
						}else{
							pMove.isCrouched = false;
						}
					JumpFallLand(State.Jump, State.Fall, State.Land);
					if(shootingLaser && !isInteracting){nextState = State.LaserIdle;}
					if(throwing || pMove.throwingBall){nextState = State.Throw;}
					if(pMove.charging && !isLanding){

						if(isRollerCharging){nextState = State.RollerCharge;}
						else{nextState = State.Charge;}

					}
					if(!pMove.pressedDown && !throwing && !pMove.charging && !pMove.pressedJump && !isLanding 
						&& !pMove.isStomping && !pMove.isStompBouncing && !pMove.throwingBall && !isInteracting && !shootingLaser){
						nextState = State.Idle;
						if(idleCounter >= 15){nextState = State.Sitting;}
						if(idleCounter >= 30){nextState = State.Knitting;}
						
					}
				}
			}
			

		}
		if(!IsOnFloor() && !pMove.underWater){

			pMove.onFloor = false;
			if(pMove.myVelocity.Y > 0){
				if(carrying){nextState = State.CarryFall;}
				if(!carrying){nextState = State.Fall;}
				if(isFloating){nextState = State.Float;}
				if(pMove.isStomping){nextState = State.Stomp;}
				if(pMove.isStompBouncing){nextState = State.StompBounce;}
				if(shootingLaser){nextState = State.LaserIdle;}
			}

			if(pMove.myVelocity.Y < 0 && !pMove.isStompBouncing && !shootingLaser){nextState = State.Jump;}
			if(shootingLaser){nextState = State.LaserIdle;}
			if(pMove.myVelocity.Y < 0 && carrying){nextState = State.CarryJump;}

			if(throwing || pMove.throwingBall){nextState = State.Throw;}
			if(pMove.stuckToCeiling){nextState = State.Hang;}

		}

		if(pMove.underWater){

				
				isUnderwater = true;
				pMove.onFloor = false;
				if(pMove.pressedLeft || pMove.pressedRight){nextState = State.Swim;}
				else{nextState = State.SwimIdle;}
				if(pMove.charging){nextState = State.SwimCharge;}
				if(pMove.isStomping){ 
					pMove.isStomping = false;
					if(pMove.pressedLeft || pMove.pressedRight){nextState = State.Swim;}
					else{nextState = State.SwimIdle;}
				}
				if(carrying){nextState = State.Throw;}

		}else{isUnderwater = false;}

		if (pMove.isClimbing)
		{
			
			if (pMove.myVelocity.Y == 0 && pMove.myVelocity.X == 0){nextState = State.ClimbIdle;}
			else{nextState = State.Climb;}

		}
		if(pMove.isHurt){
			nextState = State.Hurt;
			mySprite.Modulate = Red;
			}else{
				mySprite.Modulate = White;
			}

	}

	private void JumpFallLand(State jump, State fall, State land){

		if(pMove.pressedJump && !shootingLaser){nextState = jump;}
		if(shootingLaser && !isInteracting){nextState = State.LaserIdle;}
		if(pMove.pressedJump && carrying){nextState = State.CarryJump;}
		if(currentState == fall || currentState == State.Float ){
			isLanding = true;
			landTimer.Start();
			}
		if(isLanding && !isFloating && !pMove.isStompBouncing){nextState = land;}
		if(isFloating){nextState = State.Float;}
		if(isStomping){nextState = State.Stomp;}
		if(pMove.isStompBouncing){nextState = State.StompBounce;}
		if(isLanding && !pMove.pressedJump && carrying ){nextState = State.CarryLand;}
		if(isInteracting){nextState = State.Interact;}
	}

	private void FlipSprite(){
		if(currentDirection == Direction.Left){
			mySprite.FlipH = true;
			hatSprite.FlipH = true;
			
		}else if(currentDirection == Direction.Right){
			mySprite.FlipH = false;
			hatSprite.FlipH = false;
		}

	}

	private void AnimatedEffects(){

		if(currentState == State.Charge || currentState == State.Stomp){
			
			if(isBashing){

				Vector2 newPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y -30);
				fxMan.PlayEffectAnimation("Pow", newPosition);
			}



		}
		if(currentState == State.Charge && !pMove.isChargingThrough){
			fxMan.PlayPlayerEffectAnimation("Charge_FX");
		}
		if(currentState == State.Stomp){
			fxMan.PlayPlayerEffectAnimation("Stomp_FX");
		}
		if(currentState == State.StompBounce){
			fxMan.PlayPlayerEffectAnimation("StompBounce_FX");
		}
		if(currentState == State.RollerCharge && !pMove.isChargingThrough && !pMove.doubleEffectPlay){
			fxMan.PlayPlayerEffectAnimation("BladeCharge_FX");
		}
		if(currentState == State.Throw){
			fxMan.PlayPlayerEffectAnimation("Throw_FX");
		}
		if(currentState == State.Hurt){
			fxMan.PlayPlayerEffectAnimation("Hurt_FX");
		}
		if(currentState == State.LaserIdle){
			fxMan.PlayPlayerEffectAnimation("Laser_FX");
		}
		if(currentState == State.SwimCharge && !pMove.isChargingThrough){
			fxMan.PlayPlayerEffectAnimation("SwimCharge_FX");
		}

		if(currentState != State.LaserIdle){
			fxMan.StopPlayerEffectAnimation("Laser_FX");
		}
	}

	private void ActivateInteractable(bool active){

		slotInterface.ActivateDeactivateGame(active);
	}

	private void OnCoinCollected(int coinValue)
	{
		scoreMan.AddCoins(coinValue);
	}

	private void CountIdleTime(double delta){

		if(isIdle){
			idleCounter += delta;
		}else{
			idleCounter = 0;
		}
	}

	public void TakeDamage(){

	}

	public void PlayerThrow(){
		throwTimer.Start();
		throwing = true;
	}

	private void OnHatChanged(){
		
		currentHeadgear = hatMan.currentHat;
	}
	private void OnLandTimerTimeout(){
		isLanding = false;
	}

	private void OnThrowTimerTimeout(){
		throwing = false;
	}

	private void OnLadderEntered(){
		onLadder = true;
	}

	private void OnLadderExited(){
		onLadder = false;	
	}

	private void OnLaserTimerTimeout()
	{
		shootingLaser = false;
		laserCooldownTimer.Start();
	}

	private void OnLaserCooldownTimerTImeout()
	{
		laserCooldown = false;
	}

}










