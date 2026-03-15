using Godot;
using System;
using System.Data;

public partial class PlayerMovement : Node2D
{
	[Signal] public delegate void ActivatePauseMenuEventHandler(bool _isPaused);
	[Export] private WeaponManager _myWeaponMan;
	[Export] CharacterBody2D _playerBody;
	[Export] private Sprite2D _playerSprite;
	[Export] private AnimationPlayer _playerAnim;
	[Export] private Timer _jumpTimer, _attackTimer, _landTimer, _knockoutTimer;
	[Export] private CollisionShape2D _lowerLadderColl;
	[Export] private Label debugLabel;
	[Export] private int _playerSpeed, _playerAcceleration, _maxRunSpeed, _jumpPower, _gravity, _climbSpeed, _maxClimbSpeed;
	private Vector2 _playerVelocity, _inputDirection, _tempPosition;
	private string _currentAnim;
	private bool _isJumpActionPressed, _isEnterPressed, _isMovementActive, _isJumping, _isAttacking, _isGravityReset, _isTouchingLadder, _isInClimbMode,
	_isFloorDetectionDisabled, _isKnockedOut, _isRecovering;

	private enum _state
	{
		idle, run, jump, fall, land, climb, climbidle, knockout
	}
	private enum _stateMod
	{
		attack_, recover_, none
	}
	private _stateMod _currentMod;

	private _state _currentState;

	public override void _Ready()
	{
		GlobalSignals.Instance.EnterExitBuildMode += ActivatePlayerMovement;
		//AssignMovementBooleans();
		_isMovementActive = false;
		//Engine.TimeScale = .15f;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isMovementActive)
		{
			HandleDirectionalInput(delta);
			HandleClimbModeInput(delta);
			HandleJumpInput();
			HandleAttackInput();
			HandleDamageInput();
			DetermineCurrentAnimation();
			// _playerAnim.Play(_currentAnim);
			TemporarilyDisableFloorDetection();
			_playerBody.Velocity = _playerVelocity;
			_playerBody.MoveAndSlide();
			debugLabel.Text = (MathF.Round(_playerBody.GlobalPosition.Y)).ToString();
		}

	}

	private void ActivatePlayerMovement(bool isStoppingMovement)
	{

		_isMovementActive = !isStoppingMovement;
	}

	private void HandleDirectionalInput(double delta)
	{
		if (Input.IsActionPressed("ui_left")) _inputDirection.X -= 1;
		if (Input.IsActionPressed("ui_right")) _inputDirection.X += 1;
		if (Input.IsActionPressed("ui_up")) _inputDirection.Y -= 1;
		if (Input.IsActionPressed("ui_down")) _inputDirection.Y += 1;
		if (Input.IsActionJustReleased("ui_left") || Input.IsActionJustReleased("ui_right")) _inputDirection.X = 0;
		if (Input.IsActionJustReleased("ui_up") || Input.IsActionJustReleased("ui_down")) _inputDirection.Y = 0;

		if (_inputDirection.X != 0)
		{
			_playerVelocity.X += _inputDirection.X * (float)delta * _playerSpeed;
			_playerVelocity.X = Mathf.Clamp(_playerVelocity.X, -_maxRunSpeed, _maxRunSpeed);
			if (_inputDirection.X > 0)
			{
				_playerSprite.FlipH = false;
				//_attackSprite.FlipH = false;
				//_hurtSprite.FlipH = false;
				_myWeaponMan.Scale = new Vector2(1, 1);
			}
			else
			{
				_playerSprite.FlipH = true;
				//_attackSprite.FlipH = true;
				//_hurtSprite.FlipH = true;
				_myWeaponMan.Scale = new Vector2(-1, 1);

			}
		}
		else
		{
			_playerVelocity.X = 0;
		}

		if (_inputDirection.Y != 0 & _isTouchingLadder)
		{
			_isInClimbMode = true;
		}

		if (_playerBody.IsOnFloor() && _landTimer.IsStopped())
		{
			if (_inputDirection.X == 0)
			{
				//_currentAnim = "Idle";
				_currentState = _state.idle;

			}
			else
			{
				//_currentAnim = "Run";
				_currentState = _state.run;
			}

		}

	}

	private void HandleClimbModeInput(double delta)
	{
		if (_isInClimbMode)
		{
			if (_inputDirection.X != 0)
			{
				_playerVelocity.X += _inputDirection.X * (float)delta * _climbSpeed;
				_playerVelocity.X = Mathf.Clamp(_playerVelocity.X, -_maxClimbSpeed, _maxClimbSpeed);
				//_currentAnim = "Climb";
				_currentState = _state.climb;

			}
			if (_inputDirection.Y != 0)
			{
				_playerVelocity.Y += _inputDirection.Y * (float)delta * _climbSpeed;
				_playerVelocity.Y = Mathf.Clamp(_playerVelocity.Y, -_maxClimbSpeed, _maxClimbSpeed);
				//_currentAnim = "Climb";
				_currentState = _state.climb;
			}
			if (_inputDirection.X == 0 && !_isJumping)
			{
				_playerVelocity.X = 0;

			}
			if (_inputDirection.Y == 0 && !_isJumping)
			{
				_playerVelocity.Y = 0;

			}

			if (_inputDirection.X == 0 && _inputDirection.Y == 0)
			{
				//_currentAnim = "Climb_Idle";
				_currentState = _state.climbidle;
				//GD.Print("Climb_Idle");
			}

		}
		if (_inputDirection.Y > 0)
		{
			_lowerLadderColl.Disabled = false;
		}
		else
		{
			_lowerLadderColl.Disabled = true;
		}

	}
	private void HandleJumpInput()
	{
		if (Input.IsActionPressed("action_X") && _jumpTimer.IsStopped())
		{
			if (_playerBody.IsOnFloor() || _isInClimbMode)
			{
				_playerVelocity.Y -= _jumpPower;
				_isJumping = true;
				_jumpTimer.Start();
				_isInClimbMode = false;

			}

		}
		if (!_playerBody.IsOnFloor() && !_isInClimbMode)
		{
			_playerVelocity.Y += _gravity;
			_isGravityReset = false;
			if (_playerVelocity.Y < 0)
			{
				//_currentAnim = "Jump";
				_currentState = _state.jump;
			}
			if (_playerVelocity.Y > 0)
			{
				//_currentAnim = "Fall";
				_currentState = _state.fall;
			}
		}

		if (_playerBody.IsOnFloor())
		{
			if (!_isGravityReset)
			{
				_playerVelocity.Y = 0;
				_isGravityReset = true;
				//_currentAnim = "Land";
				_currentState = _state.land;
				_landTimer.Start();
			}
		}
	}

	private void HandleAttackInput()
	{
		if (Input.IsActionPressed("action_C") && _attackTimer.IsStopped() && !_isInClimbMode && _myWeaponMan.GetWeapon1UsableStatus())
		{
			//_attackSprite.Visible = true;
			//_playerAnim.Play("Attack_" + _currentAnim);
			_currentMod = _stateMod.attack_;
			_myWeaponMan.PlayerAttack_1(_playerSprite.FlipH);
			_attackTimer.Start();

		}
		else if (Input.IsActionPressed("action_V") && _attackTimer.IsStopped() && !_isInClimbMode && _myWeaponMan.GetWeapon2UsableStatus())
		{
			//_attackSprite.Visible = true;
			//_playerAnim.Play("Attack_" + _currentAnim);
			_currentMod = _stateMod.attack_;
			_myWeaponMan.PlayerAttack_2(_playerSprite.FlipH);
			_attackTimer.Start();
		}
		else
		{
			if (_attackTimer.IsStopped())
			{
				//_attackSprite.Visible = false;
				//_playerAnim.Play(_currentAnim);
				_currentMod = _stateMod.none;
			}
			else
			{
				//_playerAnim.Play("Attack_" + _currentAnim);
				_currentMod = _stateMod.attack_;
			}
		}
	}

	private void HandleDamageInput()
	{
		if (_isKnockedOut)
		{
			_currentMod = _stateMod.none;
			_currentState = _state.knockout;
			_playerVelocity.X = 0;

		}
		else if (_isRecovering && _currentMod != _stateMod.attack_)
		{
			_currentMod = _stateMod.recover_;
		}
	}

	private void DetermineCurrentAnimation()
	{
		if (_currentMod == _stateMod.none)
		{
			_playerAnim.Play(_currentState.ToString());
		}
		else
		{
			try
			{
				_playerAnim.Play(_currentMod + _currentState.ToString());
			}
			catch(Exception e)
			{
				GD.PrintErr(_currentMod + _currentState.ToString() + " counldn't play : exception: " + e);
			}
			
		}
	}

	private void TemporarilyDisableFloorDetection()
	{
		if (_isInClimbMode && Input.IsActionPressed("ui_down") && !_isFloorDetectionDisabled && _playerBody.IsOnFloor())
		{
			_playerBody.SetCollisionMaskValue(7, false);
			//GD.Print("Disabling floor");
			_tempPosition = _playerBody.GlobalPosition;
			_isFloorDetectionDisabled = true;
		}
		if (Math.Abs(_tempPosition.Y - _playerBody.GlobalPosition.Y) > 16 || Math.Abs(_tempPosition.X - _playerBody.GlobalPosition.X) > 16)
		{
			if (_isFloorDetectionDisabled)
			{
				_isFloorDetectionDisabled = false;
				_playerBody.SetCollisionMaskValue(7, true);
				//GD.Print("Reenabling floor");
			}
		}
	}

	private void EnterExitPauseMovementMode()
	{
		if (_isEnterPressed)
		{
			if (_isMovementActive)
			{
				_isMovementActive = false;
			}
			else
			{
				_isMovementActive = true;
			}
			EmitSignal("ActivatePauseMenu", _isMovementActive);
		}
	}

	private void DisableLowerLadderCollider(bool isDisabled)
	{
		_lowerLadderColl.Disabled = isDisabled;
	}

	private void OnLadderDetectorBodyEntered(Node2D body)
	{
		_isTouchingLadder = true;
	}
	private void OnLadderDetectorBodyExited(Node2D body)
	{
		CallDeferred("DisableLowerLadderCollider", false);
		_isTouchingLadder = false;
		_isInClimbMode = false;
	}

	private void OnJumpTimerTimeout()
	{
		_isJumping = false;
	}

	private void OnPlayerKnockout()
	{
		if (!_isKnockedOut && !_isRecovering)
		{
			_isKnockedOut = true;
			_knockoutTimer.Start();
		}
	}

	private void OnKnockoutTimerTimeout()
	{
		if (!_isRecovering)
		{
			_isKnockedOut = false;
			_isRecovering = true;
			_knockoutTimer.Start();
		}
		else
		{
			_isRecovering = false;
		}

	}
}
