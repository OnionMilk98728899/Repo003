using Godot;
using System;

public partial class PlayerMovement : Node2D
{
	[Export] private CharacterBody2D _playerBody;
	[Export] private Node2D _dustNode;
	[Export] private Sprite2D _playerSprite;
	[Export] private AnimationPlayer _playerAnim, _dustAnim;
	[Export] private ItemCollector _itemCollector;
	[Export] private int _playerSpeed;
	[Export] private Label _debugLabel;
	[Export] private Timer _jumpTimer, _knockoutTimer;
	private Vector2 _inputDirection, _playerVelocity, _startPosition, _playerPosition;
	private bool slowed, stopped, _isJumping, _isKnockedOut;
	private string _directionPath, _movementPath;
	private enum direction
	{
		left, right, up, down
	}
	private direction _currentDirection;
	public override void _Ready()
	{
		ResetPosition();
		GlobalSignals.Instance.RestartGame += ResetPosition;
	}

	private void ResetPosition()
	{
		_inputDirection.X = 1;
		_inputDirection.Y = 0;
		_currentDirection = direction.right;
		_startPosition = new Vector2(248, 150);
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
			PlaySoundFX();
		}

		//_debugLabel.Text = MathF.Round(_playerPosition.X) + ", " + MathF.Round(_playerPosition.Y);

	}

	private void ManualSlowdown(double delta)
	{
		if (Input.IsActionJustPressed("Q"))
		{
			if (!slowed)
			{
				Engine.TimeScale = 3; slowed = true;
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
		if (GlobalResources.Instance._currentGameState != GlobalResources.gameState.gameOver)
		{
			if (Input.IsActionPressed("ui_left"))
			{
				if (_currentDirection == direction.right) { _isKnockedOut = true; }
				else{_currentDirection = direction.left;}		
			}
			if (Input.IsActionPressed("ui_right"))
			{
				if (_currentDirection == direction.left) { _isKnockedOut = true; }
				else{_currentDirection = direction.right;}
			}
			if (Input.IsActionPressed("ui_up"))
			{
				if (_currentDirection == direction.down) { _isKnockedOut = true; }
				else{_currentDirection = direction.up;}	
			}
			if (Input.IsActionPressed("ui_down"))
			{
				if (_currentDirection == direction.up) { _isKnockedOut = true; }
				else{_currentDirection = direction.down;}	
			}

			if (Mathf.Round(_playerPosition.X - 8) % 16 == 0 && MathF.Round(_playerPosition.Y - 6) % 16 == 0)
			{
				if (_currentDirection == direction.left) { _inputDirection.X = -1; _inputDirection.Y = 0; }
				if (_currentDirection == direction.right) { _inputDirection.X = 1; _inputDirection.Y = 0; }
				if (_currentDirection == direction.up) { _inputDirection.Y = -1; _inputDirection.X = 0; }
				if (_currentDirection == direction.down) { _inputDirection.Y = 1; _inputDirection.X = 0; }
				
			}

			if (Input.IsActionPressed("Z") && !_isJumping)
			{
				_isJumping = true;
				_jumpTimer.Start();
				_itemCollector.DisableEnableCollider(false);
				AudioManager.Instance.PlaySFX2(AudioManager.Instance._audioLibrary.jump);
			}

			if (!_isKnockedOut)
			{
				_playerVelocity.X = _inputDirection.X * _playerSpeed;
				_playerVelocity.Y = _inputDirection.Y * _playerSpeed;
				_playerBody.Velocity = _playerVelocity;
				_playerBody.MoveAndSlide();
			}
			if(_isKnockedOut && _knockoutTimer.IsStopped())
			{
				_knockoutTimer.Start();
			}

		}

	}

	private void PlaySoundFX()
	{
		if(Input.IsActionJustPressed("ui_right") ||Input.IsActionJustPressed("ui_left") || Input.IsActionJustPressed("ui_up") || Input.IsActionJustPressed("ui_down"))
		{
			AudioManager.Instance.PlaySFX2(AudioManager.Instance._audioLibrary.turn);
		}
	}

	private void AnimatePlayer()
	{
		if (GlobalResources.Instance._currentGameState == GlobalResources.gameState.normal)
		{
			if (_isJumping)
			{
				_movementPath = "Jump_";
			}
			else if (_isKnockedOut)
			{
				_movementPath = "KO_";
			}
			else
			{
				_movementPath = "Move_";
			}
		}
		if (GlobalResources.Instance._currentGameState == GlobalResources.gameState.special)
		{
			if (_isJumping)
			{
				_movementPath = "Special_Jump_";
			}
			else
			{
				_movementPath = "Special_";
			}
		}

		if (GlobalResources.Instance._currentGameState != GlobalResources.gameState.gameOver)
		{
			if (_inputDirection.X == -1 && _inputDirection.Y == 0) { _directionPath = "Left"; }
			if (_inputDirection.X == 1 && _inputDirection.Y == 0) { _directionPath = "Right"; }
			if (_inputDirection.Y == -1 && _inputDirection.X == 0) { _directionPath = "Up"; }
			if (_inputDirection.Y == 1 && _inputDirection.X == 0) { _directionPath = "Down"; }

			_playerAnim.Play(_movementPath + _directionPath);
		}
		else if (GlobalResources.Instance._currentGameState == GlobalResources.gameState.gameOver)
		{
			_playerAnim.Play("GameOver");
		}

	}

	private void OnJumpTimerTimeout()
	{
		_isJumping = false;
		_itemCollector.DisableEnableCollider(true);
		_dustAnim.Play("Dust");
		_dustNode.GlobalPosition = _playerBody.GlobalPosition;

	}

	private void OnKOTimerTimeout()
	{
		_isKnockedOut = false;
	}

}
