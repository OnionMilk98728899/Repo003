using Godot;
using System;

public partial class BasicCombatComponent : Node2D
{
	private bool inMeleeRange, attackTimerRunning;
	[Signal] public delegate void initiateMeleeAttackEventHandler(bool attacking);
	[Export] private AnimationPlayer attackAnim;
	[Export] private Timer attackTimer;
	[Export] private Sprite2D handSprite;
	[Export] private int meleeDamage;
	[Export] private CollisionShape2D leftRightPunchBox, upDownPunchBox;
	private Vector2 directionToPlayer;
	private float attackRadians, attackDegrees;
	private bool attackActive;


	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		MeleeAttack();
	}

	private void MeleeAttack(){
		if(inMeleeRange){
			attackActive = true;
			EmitSignal("initiateMeleeAttack", attackActive);
			attackTimer.Start();
			attackTimerRunning = true;
			//attackAnim.Play("Swipe");
		}
		if(!inMeleeRange && !attackTimerRunning){
			attackAnim.Stop();
			attackActive = false;
			EmitSignal("initiateMeleeAttack", attackActive);
			if(!leftRightPunchBox.Disabled){
				leftRightPunchBox.Disabled = true;
			}
			if(!upDownPunchBox.Disabled){
				upDownPunchBox.Disabled = true;
			}
		}
	}

	private void OnMeleeRangeEntered(bool range, Vector2 playerDirection){

		directionToPlayer = playerDirection;
		inMeleeRange = range;

		if(range){
			DetermineAttackDirection();
		}else{
			
			if(!attackAnim.IsPlaying()){
				handSprite.Visible = false;
			}
			
		}
		
	}

	private void DetermineAttackDirection(){

		attackRadians = Mathf.Atan2(directionToPlayer.Y, directionToPlayer.X);
		attackDegrees = Mathf.RadToDeg(attackRadians);	

		if(attackDegrees >= -45 && attackDegrees < 45){     ///Right Facing Position

			attackAnim.Play("Swipe_Right");

		}else if(attackDegrees >= 45 && attackDegrees < 135){   ///Downward Facing Position

			attackAnim.Play("Swipe_Down");
		}else if(attackDegrees >= 135 || attackDegrees <-135){     /// Left Facing Position

			attackAnim.Play("Swipe_Left");

		}else if(attackDegrees >= -135 && attackDegrees <-45){    ///Upward Facing Position
			attackAnim.Play("Swipe_Up");
		}


	}

	private void OnAttackTimerTimeout(){
		attackTimerRunning = false;
	}
}
