using Godot;
using System;

public partial class PunchActionComponent : Node2D
{
	[Export] private Sprite2D headSprite;
	[Export] private Texture2D[] gloveSheets;
	[Export] private Timer punchTimer;
	private float angleRadians, angleDegrees;
	//private Node2D rotatingDeck;
	private Vector2 mouseDirection;
	private CharacterBody2D leftHand, rightHand;
	private Sprite2D leftHandSprite, rightHandSprite;
	private Camera2D myCamera;
	private AnimationPlayer punchAnim;
	[Export] public int punchDamage = 50, knockback = 3, stunFactor = 3;
	private int comboCounter;
	private double buttonHoldCounter;
	private bool rotateGloves, leftButtonHeld, rightButtonHeld, isHurt;


	public override void _Ready()
	{
		leftHand = GetNode<CharacterBody2D>("LeftHand");
		rightHand = GetNode<CharacterBody2D>("RightHand");
		leftHandSprite = GetNode<Sprite2D>("LeftHand/LeftHandSprite");
		rightHandSprite = GetNode<Sprite2D>("RightHand/RightHandSprite");
		myCamera = GetNode<Camera2D>("../../2DCam");
		punchAnim = GetNode<AnimationPlayer>("PunchAnim");
		//rotatingDeck = GetNode<Node2D>("RotatingDeck");

	}

	public override void _PhysicsProcess(double delta)
	{
		if(!punchAnim.IsPlaying()){
			punchAnim.Play("RESET");
		}
		GetMouseToPlayerAngleII();
		GetTexturesBasedOnAngle();
		ThrowPunches();
	}

	private void GetMouseToPlayerAngleII(){

		Vector2 mousePosition = myCamera.GetGlobalMousePosition();

		mouseDirection = mousePosition - GlobalPosition;
		// Find the angle in radians
		angleRadians = Mathf.Atan2(mouseDirection.Y, mouseDirection.X);
		angleDegrees = Mathf.RadToDeg(angleRadians);	

		RotationDegrees = angleDegrees;
		leftHandSprite.RotationDegrees = -angleDegrees;
		rightHandSprite.RotationDegrees = -angleDegrees;
	}

	private void GetTexturesBasedOnAngle(){

		if(angleDegrees >=-22.5 && angleDegrees <22.5){     ///Right Facing Position
			leftHandSprite.Texture = gloveSheets[4];
			rightHandSprite.Texture = gloveSheets[14];

		}else if(angleDegrees >=-67.5 && angleDegrees <-22.5){   ///Right-up Facing Position
			leftHandSprite.Texture = gloveSheets[6];
			rightHandSprite.Texture = gloveSheets[13];

		}else if(angleDegrees >=-112.5 && angleDegrees <-67.5){     ///Upward Facing Position
			leftHandSprite.Texture = gloveSheets[7];
			rightHandSprite.Texture = gloveSheets[12];

		}else if(angleDegrees >= -157.5 && angleDegrees <-112.5){    ///Left-up Facing Position
			leftHandSprite.Texture = gloveSheets[3];
			rightHandSprite.Texture = gloveSheets[11];

		}else if(angleDegrees >=157.5 || angleDegrees <-157.5){     ///Left Facing Position
			leftHandSprite.Texture = gloveSheets[2];
			rightHandSprite.Texture = gloveSheets[10];

		}else if(angleDegrees >=112.5 && angleDegrees <157.5){   ///Left-down Facing Position
			leftHandSprite.Texture = gloveSheets[1];
			rightHandSprite.Texture = gloveSheets[9];

		}else if(angleDegrees >=67.5 && angleDegrees <112.5){     ///Downward Facing Position
			leftHandSprite.Texture = gloveSheets[0];
			rightHandSprite.Texture = gloveSheets[8];

		}else if(angleDegrees >= 22.5 && angleDegrees <67.5){    ///Right-down Facing Position
			leftHandSprite.Texture = gloveSheets[5];
			rightHandSprite.Texture = gloveSheets[15];

		}


		if(angleDegrees >= -45 && angleDegrees < 45){
			headSprite.Frame = 1;
			if(isHurt){
				headSprite.Frame = 5;
			}
		}else if(angleDegrees >= 45 && angleDegrees < 135){
			headSprite.Frame = 0;
			if(isHurt){
				headSprite.Frame = 4;
			}
		}else if(angleDegrees >= 135 || angleDegrees < -135){
			headSprite.Frame = 3;
			if(isHurt){
				headSprite.Frame = 7;
			}
		}else if (angleDegrees >= -135 && angleDegrees < -45){
			headSprite.Frame = 2;
			if(isHurt){
				headSprite.Frame = 6;
			}
		}

	}

	private void ThrowPunches(){


		if (Input.IsActionJustPressed("mouse_click_left") && !Input.IsActionPressed("mouse_click_right")){

			switch(comboCounter){
				case 0:
					punchAnim.Play("Left_Jab");
					punchTimer.Start();
				break;
				case 1:
					punchAnim.Play("Right_Jab");
					punchTimer.Start();

				break;
				case 2:
					punchAnim.Play("Left_Hook");
					punchTimer.Start();
				break;
				case 3:
					punchAnim.Play("Right_Uppercut");
					punchTimer.Start();
				break;
			}
			comboCounter++;		
		}

		if(Input.IsActionPressed("mouse_click_left") && !Input.IsActionPressed("mouse_click_right")){
			buttonHoldCounter++;
			if(buttonHoldCounter > 25){
				punchAnim.Play("Left_Charge");
			}
			

		}
		if(Input.IsActionJustReleased("mouse_click_left")){
			if(buttonHoldCounter > 25){
				punchAnim.Play("Left_PowerPunch");
			}
			buttonHoldCounter = 0;
		}

		else if (Input.IsActionPressed("mouse_click_right") && !Input.IsActionPressed("mouse_click_left")){
			punchAnim.Play("Block");

		}

	}

	

	private void OnPunchTimerTimeout(){

		comboCounter = 0;

	}
	private void OnButtonHoldTimerTimeout(){

	}
	private void OnPlayerHurt(bool hurt){
		isHurt = hurt;
	}
}
