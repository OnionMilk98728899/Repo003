using Godot;
using System;

public partial class PlayerProjectile : CharacterBody2D
{
	[Export] private Texture2D fireballTexture, iceballTexture, slimeballTexture, eggTexture;
	[Export] private PackedScene myBabyGrubScene;
	private basic_enemy myBabyGrub;
	private Sprite2D ballSprite;
	private GpuParticles2D flameTrail, iceTrail, slimeTrail, eggTrail;
	private float elapsedTime, airTime;
	[Export] public float gravity , speed;
	private Vector2 myVelocity; 
	public Vector2 startPosition, initialVelocity;
	//private Vector2 velocity = new Vector2(40,-.25f);
	public int directionMod;
	public ballType currentBallType;
	public enum ballType{
	   iceBall, fireBall, slimeBall, egg
	}

	private bool isFlying = true;
	public override void _Ready()
	{
		ballSprite = GetNode<Sprite2D>("BallSprite");
		flameTrail = GetNode<GpuParticles2D>("FlameTrail");
		iceTrail = GetNode<GpuParticles2D>("IceTrail");
		slimeTrail = GetNode<GpuParticles2D>("SlimeTrail");
		eggTrail = GetNode<GpuParticles2D>("EggTrail");
		
		//initialVelocity = new Vector2(directionMod*300, 300);
		GlobalPosition = startPosition;
		

	}
	public override void _PhysicsProcess(double delta)
	{
		Velocity = initialVelocity;
		elapsedTime += (float)delta;
		airTime += (float)delta;

		if(IsOnFloor()){
			myVelocity.Y = -50;
			airTime = 0;
			if(currentBallType == ballType.egg){
				HatchGrubling();
				DestroyProjectile();
			}
		}else{
			myVelocity.Y += airTime * gravity;
		}
		
		myVelocity.X = directionMod * speed;

		Velocity = myVelocity;
		MoveAndSlide();

		if(elapsedTime > 1.5 && currentBallType != ballType.egg){
			DestroyProjectile();
		}
	}

	private void EnableColliderAfterThrow(){
		if(elapsedTime >= 1.5){
			SetCollisionLayerValue(1, true);

		}
	}

	public void DestroyProjectile(){
		QueueFree();
	}

	public void DetermineBallType(int ballIndex){
		
		switch(ballIndex){
			case 0: ////FIREBALL
			ballSprite.Texture = fireballTexture;
			DeactivateUnusedTextures(iceTrail, slimeTrail, eggTrail);
			currentBallType = ballType.fireBall;
			break;
			case 1:  /////ICEBALL
			ballSprite.Texture = iceballTexture;
			DeactivateUnusedTextures(flameTrail, slimeTrail, eggTrail);
			currentBallType = ballType.iceBall;
			break;
			case 2: /////SLIMEBALL
			ballSprite.Texture = slimeballTexture;
			DeactivateUnusedTextures(iceTrail, flameTrail, eggTrail);
			currentBallType = ballType.slimeBall;
			break;
			case 3: /////EGG
			ballSprite.Texture = eggTexture;
			DeactivateUnusedTextures(iceTrail, slimeTrail, flameTrail);
			currentBallType = ballType.egg;
			break;
		}
	}

	private void DeactivateUnusedTextures(GpuParticles2D texture1, GpuParticles2D texture2, GpuParticles2D texture3){

		texture1.Visible = false;
		texture2.Visible = false;
		texture3.Visible = false;
	}

	private void HatchGrubling(){

		myBabyGrub = myBabyGrubScene.Instantiate<basic_enemy>();
		myBabyGrub.Position = GlobalPosition;
		myBabyGrub.spawnedByBoss = true;
		GetNode<Node2D>("../..").AddChild(myBabyGrub);
	}

}
