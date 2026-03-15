using Godot;
using System;

public partial class BasicMovement2D : CharacterBody2D
{
	[Export] float speed, acceleration, deceleration;
	private Vector2 myVelocity, direction;
	private float angleRadians, angleDegrees;
	private Node2D rotatingDeck;

	private CharacterBody2D leftHand, rightHand;
	private Sprite2D leftHandSprite, rightHandSprite;
	private Sprite2D myLogo;
	private Camera2D myCamera;
	private AnimationPlayer punchAnim;
	private bool rotateGloves;
	private Label debugLabel;
	public override void _Ready()
	{
		leftHand = GetNode<CharacterBody2D>("RotatingDeck/LeftHand");
		rightHand = GetNode<CharacterBody2D>("RotatingDeck/RightHand");
		leftHandSprite = GetNode<Sprite2D>("RotatingDeck/LeftHand/LeftHandSprite");
		rightHandSprite = GetNode<Sprite2D>("RotatingDeck/RightHand/RightHandSprite");
		debugLabel = GetNode<Label>("RotatingDeck/LeftHand/LeftLabel");
		myLogo = GetNode<Sprite2D>("Logo");
		myCamera = GetNode<Camera2D>("../2DCam");
		punchAnim = GetNode<AnimationPlayer>("RotatingDeck/PunchAnim");
		rotatingDeck = GetNode<Node2D>("RotatingDeck");
	}

	public override void _PhysicsProcess(double delta)
	{

		Vector2 inputDirection = GetInputDirection();
		inputDirection = inputDirection.Normalized();

		if (inputDirection.Length() > 0)
		{
			// Accelerate towards input direction
			myVelocity = myVelocity.Lerp(inputDirection * speed, (float)delta * acceleration);
		}
		else
		{
			// Decelerate when no input
			myVelocity = myVelocity.Lerp(Vector2.Zero, (float)delta * deceleration);
		}

		Velocity = myVelocity;
		GetMouseToPlayerAngle();
		ThrowPunches();
		MoveAndSlide();

	}

	private Vector2 GetInputDirection(){

		Vector2 direction = Vector2.Zero;

		// Read input and set direction
		if (Input.IsActionPressed("move_forward")) direction.Y -= 1;
		//GD.Print("Move Forward");
		if (Input.IsActionPressed("move_backward")) direction.Y += 1;
		//GD.Print("Move Backward");
		if (Input.IsActionPressed("move_left")) direction.X -= 1;
		//GD.Print("Move Left");
		if (Input.IsActionPressed("move_right")) direction.X += 1;
		//GD.Print("Move Right");
		
		return direction;
	}

	private void GetMouseToPlayerAngle(){

		Vector2 mousePosition = myCamera.GetGlobalMousePosition();

		direction = mousePosition - Position;
		// Find the angle in radians
		angleRadians = Mathf.Atan2(direction.Y, direction.X);
		angleDegrees = Mathf.RadToDeg(angleRadians);

		//GD.Print("angleDegrees "+ angleDegrees);

		if(angleDegrees >=-22.5 && angleDegrees <22.5){     ///Right Facing Position
			rotatingDeck.RotationDegrees = 0;
			rotateGloves = false;
		}else if(angleDegrees >=-67.5 && angleDegrees <-22.5){   ///Right-up Facing Position
		   rotatingDeck.RotationDegrees = -45;
		   rotateGloves = true;
		}else if(angleDegrees >=-112.5 && angleDegrees <-67.5){     ///Upward Facing Position
			rotatingDeck.RotationDegrees = -90;
			rotateGloves = false;
		}else if(angleDegrees >= -157.5 && angleDegrees <-112.5){    ///Left-up Facing Position
			rotatingDeck.RotationDegrees = -135;
			rotateGloves = true;
		}else if(angleDegrees >=157.5 || angleDegrees <-157.5){     ///Left Facing Position
			rotatingDeck.RotationDegrees = 180;
			rotateGloves = false;
		}else if(angleDegrees >=112.5 && angleDegrees <157.5){   ///Left-down Facing Position
		   rotatingDeck.RotationDegrees = 135;
		   rotateGloves = true;
		}else if(angleDegrees >=67.5 && angleDegrees <112.5){     ///Downward Facing Position
			rotatingDeck.RotationDegrees = 90;
			rotateGloves = false;
		}else if(angleDegrees >= 22.5 && angleDegrees <67.5){    ///Right-down Facing Position
			rotatingDeck.RotationDegrees = 45;
			rotateGloves = true;
		}

		if(rotateGloves){
			leftHandSprite.RotationDegrees = 45;
			rightHandSprite.RotationDegrees = 45;
		}else{
			leftHandSprite.RotationDegrees = 0;
			rightHandSprite.RotationDegrees = 0;
		}
		//GD.Print("Deck Rotation is "+ rotatingDeck.Rot)
	}

	private void ThrowPunches(){
		if (Input.IsActionPressed("mouse_click_left")){

			if(Input.IsActionPressed("ui_focus_next")){
				punchAnim.Play("Left_Hook");
			}else{
				punchAnim.Play("Left_Jab");
			}
			
		}
		if (Input.IsActionPressed("mouse_click_right")){
			punchAnim.Play("Right_Jab");
		}

		debugLabel.Text = leftHand.Velocity.ToString();
	}

	
}
