using Godot;
using System;

public partial class EffectsManager : Node2D
{
	private granny myGranny;
	private AnimationPlayer myAnim, myAnim2;
	private Sprite2D impactSprite, playerFXSprite;
	private Vector2 fxOffset = new Vector2(0,-31), laserColliderPosition;
	private Area2D enemyDetectionArea;
	private CollisionShape2D enemyDetector;
	private Damageable damageable;
	private carryable_item carryableItem;
	public override void _Ready()
	{
		myGranny = GetNode<granny>("/root/Scene/Granny");
		myAnim = GetNode<AnimationPlayer>("EffectsAnim");
		myAnim2 = GetNode<AnimationPlayer>("EffectsAnim2");
		impactSprite = GetNode<Sprite2D>("ImpactFX");
		playerFXSprite = GetNode<Sprite2D>("PlayerFX");
		enemyDetectionArea = GetNode<Area2D>("EnemyDetectionArea");
		enemyDetector = GetNode<CollisionShape2D>("EnemyDetectionArea/EnemyDetector");
	}

	public override void _PhysicsProcess(double delta)
	{

	}

	public void PlayEffectAnimation(string anim, Vector2 position){

		impactSprite.Position = position;
		myAnim.Play(anim);

	}

	public void PlayPlayerEffectAnimation(string anim){

		playerFXSprite.Visible = true;
		playerFXSprite.Position = myGranny.Position;
		
		if(myGranny.currentDirection == granny.Direction.Left){
			playerFXSprite.FlipH = true;
			fxOffset.X = 5;
			playerFXSprite.Offset = fxOffset;
			laserColliderPosition = new Vector2(myGranny.Position.X - 28, myGranny.Position.Y);
			
		}else{
			fxOffset.X = -5;
			playerFXSprite.Offset = fxOffset;
			playerFXSprite.FlipH = false;
			laserColliderPosition = new Vector2(myGranny.Position.X + 28, myGranny.Position.Y);
		}

		enemyDetector.Position = laserColliderPosition;

		if(anim == "Laser_FX"){
			playerFXSprite.ZIndex = 8;
		}else{
			playerFXSprite.ZIndex = 2;
		}
		myAnim2.Play(anim);

	}

	public void StopPlayerEffectAnimation(string anim){

		if(myAnim2.IsPlaying() && myAnim2.CurrentAnimation == anim){
			myAnim2.Stop();
			playerFXSprite.Visible = false;
		}
	}



	private void OnEnemyDetectionAreaEntered(Node2D body)
	{
		if(body.IsInGroup("Enemy")){

			damageable = body.GetNode<Damageable>("Damageable");
			damageable.TakeDamageII("laser");
		}

		if(body.IsInGroup("Throwable")){
			carryableItem = body.GetNode<carryable_item>(".");
			carryableItem.DestroyWithLaser();
		}

	}
}



