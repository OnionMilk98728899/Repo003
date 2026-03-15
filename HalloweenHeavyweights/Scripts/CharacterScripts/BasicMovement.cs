using Godot;
using System;

public partial class BasicMovement : CharacterBody3D
{
	[Export] public float Speed = 5f; // Movement speed
	[Export] public float Acceleration = 10f; // Smooth acceleration
	[Export] public float Deceleration = 15f; // Smooth deceleration
	[Export] public float GravWeight = 20f; // Gravity

	private Vector3 _velocity = Vector3.Zero;
	//private Sprite3D debugPoint;
	[Export] private NodePath _cameraPath; // Assign the Camera3D node in the Inspector

	private Camera3D _camera;

	public override void _Ready()
	{
		// Get the Camera3D node
		_camera = GetNode<Camera3D>(_cameraPath);
		//debugPoint = GetNode<Sprite3D>("DebugPoint");

		if (_camera == null)
		{
			GD.PrintErr("Camera3D not assigned or found. Please set _cameraPath in the Inspector.");
		}else{
			GD.Print("No Error -- Basic Movement being called");
		}

		
	}

	public override void _PhysicsProcess(double delta)
	{
		// Get input direction
		Vector3 inputDirection = GetInputDirection();
		inputDirection = inputDirection.Normalized();

		//FacePosition(CastRayFromCharToMouse());

		// Smoothly adjust velocity
		if (inputDirection.Length() > 0)
		{
			// Accelerate towards input direction
			_velocity = _velocity.Lerp(inputDirection * Speed, (float)delta * Acceleration);
		}
		else
		{
			// Decelerate when no input
			_velocity = _velocity.Lerp(Vector3.Zero, (float)delta * Deceleration);
		}

		Velocity = _velocity;
		MoveAndSlide();  
		ApplyGravity(delta);
		LookAtMouse();
		
		GD.Print("Velocity: "+ Velocity + "  ");
	}

	private Vector3 GetInputDirection()
	{
		Vector3 direction = Vector3.Zero;

		// Read input and set direction
		if (Input.IsActionPressed("move_forward")) direction.Z -= 1;
		//GD.Print("Move Forward");
		if (Input.IsActionPressed("move_backward")) direction.Z += 1;
		//GD.Print("Move Backward");
		if (Input.IsActionPressed("move_left")) direction.X -= 1;
		//GD.Print("Move Left");
		if (Input.IsActionPressed("move_right")) direction.X += 1;
		//GD.Print("Move Right");
		
		return direction;
	}

	private void LookAtMouse(){

		Plane targetPlaneMouse = new Plane(new Vector3(0,1,0), Position.Y);
		float rayLength = 1000;
		Vector2 mousePosition = GetViewport().GetMousePosition();
		Vector3 from = _camera.ProjectRayOrigin(mousePosition);
		Vector3 to = from + _camera.ProjectRayNormal(mousePosition)  * rayLength;

		Vector3? nullableVector =  targetPlaneMouse.IntersectsRay(from, to);
		Vector3 curserPositionOnPlane = nullableVector ?? new Godot.Vector3();

		LookAt(curserPositionOnPlane);
		
	}

	private void ApplyGravity(double delta){

		if(!IsOnFloor()){
			_velocity.Y -=  (float)delta * GravWeight;
		}else{
			_velocity.Y = 0;
		}

		if(_velocity.Y > 1){
			_velocity.Y = 0;
		}
		
	}




}
