using Godot;
using System;


public partial class carryable_item : CharacterBody2D
{
	[Export] private int gravity, speed, acceleration;
	[Export] private Texture2D itemImage, frozenItem;
	[Export] public PlayerCarryable.CarryableType carryType;
	[Export] public collectible_item.ItemType containedItem;
	[Export] public int containedItemQuan;
	[Export] public string itemManNodePath;
	public int boxIndex;
	[Signal] public delegate void ItemDestroyedEventHandler();
	[Signal] public delegate void EnemyCarriedEventHandler();
	[Export] private GpuParticles2D dustParticles, gooParticles, iceParticles, explosionParticles;
	private GpuParticles2D myParticles;
	private Container container;	
	private Label StateLabel;
	private Vector2 myVelocity, noMovement;
	private RayCast2D myUpcast, myDowncast;
	private ItemManager itemMan;
	private PlayerManager playerMan;
	private CollisionShape2D myCollider;
	private Sprite2D mySprite;
	private PlayerCarryable playerCarryable;
	private granny myGranny;
	private PlayerMovement pMove;
	private State currentState, nextState;
	public bool touchingPlayer, carryable, carried, noLongerCarryable, thrown, bounced, 
	hasDirection, contentsReleased, carryableEnemy, frozen, broken;
	private bool hasBeenActivated;
	private Timer breakTimer, stompTimer;
	private PlayerProjectile myBall;

	
	public enum State{
		Falling,
		Idle, 
		Carryable,
		Carried,
		Thrown,
		Bounced,
		Breaking,
		Broken
	} 
	public override void _Ready(){
		
		currentState = State.Idle;
		StateLabel = GetNode<Label>("StateLabel");

		playerMan = GetNode<PlayerManager>("/root/Scene/PlayerManager");
		mySprite = GetNode<Sprite2D>("ItemSprite");
		myCollider = GetNode<CollisionShape2D>("ItemBorder");
		breakTimer = GetNode<Timer>("BreakTimer");
		stompTimer = GetNode<Timer>("StompTimer");
		myParticles = GetNode<GpuParticles2D>("DustParticles");
		container = GetNode<Container>("Container");
		myUpcast = GetNode<RayCast2D>("BoxCheckUp");
		myDowncast = GetNode<RayCast2D>("BoxCheckDown");

		noMovement = new Vector2(0, 0);
		mySprite.Texture = itemImage;

		itemMan = GetNode<ItemManager>(itemManNodePath);

		
	}

	public override void _PhysicsProcess(double delta)
	{   
		DetermineState(delta);
		ApplyGravity(delta);
		ActivatePlayerCarry();

		if(carryable){
			CheckToDropCarryable();
		}
		
		if(currentState == State.Breaking){
			ExplodeItem();
		}
		Velocity = myVelocity;
		MoveAndSlide();
		currentState = nextState;

		
		ReadLabel();
		
	}

	private void ApplyGravity(double delta)
	{
		

		if (!IsOnFloor() && currentState != State.Carried && !myDowncast.IsColliding() )
		{
			myVelocity.Y += gravity * (float)delta;

		}
		if (IsOnFloor() && currentState != State.Bounced || myCollider.Disabled)
		{

			myVelocity.Y = 0;
			myVelocity.X = 0;
		}

	}

	private void DetermineState(double delta){

		if(!IsOnFloor()){

			if(currentState == State.Thrown && !hasDirection){
				if(myGranny.currentDirection == granny.Direction.Left){myVelocity.X = -200;}
				if(myGranny.currentDirection == granny.Direction.Right){myVelocity.X = 200;}
				hasDirection = true;
				
			}

			if(myVelocity.X <= 0 && currentState != State.Thrown){
				nextState = State.Falling;
			}

			if(breakTimer.TimeLeft > 0){
				nextState = State.Breaking;
				myVelocity.Y = 0;
			}
			
		}
		if(IsOnFloor()){

			if(carryable){
				nextState = State.Carryable;
			}else{
				nextState = State.Idle;
			}

			if(thrown){
				nextState = State.Breaking;
				breakTimer.Start();
				//thrown = false;
				
			}
			if(bounced){
			nextState = State.Bounced;

			}

		}

	}

	private void ActivatePlayerCarry(){

		if(touchingPlayer && !noLongerCarryable){
			
			CheckIfThisItemCanBeCarried();
			
		}
		
		if(carryable && playerMan.carryTriggered && !thrown && !myGranny.carrying && !myGranny.isUnderwater && !noLongerCarryable){

			if(frozen){mySprite.Texture = frozenItem;}
			
			nextState = State.Carried;
			myGranny.carrying = true;
			mySprite.Visible = false;
			myCollider.Disabled = true;
			playerCarryable.DetermineCarryableType(carryType, frozen);
			playerCarryable.ActivateCarryable();
			EmitSignal("EnemyCarried");
			carried = true;
			hasBeenActivated = true;
		}
	}

	private void CheckToDropCarryable(){

		if(playerCarryable.carrying && carried){
			if(!myGranny.carrying || myGranny.isUnderwater){
				ThrowCarriedItem();
			}
		}
	}

	private void DropCarriedBox(){

		GlobalPosition = new Vector2(myGranny.Position.X +15, myGranny.Position.Y);
		playerCarryable.DeactivateCarryable();
		myGranny.carrying = false;
		mySprite.Visible = true;
		myCollider.Disabled = false;
	}

	private void ThrowCarriedItem(){

		GlobalPosition = new Vector2(myGranny.Position.X , myGranny.Position.Y - 32);
		
		nextState = State.Thrown;
		playerCarryable.DeactivateCarryable();
		
		myGranny.PlayerThrow();
		myGranny.carrying = false;
		mySprite.Visible = true;
		myCollider.Disabled = false;
		carried = false;
		thrown = true;
		SetCollisionLayerValue(6, true);
		//playerDetector.SetCollisionMaskValue(3, false);
		SetCollisionLayerValue(8, false);
	}

	private void ExplodeItem(){

		mySprite.Visible = false;
		CallDeferred(nameof(DisableCollisionShape));
		DetermineParticleType();
		myParticles.Emitting = true;

		ReleaseContents();
		
		if(!broken){
			breakTimer.Start();
			broken =true;
		}

	}

	private void DetermineParticleType(){

		if (carryType == PlayerCarryable.CarryableType.Grub)
			{
				if(frozen){
					myParticles = iceParticles;
				}else{
					myParticles = gooParticles;
				}

			}
			if (carryType == PlayerCarryable.CarryableType.Box)
			{	
				
				myParticles = dustParticles;

			}

	}
	private void ReleaseContents(){

		
		if(!contentsReleased){
			
			contentsReleased = true;
			Vector2 coinPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y -15);
			// if(itemMan == null){
				
			// }
			itemMan.ReleaseContentsOfContainer(containedItemQuan, containedItem, coinPosition);
			
		}

	}

	private void DisableCollisionShape(){
		myCollider.Disabled = true;
	}


	private void ReadLabel(){
		//StateLabel.Text =  carried.ToString();
	}


	private void CheckIfThisItemCanBeCarried(){

		if (myGranny != null)
		{
			if (carryType == PlayerCarryable.CarryableType.Grub)
			{
				if (!myGranny.carrying && !pMove.isCrouched && carryableEnemy)
				{
					carryable = true;
				}
			}
			if (carryType == PlayerCarryable.CarryableType.Box)
			{

				if (!myGranny.carrying && !pMove.isCrouched)
				{
					carryable = true;
				}
			}
		}
	}

	public void CheckIfChargedItemCanBeDestroyed(){

		if (carryType == PlayerCarryable.CarryableType.Box)
		{
			
			if(pMove.isStomping){
				
				pMove.stompingBox = true;

				ExplodeItem();
				breakTimer.Start();
				myGranny.isBashing = true;
				//stompTimer.Start();

			}else{

				myGranny.isBashing = true;
				ExplodeItem();
				pMove.ChargeThrough(GlobalPosition);
				breakTimer.Start();

			}
			
		}
	}


	private void BurnItemWithFireball(){

		if(carryType == PlayerCarryable.CarryableType.Box){
			myBall.DestroyProjectile();
			ExplodeItem();
			breakTimer.Start();
			hasBeenActivated = true;
		}
		
	}

	private void FreezeItemWithIceball(){

		if(carryType == PlayerCarryable.CarryableType.Box){

		myBall.DestroyProjectile();
		ExplodeItem();
		breakTimer.Start();
		hasBeenActivated = true;
		}
	}

	public void DestroyWithLaser(){

		if (carryType == PlayerCarryable.CarryableType.Box)
		{
			ExplodeItem();
			breakTimer.Start();
			hasBeenActivated = true;
		}
	}
	///////////////////////////////////////////////////////////////////////////////// ON BODY ENTERED /////////////////////////////////////////////////
	private void OnArea2DBodyEntered(PhysicsBody2D body){ 

		if (body == null || hasBeenActivated){
			carryable = false;
			return;
		}

		if(body.IsInGroup("Player")){

			myGranny = body.GetNode<granny>(".");
			pMove = body.GetNode<PlayerMovement>("PlayerMovement");
			playerCarryable =  body.GetNode<PlayerCarryable>("PlayerCarryable");

			if(thrown || noLongerCarryable){return;}

			if(!pMove.charging && !pMove.isStomping){

				CheckIfThisItemCanBeCarried();
				touchingPlayer = true;
				pMove.GetDestroyableItem(this);
	
			}else if(pMove.charging || pMove.isStomping){

				CheckIfChargedItemCanBeDestroyed();
			}
 
		}

		if(body.IsInGroup("Projectile")){

	
			myBall = body.GetNode<PlayerProjectile>(".");

			if(myBall.currentBallType == PlayerProjectile.ballType.fireBall){
				BurnItemWithFireball();

			}else if(myBall.currentBallType == PlayerProjectile.ballType.iceBall){
				FreezeItemWithIceball();
			}
		}
	}

	private void OnArea2DBodyExited(PhysicsBody2D body){

		if(body.IsInGroup("Player")){

			if(carried || thrown){
				
				carryable = true;
				touchingPlayer = true;
			}else{

				carryable = false;
				touchingPlayer = false;
				if(pMove != null){
					pMove.canDestroyItem = false;
				}
				
			}
		}
	}

	private void OnBreakTimerTimeout()
	{

		switch (carryType)
		{
			case PlayerCarryable.CarryableType.Box:

				if(myGranny != null){
					myGranny.isBashing = false;
				}
				if(pMove != null){
					pMove.canDestroyItem = false;
				}

				QueueFree();
				break;

			case PlayerCarryable.CarryableType.Grub:

				mySprite.Visible = false;
				myCollider.Disabled = true;
				EmitSignal("ItemDestroyed");
				break;
		}

	}

	private void OnStompTimerTimeout()
	{
		pMove.stompingBox = false;
	
	}
}



