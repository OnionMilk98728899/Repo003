using Godot;
using System;

public partial class PlayerMovement : Node2D
{
	[Export] private CharacterBody2D _playerBody;
	[Export] private Sprite2D _playerSprite;
	[Export] private AnimationPlayer _playerAnim;
	[Export] private int _playerSpeed;
	[Export] private Label _debugLabel;
	private Vector2 _inputDirection, _playerVelocity, _startPosition, _playerPosition;
	private bool slowed, stopped;
	private enum direction
	{
		left, right, up, down
	}
	private direction _currentDirection;
	public override void _Ready()
	{
		ResetPosition();
		GlobalSignals.Instance.GameOver += ResetPosition;
	}

	private void ResetPosition()
	{
		_inputDirection.X = 1;
		_inputDirection.Y = 0;
		_currentDirection = direction.right;
		_startPosition = new Vector2(248 , 150);
		_playerBody.GlobalPosition = _startPosition;
	}


	public override void _PhysicsProcess(double delta)
	{
		_playerPosition = _playerBody.GlobalPosition;
		
		AnimatePlayer();
		ManualSlowdown(delta);
		if (!stopped)
		{
			MovePlayer(delta);
		}
		//_debugLabel.Text = MathF.Round(_playerPosition.X) + ", " + MathF.Round(_playerPosition.Y);
		
	}

	private void ManualSlowdown(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			if (!slowed)
			{
				Engine.TimeScale = .4; slowed = true;
			}
			else
			{
				Engine.TimeScale = 1; slowed = false;
			}
		}
		if (Input.IsActionJustPressed("ui_focus_next"))
		{
			if (!stopped)
			{
				stopped = true;
			}
			else
			{
				stopped = false;
			}
		}
	}

	private void MovePlayer(double delta)
	{
		if (Input.IsActionPressed("ui_left")) { _currentDirection = direction.left; }
		if (Input.IsActionPressed("ui_right")) { _currentDirection = direction.right; }
		if (Input.IsActionPressed("ui_up")) { _currentDirection = direction.up; }
		if (Input.IsActionPressed("ui_down")) { _currentDirection = direction.down; }

		if (Mathf.Round(_playerPosition.X - 8) % 16 == 0 && MathF.Round(_playerPosition.Y - 6) % 16 == 0)
		{
			if (_currentDirection == direction.left) { _inputDirection.X = -1; _inputDirection.Y = 0; }
			if (_currentDirection == direction.right) { _inputDirection.X = 1; _inputDirection.Y = 0; }
			if (_currentDirection == direction.up) { _inputDirection.Y = -1; _inputDirection.X = 0; }
			if (_currentDirection == direction.down) { _inputDirection.Y = 1; _inputDirection.X = 0; }
		}

		_playerVelocity.X = _inputDirection.X * _playerSpeed;
		_playerVelocity.Y = _inputDirection.Y * _playerSpeed;
		_playerBody.Velocity = _playerVelocity;
		_playerBody.MoveAndSlide();
	}

	private void AnimatePlayer()
	{
		if (_inputDirection.X == -1 && _inputDirection.Y == 0) { _playerAnim.Play("Move_Left"); }
		if (_inputDirection.X == 1 && _inputDirection.Y == 0) { _playerAnim.Play("Move_Right"); }
		if (_inputDirection.Y == -1 && _inputDirection.X == 0) { _playerAnim.Play("Move_Up"); }
		if (_inputDirection.Y == 1  &&_inputDirection.X == 0) { _playerAnim.Play("Move_Down"); }
	}

}
