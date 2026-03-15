using Godot;
using System;

public partial class collectible_item : CharacterBody2D
{
	[Export] private int gravity;
	[Export] public ItemType thisType;
	[Export] private Texture2D coinText, coinBagText, sodaText, keyText, hardHatText, vikingText, 
	bikeHelmText, sunhatText, snorkelText, winterhatText, firehatText, visorText;
	private Texture2D thisTexture;
	public enum ItemType{
		Coin, CoinBag, Soda, Key, Hat
	}
	public enum State{
		Idle, Collect
	}

	[Export] public Headgear.HeadgearType hatType;
	private double coinThrowCounter;
	private string animPath;
	private Random random = new Random();
	private Vector2 randomDirection;
	public CollisionShape2D myColl;
	public Sprite2D mySprite;
	private AnimationPlayer myAnim;
	private Vector2 myVelocity, throwDirection;
	private Timer collectTimer;
	public Timer collisionDelayTimer;
	private bool underWater, hidden, thrownFromBox;
	public bool hasBeenCollected;


	public override void _Ready(){

		myAnim = GetNode<AnimationPlayer>("ItemAnim");
		collectTimer = GetNode<Timer>("CollectTimer");
		myColl = GetNode<CollisionShape2D>("ItemCollider");
		mySprite = GetNode<Sprite2D>("ItemSprite");
		collisionDelayTimer = GetNode<Timer>("CollisionDelayTimer");

		
		DetermineSprite();
		myAnim.Play($"{animPath}Spin");
	}

	public override void _PhysicsProcess(double delta){
		ApplyGravity(delta);
		Velocity = myVelocity;
		MoveAndSlide();

	}

	public void Collect()
	{
		hasBeenCollected = true;
		collectTimer.Start();
		thisTexture = coinText;
		myAnim.Play($"{animPath}Collect");

	}

	public void HideItem(){

		hidden = true;
		mySprite.Visible = false;
		myColl.Disabled = true;
	}

	public void RevealItem(){
		
		hidden = false;
		mySprite.Visible = true;
		DelayCollisionTemporarily();
  
	}

	public void DelayCollisionTemporarily(){

		//CallDeferred("SetCollisionLayerValue", 4, false);
		//SetDeferred("CollisionLayerValue", false);
		SetCollisionLayerValue(4, false);
		CallDeferred("EnableCollisionShape");
		if(collisionDelayTimer == null){
			collisionDelayTimer = GetNode<Timer>("CollisionDelayTimer");
		}
		collisionDelayTimer.Start();
	}

	private void EnableCollisionShape(){
		if(myColl == null){
			myColl = GetNode<CollisionShape2D>("ItemCollider");
		}
		myColl.Disabled = false;
	}


	private void ApplyGravity(double delta)
	{
		if (!underWater)
		{
			if (!IsOnFloor())
			{
				myVelocity.Y += gravity * (float)delta;
			}
			if(IsOnFloor() && myVelocity.Y > 10){
				myVelocity.Y -= .25f * gravity * (float)delta;;
			} 
		}
		else if (underWater)
		{
			if (!IsOnFloor())
			{
				myVelocity.Y += .3f*gravity * (float)delta;
			}else{
				myVelocity.Y = 0;
			}
		}
		if(hidden){
			myVelocity.X = 0;
			myVelocity.Y = 0;
		}

		if(thrownFromBox){
			coinThrowCounter++;
			myVelocity = 40 * randomDirection;
			if(!IsOnFloor()){
				myVelocity.Y += (float)coinThrowCounter;
			}
			if(IsOnFloor()){
				
				myVelocity.Y = 0;
				if(myVelocity.X > 0|| myVelocity.X < 0){ myVelocity.X /= myVelocity.X*2;}

			}
		}
	}

	private void DetermineSprite(){
		switch(thisType){
			case ItemType.Coin:

			animPath = "coin";
			thisTexture = coinText;
			hatType = Headgear.HeadgearType.None;

			break;
			case ItemType.CoinBag:

			animPath = "coinbag";
			thisTexture = coinBagText;
			hatType = Headgear.HeadgearType.None;

			break;
			case ItemType.Soda:

			animPath = "soda";
			thisTexture = sodaText; 
			hatType = Headgear.HeadgearType.None;

			break;
			case ItemType.Key:

			animPath = "key";
			thisTexture = keyText;
			hatType = Headgear.HeadgearType.None;

			break;
			case ItemType.Hat:
				animPath = "hat";
				switch(hatType){
					case Headgear.HeadgearType.BikeHelm:

					thisTexture = bikeHelmText;
					//animPath = "hat";
					break;
					case Headgear.HeadgearType.Firehat:
					thisTexture = firehatText;  
					//animPath = "hat";
					break;
					case Headgear.HeadgearType.Hardhat:
					thisTexture = hardHatText;
					//animPath = "hardhat";
					break;
					case Headgear.HeadgearType.Snorkel:
					thisTexture = snorkelText;  
					//animPath = "snorkel";
					break;
					case Headgear.HeadgearType.Sunhat:
					thisTexture = sunhatText;  
					//animPath = "sunhat";
					break;
					case Headgear.HeadgearType.Viking:
					thisTexture = vikingText; 
					//animPath = "viking"; 
					break;
					case Headgear.HeadgearType.Visor:
					thisTexture = visorText;  
					//animPath = "visor";
					break;
					case Headgear.HeadgearType.Winterhat:
					thisTexture = winterhatText;  
				  
					break;
				   
				}

			break;
		}

		mySprite.Texture = thisTexture;
	}

	public void ThrowInRandomDirection(){

		thrownFromBox = true;

		double randomAngle = random.NextDouble() * Math.PI * 2;
		float x = (float)Math.Cos(randomAngle);
		float y = (float)Math.Sin(randomAngle);

		randomDirection =  new Vector2(x, y);
	}

	private void OnCollectTimerTimeout(){
		QueueFree();
	}

	private void OnCollisionDelayTimerTimeout(){
		SetCollisionLayerValue(4, true);
	}
}
