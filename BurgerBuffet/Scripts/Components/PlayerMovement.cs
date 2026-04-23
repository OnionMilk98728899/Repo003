using Godot;
using System;

public partial class PlayerMovement : Node2D
{
	[Export] private CharacterBody2D _playerBody;
	[Export] private Node2D _dustNode;
	[Export] private Sprite2D _playerSprite, _shadowSprite, _dizzySprite;
	[Export] private AnimationPlayer _playerAnim, _dustAnim, _dizzyAnim;
	[Export] private ItemCollector _itemCollector;
	[Export] private int _playerSpeed;
	[Export] private Label _debugLabel;
	[Export] private Timer _jumpTimer,_landTimer, _knockoutTimer, _jumpCooldownTimer, _moveCooldownTimer, _knockoutCooldownTimer, _dizzyCooldownTimer;
	private Vector2 _inputDirection, _playerVelocity, _startPosition, _playerPosition;
	private bool slowed, stopped, _isJumping, _isKnockedOut, _isRecoveryFrame ;
	private string _directionPath, _movementPath;
	private int _dizzyCounter;
	private enum direction
	{
		left, right, up, down
	}
	private direction _currentDirection, _nextDirection;
	public override void _Ready()
	{
		ResetPosition();
		GlobalSignals.Instance.RestartGame += ResetPosition;
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
		//_debugLabel.Text = _currentDirection.ToString();
	}

	private void ResetPosition()
	{
		_inputDirection.X = 1;
		_inputDirection.Y = 0;
		_currentDirection = direction.right;
		_startPosition = new Vector2(248, 150);
		_playerBody.GlobalPosition = _startPosition;
	}

	private void ManualSlowdown(double delta)
	{
		if (Input.IsActionJustPressed("Q"))
		{
			if (!slowed)
			{
				Engine.TimeScale = 4; slowed = true;
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

			if (_moveCooldownTimer.IsStopped() && !_isJumping && !_isRecoveryFrame && !_isKnockedOut && _knockoutCooldownTimer.IsStopped())
			{
				if (Input.IsActionPressed("ui_left"))
				{
					if (_currentDirection == direction.right ) { 
						_dizzyCounter +=1; 
					}
					_currentDirection = direction.left;
					_moveCooldownTimer.Start();
				}
				else if (Input.IsActionPressed("ui_right"))
				{
					if (_currentDirection == direction.left ) { 
						_dizzyCounter +=1; 
						}
					_currentDirection = direction.right; 
					_moveCooldownTimer.Start();
				}
				else if (Input.IsActionPressed("ui_up"))
				{
					if (_currentDirection == direction.down ) {
						 _dizzyCounter +=1; 
						 }
					_currentDirection = direction.up;
					_moveCooldownTimer.Start();
				}
				else if (Input.IsActionPressed("ui_down"))
				{
					if (_currentDirection == direction.up ) {
						 _dizzyCounter +=1; 
						 }
					_currentDirection = direction.down;
					_moveCooldownTimer.Start();
				}
			}

			if(_dizzyCounter == 1 && _dizzyCooldownTimer.IsStopped())
			{
				_dizzyCooldownTimer.Start();
			}
			if(_dizzyCounter == 2)
			{
				_dizzyAnim.Play("Dizzy");
				
			}
			if(_dizzyCounter == 3)
			{
				_isKnockedOut = true;
				_dizzyAnim.Play("KO");
				
			}

			if (Mathf.Round(_playerPosition.X - 8) % 16 == 0 && MathF.Round(_playerPosition.Y - 6) % 16 == 0 || _isRecoveryFrame)
			{
				if (_currentDirection == direction.left) { _inputDirection.X = -1; _inputDirection.Y = 0; }
				if (_currentDirection == direction.right) { _inputDirection.X = 1; _inputDirection.Y = 0; }
				if (_currentDirection == direction.up) { _inputDirection.Y = -1; _inputDirection.X = 0; }
				if (_currentDirection == direction.down) { _inputDirection.Y = 1; _inputDirection.X = 0; }
				if (_isRecoveryFrame) { _isRecoveryFrame = false; }
			}


			if (Input.IsActionPressed("Z") && !_isJumping && _jumpCooldownTimer.IsStopped())
			{
				_isJumping = true;
				_jumpTimer.Start();
				_landTimer.Start();
				_itemCollector.DisableEnableCollider(false);
				AudioManager.Instance.PlaySFX(AudioManager.Instance._sfx2Player,AudioManager.Instance._audioLibrary.jump);
			}

			if (!_isKnockedOut)
			{
				_playerVelocity.X = _inputDirection.X * _playerSpeed;
				_playerVelocity.Y = _inputDirection.Y * _playerSpeed;
				_playerBody.Velocity = _playerVelocity;
				_playerBody.MoveAndSlide();
			}
			if (_isKnockedOut && _knockoutTimer.IsStopped())
			{
				_playerVelocity = Vector2.Zero;
				_knockoutTimer.Start();
			}
		}
	}

	private void PlaySoundFX()
	{
		if (Input.IsActionJustPressed("ui_right") || Input.IsActionJustPressed("ui_left") || Input.IsActionJustPressed("ui_up") || Input.IsActionJustPressed("ui_down"))
		{
			AudioManager.Instance.PlaySFX(AudioManager.Instance._sfx2Player, AudioManager.Instance._audioLibrary.turn);
		}
	}

	public void PlayDizzySFX()
	{
		AudioManager.Instance.PlaySFX(AudioManager.Instance._playerDizzyFX, AudioManager.Instance._audioLibrary.dizzy);
	}

	public void PlayKnockoutSFX()
	{
		AudioManager.Instance.PlaySFX(AudioManager.Instance._playerDizzyFX, AudioManager.Instance._audioLibrary.knockout);
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
			_shadowSprite.Frame = 6;
		}

	}

	private void OnItemCollectorPlayerKnockout()
	{
		_isKnockedOut = true;
		if (_currentDirection == direction.right) { _isKnockedOut = true; _currentDirection = direction.left; }
		else if (_currentDirection == direction.left) { _isKnockedOut = true; _currentDirection = direction.right; }
		else if (_currentDirection == direction.down) { _isKnockedOut = true; _currentDirection = direction.up; }
		else if (_currentDirection == direction.up) { _isKnockedOut = true; _currentDirection = direction.down; }
	}

	private void OnJumpTimerTimeout()
	{
		_isJumping = false;
		_dustAnim.Play("Dust");
		_dustNode.GlobalPosition = _playerBody.GlobalPosition;
		_jumpCooldownTimer.Start();

	}
	private void OnLandTimerTimeout()
	{
		_itemCollector.DisableEnableCollider(true);
	}

	private void OnKOTimerTimeout()
	{
		_isKnockedOut = false;
		_isRecoveryFrame = true;
		_knockoutCooldownTimer.Start();

		_dizzyCounter = 0;
		_dizzyAnim.Stop();
		_dizzySprite.Frame = 15;

	}
	private void OnDizzyCooldownTimerTimeout()
	{
		_dizzyCounter = 0;
		_dizzyAnim.Stop();
		_dizzySprite.Frame = 15;
	}


}
