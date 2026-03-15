using Godot;
using System;
using System.Collections.Generic;
using System.IO;
//using System.Numerics;

public partial class HurtboxComponent : Area2D
{

	[Signal] public delegate void enemyCollideEventHandler(Vector2 enemyPosition);  ////  handles enemy-to-enemy collisions
	[Export] private HealthComponent healthComponent;
	[Export] private unitType currentUnitType;
	[Export] private Timer isPunchedTimer;
	[Export] private PackedScene hitFXScene;

	private EffectsManager fxMan;
	private EnemyManager enemyMan;
	private HitEffect myHitFX;
	private float hitRadians, hitDegrees;
	private int damageToGive, knockbackApplied, stunApplied;
	private CharacterBody2D gloveBody, enemyBody, playerBody, other;
	private List<PathfindComponent> adjacentEnemiesList;
	private bool isPunched, isTouchingAnotherEnemy;
	public int enemiesTouching = 0;

	private enum unitType {
		player, enemy, trap
	}

	public override void _Ready()
	{
		adjacentEnemiesList = new List<PathfindComponent>();

		fxMan = (EffectsManager)GetNode("/root/EffectsManager");
		
	}

	private void OnBodyEntered(Node2D body)
	{

		if(body.IsInGroup("Enemy") && currentUnitType == unitType.enemy && body != this){

			adjacentEnemiesList.Add(body.GetNode<PathfindComponent>("./PathfindComponent"));
			enemiesTouching = adjacentEnemiesList.Count;

			if(adjacentEnemiesList.Count > 1){
				isTouchingAnotherEnemy = true;
			}
			
		}

		if (body.IsInGroup("Glove") && currentUnitType == unitType.enemy)
		{
			damageToGive = body.GetNode<PunchActionComponent>("..").punchDamage;
			knockbackApplied = body.GetNode<PunchActionComponent>("..").knockback;
			stunApplied = body.GetNode<PunchActionComponent>("..").stunFactor;
			gloveBody = body.GetNode<CharacterBody2D>(".");
			playerBody = body.GetNode<CharacterBody2D>("../..");
			healthComponent.TakeDamage(damageToGive, knockbackApplied, stunApplied, playerBody.GlobalPosition);
			isPunched = true;
			isPunchedTimer.Start();

			if(gloveBody == null){

				GD.PrintErr("Glove Body Not found, OOPS!");

			}else{
				
				
				PlayHitEffect(gloveBody, GlobalPosition);

			}


			if(enemiesTouching > 1){
				foreach(PathfindComponent enemy in adjacentEnemiesList){
					enemy.ActivateSecondaryKnockback((GlobalPosition - playerBody.GlobalPosition).Normalized(), knockbackApplied, stunApplied);
				}
			}

		}
		if (body.IsInGroup("EnemyHand") && currentUnitType == unitType.player){

			damageToGive = 5;
			enemyBody = body.GetNode<CharacterBody2D>(".");

			// if(enemyBody != null){
			// 	GD.Print("Enemy Body FOund" + enemyBody.GlobalPosition.ToString());
			// }else{
			// 	GD.Print("Enemy Body FOund" + enemyBody.GlobalPosition.ToString());
			// }

			// knockbackApplied = body.GetNode<PunchActionComponent>("..").knockback;
			// stunApplied = body.GetNode<PunchActionComponent>("..").stunFactor;
			// playerBody = body.GetNode<CharacterBody2D>("../..");
			
			healthComponent.TakeDamage(damageToGive,0, 0, enemyBody.GlobalPosition);
		
		}
		if(body.IsInGroup("Glove") && currentUnitType == unitType.trap){
			
		}

	}

	private void OnBodyExited(Node2D body){

		if(body.IsInGroup("Enemy") && currentUnitType == unitType.enemy){

			adjacentEnemiesList.Remove(body.GetNode<PathfindComponent>("./PathfindComponent"));
			enemiesTouching = adjacentEnemiesList.Count;
			if(adjacentEnemiesList.Count == 1){
				isTouchingAnotherEnemy = false;
			}

		}
	}

	private void OnIsPunchedTimerTimeout(){
		isPunched = false;
	}

	private void PlayHitEffect(CharacterBody2D strikingBody, Vector2 struckBody){

		Vector2 strikeDirection = strikingBody.GlobalPosition - GlobalPosition;
		hitRadians = Mathf.Atan2(strikeDirection.Y, strikeDirection.X);
		hitDegrees = Mathf.RadToDeg(hitRadians);
		
		myHitFX = hitFXScene.Instantiate<HitEffect>();
		myHitFX.GlobalPosition = (struckBody + strikingBody.GlobalPosition)/2;
		myHitFX.GlobalPosition = struckBody; 
		myHitFX.PlayHitEffect(myHitFX.GlobalPosition, hitDegrees);

		fxMan.AddChild(myHitFX);

	}


}



