using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class MainCamera : Camera2D
{
	[Export] private Camera2D _mainCam;
	[Export]float _cameraSpeed;
	[Export] private Vector2 _initialCamPos;
	private Vector2 _followTarget;

	public override void _Ready()
	{
		_followTarget = _initialCamPos;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (_followTarget != _mainCam.Position)
		{
			Vector2 target = new Vector2(_mainCam.Position.X,_followTarget.Y - 40f);

			_mainCam.Position = GlobalPosition.Lerp(target, (float)GetProcessDeltaTime() * _cameraSpeed);
		}   
	}
	private void OnGridModeCursorMoved(Vector2 position)
	{
		_followTarget = position;
	}

}
