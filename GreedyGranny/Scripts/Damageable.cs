using Godot;
using System;
using System.Collections.Generic;

public partial class Damageable : Node2D
{
	[Signal] public delegate void carryableStatusEventHandler(bool carryableStatus);
	[Signal] public delegate void carriedStatusEventHandler(bool carriedStatus);
	[Signal] public delegate void killEnemyEventHandler(string method1, string method2);
	[Signal] public delegate void freezeEnemyEventHandler();
	[Export] private int hitPoints;
	private string method1, method2, method3, method4, method5, method6, method7, method8, method9;
	private granny myGranny;
	private EnemyPatrol enemyPatrol;
	private PlayerCarryable playerCarryable;
	private carryable_item carryableItem;
	private RayCast2D takeDmgSensor;
	private Node2D damageSensor;
	private int dmgSensorsTriggered = 0;
	private List<RayCast2D> dmgSensorList;
	private string[] methodActions = new string[9]; 
	private Sprite2D mySprite;

	private PlayerMovement pMove;
	private Timer hitColorTimer;
	private PlayerProjectile myBall;
	private CollisionShape2D myColl, myPlayerSensor;
	private bool alreadyTakingDmg, colorChangeTriggered, isFrozen, noLongerCarryable;
	public bool isUpsideDown, canDamagePlayer, isMoving;
	private PlayerManager playerMan;

	public override void _Ready(){

		enemyPatrol = GetNode<EnemyPatrol>("../EnemyPatrol");
		damageSensor = GetNode<Node2D>("DamageSensors");
		hitColorTimer = GetNode<Timer>("../HitColorTimer");
		carryableItem = GetNode<carryable_item>("../CarryableItem");
		myPlayerSensor = GetNode<CollisionShape2D>("PlayerSensor/PlayerSensorCollider");
		mySprite = GetNode<Sprite2D>("../EnemySprite");
		playerMan = GetNode<PlayerManager>("/root/Scene/PlayerManager");

		carryableItem.Visible = false;
		dmgSensorList = new List<RayCast2D>();

		foreach(RayCast2D ray in damageSensor.GetChildren()){
			
			dmgSensorList.Add(ray);
		
		}

	}

	public override void _PhysicsProcess(double delta){


	}


	public void TakeDamageII(string damageType){

		if (!alreadyTakingDmg)
		{
			hitPoints--;

			switch (damageType){
				
				case "jump":
				methodActions = new string[]{ "BouncePlayer", "ChangeColorWhenHit", "null", 
				"BouncePlayer", "ChangeColorWhenHit", "FlipEnemyUpsideDown", "BouncePlayer", "DeactivateAndDropEnemy" , "ReleaseCoin"};
				break;
				case "stomp":
				//Takes extra damage
				hitPoints--;
				methodActions = new string[]{ "BouncePlayer", "ChangeColorWhenHit", "null", 
				"BouncePlayer", "ChangeColorWhenHit", "FlipEnemyUpsideDown", "BouncePlayer", "ExplodeEnemy" , "null"};

				break;
				case "charge":
				methodActions = new string[]{ "BounceEnemyOffPlayer", "ChangeColorWhenHit", "null", 
				"BounceEnemyOffPlayer", "ChangeColorWhenHit", "FlipEnemyUpsideDown", "BounceEnemyOffPlayer", "DeactivateAndDropEnemy" , "ReleaseCoin"};

				break;
				case "throwable":
				//Takes extra damage
				hitPoints--;
				methodActions = new string[]{  "ChangeColorWhenHit", "null", "null", 
				"ChangeColorWhenHit", "FlipEnemyUpsideDown", "null",  "null", "DeactivateAndDropEnemy" , "ReleaseCoin"};

				break;
				case "fire":
				//Takes extra damage
				hitPoints--;
				methodActions = new string[]{  "ChangeColorWhenHit", "null", "null", 
				"ChangeColorWhenHit", "null", "null",  "null", "BurnEnemy" , "null"};
	
				break;
				case "ice":

				methodActions = new string[]{  "ChangeColorWhenHit", "null", "null", 
				"FreezeEnemy", "null", "null",  "null", "ShatterFrozenEnemy" , "ReleaseCoin"};

				//hitPoints--;
				break;
				case "laser":
				//Takes extra damage
				hitPoints--;
				methodActions = new string[]{  "ChangeColorWhenHit", "null", "null", 
				"ChangeColorWhenHit", "null", "null",  "null", "ExplodeEnemy" , "null"};

				break;
			}

			if(hitPoints > 1){

				//ChangeColorWhenHit();
				CallMethodByName(methodActions[0]);
				CallMethodByName(methodActions[1]);
				CallMethodByName(methodActions[2]);

			}else if(hitPoints == 1){

				// ChangeColorWhenHit();
				// isUpsideDown = true;
				// enemyPatrol.isUpsideDown = true;
				CallMethodByName(methodActions[3]);
				CallMethodByName(methodActions[4]);
				CallMethodByName(methodActions[5]);

			}else if(hitPoints <= 0){
				CallMethodByName(methodActions[6]);
				EmitSignal("killEnemy", methodActions[7], methodActions[8]);
			}

			alreadyTakingDmg = true;
			
		}else{
			return;
		}

	}

	private void ChangeColorWhenHit(){
		if(!colorChangeTriggered){

			mySprite.Modulate = new Color (1,0,0,1);
			hitColorTimer.Start();
			colorChangeTriggered = true;
		}
	}

	private void BounceEnemyOffPlayer(){

		enemyPatrol.BounceOffOfPlayer(pMove.myVelocity.X);
		enemyPatrol.myVelocity.X  = pMove.myVelocity.X - GlobalPosition.X;

		carryableItem.bounced = true;
		enemyPatrol.isBounced = true;
		canDamagePlayer = false;
	}

	private void BouncePlayer(){

		pMove.BouncePlayer();

		if (hitPoints == 0){
			//noLongerCarryable = true;
			carryableItem.noLongerCarryable = true;
		}


	}

	private void FlipEnemyUpsideDown(){

		isUpsideDown = true;
		enemyPatrol.isMotionless = true;
	}

	private void FreezeEnemy(){
		
		EmitSignal("freezeEnemy");
		isFrozen = true;
	
	}

	private void CallMethodByName(string methodName){
		if(methodName != "null"){
			Call(methodName);
		}
	}


	private void OnPlayerSensorBodyEntered(Node2D body){

		if(body.IsInGroup("Player")){
			
			pMove =  body.GetNode<PlayerMovement>("PlayerMovement");
			playerCarryable =  body.GetNode<PlayerCarryable>("PlayerCarryable");
			myGranny = body.GetNode<granny>(".");

			EmitSignal("carryableStatus", true);
			enemyPatrol.DetermineFloorPosition(GlobalPosition);

			foreach (RayCast2D ray in dmgSensorList)
			{
				if (ray.IsColliding() && !pMove.charging)
				{

					if (pMove.pressedAction1 && !pMove.isStomping)
					{
						if(isUpsideDown || isFrozen){
							if(myGranny.carrying){
								TakeDamageII("jump");
							}else{
								EmitSignal("carriedStatus", true);
							}
						}else{
							TakeDamageII("jump");
						}	
					}else{
						if(pMove.isStomping){
							TakeDamageII("stomp");
						}else{
							TakeDamageII("jump");
						}
					}
				}else{
					dmgSensorsTriggered++;
				}
			}

			if(dmgSensorsTriggered == dmgSensorList.Count){canDamagePlayer = true;}else{
				canDamagePlayer = false;
			}
   
			dmgSensorsTriggered = 0;

			if(isUpsideDown){canDamagePlayer = false;}

			if(isUpsideDown || isFrozen){

				if(pMove.pressedAction1 && !myGranny.carrying){
					EmitSignal("carriedStatus", true);
					//isCarried = true;
					carryableItem.carryableEnemy = true;
				}
				canDamagePlayer = false;
			}
			
			if (pMove.charging){
				TakeDamageII("charge");
			}

			if(canDamagePlayer){
				pMove.TakeDamage(GlobalPosition);
				canDamagePlayer = false; 
			}		
		}

		if(body.IsInGroup("Throwable")){

			TakeDamageII("throwable");
		}

		if(body.IsInGroup("Projectile")){
			myBall = body.GetNode<PlayerProjectile>(".");
			if(myBall.currentBallType == PlayerProjectile.ballType.fireBall){

				TakeDamageII("fire");
			}else if(myBall.currentBallType == PlayerProjectile.ballType.iceBall){
				
				TakeDamageII("ice");
			}
		}
		alreadyTakingDmg = false;
	}

	private void OnPlayerSensorBodyExited(Node2D body){

		if(body.IsInGroup("Player")){
			EmitSignal("carryableStatus", false);
			 EmitSignal("carriedStatus", false);
			alreadyTakingDmg = false;
		}

	}
}



