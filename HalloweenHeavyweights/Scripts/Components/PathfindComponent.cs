using Godot;
using System;

public partial class PathfindComponent : Node2D
{
	[Signal] public delegate void insideMeleeRangeEventHandler(bool range, Vector2 directionToPlayer);
	[Signal] public delegate void movementDirectionChangedEventHandler(bool movingUp);
	[Export] private CharacterBody2D unitBody;
	[Export] private Timer knockbackTimer, stunTimer, movementChangeTimer;
	[Export] private HurtboxComponent myHurtbox;
	private BasicControllerComponent myPlayer;
	private CharacterBody2D otherEnemyBody;
	private Vector2 myVelocity, velocityModifier, playerPosition, knockBackModifier, secondaryKnockBackModifier, targetPosition, direction, secondaryDirection;
	private float distance, secondaryKnockBackWeight = .6f, knockBackFriction =.8f;
	[Export]private float stopRadius, followSpeed;
	private bool followEnabled, declutterEnabled, knockBackActive, secondaryKnockbackActive, stunActive, inMeleeRange, movingUp;
	private Label debugLabel;
	public override void _Ready()
	{
		myPlayer = GetNode<BasicControllerComponent>("../../../2DTestChar/BasicControllerComponent");
		debugLabel = GetNode<Label>("../DebugLabel");
		followEnabled = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		FollowPlayer(delta);

		unitBody.Velocity = myVelocity;
		unitBody.MoveAndCollide(myVelocity);

	}

	private void FollowPlayer(double delta){

		direction = myPlayer.GlobalPosition - GlobalPosition;
		distance = direction.Length();

		if (followEnabled)
		{
			if (distance > stopRadius)
			{
				direction = direction.Normalized();
				myVelocity = direction * .01f * followSpeed;
				inMeleeRange = false;
				EmitSignal("insideMeleeRange", inMeleeRange, direction);
			}
			else
			{
				inMeleeRange = true;
				EmitSignal("insideMeleeRange", inMeleeRange, direction);
				myVelocity = Vector2.Zero;
			}
		}
		if(declutterEnabled){

			if(otherEnemyBody !=null){

				
				secondaryDirection = secondaryDirection.Normalized();
				myVelocity = -secondaryDirection * .005f * followSpeed;
			}
			
		}

		if (distance > stopRadius)
		{
			direction = direction.Normalized();
			inMeleeRange = false;
			EmitSignal("insideMeleeRange", inMeleeRange, direction);
		}


		if (direction.Y < 0 && !movingUp){
			movingUp = true;
			EmitSignal("movementDirectionChanged", movingUp);
		}else if (direction.Y >=0 && movingUp){
			movingUp = false;
			EmitSignal("movementDirectionChanged", movingUp);
		}
		if(stunActive && !knockBackActive && !secondaryKnockbackActive){
			myVelocity = Vector2.Zero;
		}

		if(knockBackActive){
			
			myVelocity = knockBackModifier;
			knockBackModifier *= knockBackFriction;

		}
		if(secondaryKnockbackActive){

			myVelocity = secondaryKnockBackModifier;
			secondaryKnockBackModifier *= knockBackFriction;

		}
		

		if(knockBackActive || stunActive || secondaryKnockbackActive){
			followEnabled = false;
		}else{
			followEnabled = true;
		}

	}

	public void ActivateSecondaryKnockback(Vector2 knockbackDirecton, int knockBackValue, int stunValue){

		secondaryKnockbackActive = true;
		stunActive = true;

		knockbackTimer.Start();
		stunTimer.Start();

		secondaryKnockBackModifier =  knockbackDirecton * knockBackValue;
		stunTimer.WaitTime = stunValue;	
	}

	private void OnUnitKnockback(Vector2 knockbackDirecton, int knockBackValue, int stunValue){

		knockBackActive = true;
		stunActive = true;

		knockbackTimer.Start();
		stunTimer.Start();

		knockBackModifier = knockbackDirecton * knockBackValue;
		stunTimer.WaitTime = stunValue;	

	}

	private void OnKnockbackTimerTimeout()
	{
		if(knockBackActive){
			knockBackActive = false;
		}
		if(secondaryKnockbackActive){
			secondaryKnockbackActive = false;
		}
	}


	private void OnStunTimerTimeout()
	{
		stunActive = false;

	}

	private void OnEnemyCollide(Vector2 enemyPosition){
		if(knockBackActive){
			return;
		}else{

			secondaryKnockbackActive = true;
			knockbackTimer.Start();
			secondaryKnockBackModifier = enemyPosition * secondaryKnockBackWeight;
		}	
	}

	private void OnSpawningSicknessInitiated(double spawnFreeze)
	{
		stunActive = true;
		CallDeferred("DeferredStunTimerStart", spawnFreeze);
	}

	private void DeferredStunTimerStart(double duration){
			stunTimer.WaitTime = duration;
			stunTimer.Start();
	}

	private void OnOtherEnemyDetectorEntered(Node2D body){

		if(body.IsInGroup("Enemy")){

			int fearCoeff = GD.RandRange(0,10);
			if(fearCoeff < 5){
				otherEnemyBody = body.GetNode<CharacterBody2D>(".");
				declutterEnabled = true;
				secondaryDirection = otherEnemyBody.GlobalPosition - GlobalPosition;
				movementChangeTimer.Start();
			}
			
		}	
	}

	private void OnMovementChangeTimerTimeout(){
		declutterEnabled = false;
	}

}









