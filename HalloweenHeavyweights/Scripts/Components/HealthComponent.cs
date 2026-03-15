using Godot;
using System;


public partial class HealthComponent : Node2D
{   
	[Signal] public delegate void killUnitEventHandler(); 
	[Signal] public delegate void knockUnitBackEventHandler(Vector2 knockbackDirection, int knockBackValue, int stunValue);
	[Signal] public delegate void hurtEnemyUnitEventHandler(bool hurt);
	[Signal] public delegate void hurtPlayerEventHandler(bool hurt);
	[Signal] public delegate void healthToGUIEventHandler(int damagepoints);
	[Export] AnimationPlayer bloodAnim;
	[Export] Sprite2D playerBodySprite;
	[Export] Sprite2D healthBox, healthBar;	
	[Export] private string bloodAnimLeft, bloodAnimRight, bloodAnimUp, bloodAnimDown;
	private string animationNodePath;
	[Export] private Timer hurtTimer;
	[Export] private Sprite2D enemyBodySprite, bloodSprite;
	[Export] private int maximumHealth;
	[Export] private bool hasKnockback;
	[Export] private int knockbackCoefficient; 
	[Export] private unitType myUnitType;
	private float opponentAngleRadians, opponentAngleDegrees;
	private int currentHealth;
	private bool isHurt;
	private Color red = new Color(1,0,0,1), white = new Color(1,1,1,1);

	private enum unitType {
		player, enemy, trap
	}

	public override void _Ready()
	{
		currentHealth = maximumHealth;
	}

	public void TakeDamage(int damagePoints, int knockBackPoints, int stunPoints, Vector2 opponentPosition){
		
		if(!isHurt){
			hurtTimer.Start();
		}
		isHurt = true;
		
		if(myUnitType == unitType.enemy){

			SubtractEnemyHealth(damagePoints);
			EmitSignal("hurtEnemyUnit", isHurt);

		}

		if(myUnitType == unitType.player){

			EmitSignal("hurtPlayer", isHurt);
			SubtractPlayerHealth(damagePoints);
		}


		Vector2 opponentDirection = (GlobalPosition - opponentPosition).Normalized();

		if(hasKnockback){
			
			int knockBackTransmitted = knockBackPoints* knockbackCoefficient;
			EmitSignal("knockUnitBack", opponentDirection, knockBackTransmitted, stunPoints);
		}

		if(myUnitType == unitType.enemy){

			EmitBlood(opponentDirection);
		}
		
	}

	private void ChangeColor(){
		hurtTimer.Start();
		enemyBodySprite.Modulate = red;
	}

	private void SubtractEnemyHealth(int damagePoints){
		currentHealth -= damagePoints;

		if(currentHealth <= 0){

			//EmitSignal("killUnit");
			GetParent().QueueFree();
		}else{	

			float xBarLength = (float)currentHealth/100;

			healthBar.Scale = new Vector2(xBarLength, 1);
	
		}
		
	}

	private void SubtractPlayerHealth(int damagePoints){

		EmitSignal("healthToGUI", damagePoints);

	}

	private void EmitBlood(Vector2 opponentDirection){

		opponentAngleRadians = Mathf.Atan2(opponentDirection.Y, opponentDirection.X);
		opponentAngleDegrees = Mathf.RadToDeg(opponentAngleRadians);

		if(opponentAngleDegrees <= 45 && opponentAngleDegrees > -45){
			animationNodePath = bloodAnimLeft;
		}else if(opponentAngleDegrees > 45 && opponentAngleDegrees <= 135){
			animationNodePath = bloodAnimUp;
		}else if(opponentAngleDegrees > 135 || opponentAngleDegrees <= -135){
			animationNodePath = bloodAnimRight;
		}else if(opponentAngleDegrees <= -45 && opponentAngleDegrees > -135){
			animationNodePath = bloodAnimDown;
		}

		bloodAnim.Play(animationNodePath);
	}

	private void ApplyKnockback(){

	}

	private void OnHurtTimerTimeout()
	{
		isHurt = false;
		if(myUnitType == unitType.enemy){
			EmitSignal("hurtEnemyUnit", isHurt);
		}
		if(myUnitType == unitType.player){
			EmitSignal("hurtPlayer", isHurt);
		}
		
	}
}
