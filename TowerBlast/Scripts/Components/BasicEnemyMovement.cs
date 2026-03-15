using Godot;
using System;
using System.Collections.Generic;

public partial class BasicEnemyMovement : Node2D
{
	[Export] int _walkSpeed, _climbSpeed, _maxWalkSpeed, _maxClimbSpeed, _gravity;
	[Export] private AttackBox _attackBox;
	[Export] CharacterBody2D _enemyBody;
	[Export] Sprite2D _enemySprite;
	[Export] private AnimationPlayer _enemyAnim;
	[Export] private Label debugLabel;
	[Export] private Timer _hurtTimer, _attackTimer;
	[Export] private MinionDetector _minionDetector;
	[Export] private int _damageOutput, _crushVulnerability;
	private Vector2 _enemyVelocity, _target = Vector2.Zero;
	private List<(Vector2, int)> _pathCoordinates = new List<(Vector2, int)>();
	private int _pathSequence = 0, _frozenMovePenalty, _crushMovePenalty, _moveSpeed;
	private bool _isTouchingLadder, _isYTargetAdjusted, _isInAttackMode, _isAMinionTrackingThisBody, _isBeingTargeted, _isFrozen, _isBurning,
	_isPoisoned, _isElectrocuted, _isCrushed, _isPaused;
	private string _moveStateAnimation, _damageStateAnimation, _animationToPlay;

	public enum _damageType
	{
		physical, magic, fire, ice, poison, electric
	}

	public override void _Ready()
	{
		_minionDetector.SetDamage(_damageOutput);
	}
	public override void _PhysicsProcess(double delta)
	{
		GetNextTarget();
		SetEnemyVelocity();
		ApplyGravity();
		AnimateBody();
		_enemyBody.Velocity = _enemyVelocity;
		_enemyBody.MoveAndSlide();

	}
	private void ApplyGravity()
	{
		if (!_enemyBody.IsOnFloor() && !_isTouchingLadder)
		{
			_enemyVelocity.Y += _gravity;
		}
		if (_enemyBody.IsOnFloor() && !_isTouchingLadder && _enemyBody.GlobalPosition.Y >= _target.Y)
		{
			_enemyVelocity.Y = 0;
		}
	}

	private void SetEnemyVelocity()
	{
		if (_isFrozen || _isCrushed)
		{
			double movePen = (double)(100 - _crushMovePenalty - _frozenMovePenalty);
			double movePenPerc = movePen / 100;

			_moveSpeed = (int)(_walkSpeed * movePenPerc);
			debugLabel.Text = _moveSpeed.ToString();


			if (_moveSpeed < 0)
			{
				_moveSpeed = 0;
			}
		}
		else
		{
			debugLabel.Text = _moveSpeed.ToString();
			_moveSpeed = _walkSpeed;
		}

		if (_isInAttackMode || _isPaused)
		{
			_moveSpeed = 0;
		}

		_enemyVelocity = (_target - _enemyBody.GlobalPosition).Normalized() * _moveSpeed;

		if (MathF.Abs(_enemyVelocity.X) > MathF.Abs(_enemyVelocity.Y))
		{
			_enemyVelocity.Y = 0;
		}
		else if (MathF.Abs(_enemyVelocity.Y) > MathF.Abs(_enemyVelocity.X))
		{
			_enemyVelocity.X = 0;
		}

		if (_pathSequence == _pathCoordinates.Count && _enemyBody.GlobalPosition.X - _target.X <= 8)
		{
			_enemyVelocity = Vector2.Zero;
		}

	}

	public Vector2 GetEnemyVelocity()
	{
		return _enemyVelocity;
	}

	private void AnimateBody()
	{
		_moveStateAnimation = "Idle";
		if (_enemyBody.IsOnFloor() && Mathf.Abs(_enemyVelocity.X) > 0)
		{
			_moveStateAnimation = "Walk";
		}
		if (_isTouchingLadder && !_enemyBody.IsOnFloor())
		{
			_moveStateAnimation = "Climb";
		}
		if (_enemyVelocity == Vector2.Zero)
		{
			_moveStateAnimation = "Idle";
		}
		if (_enemyVelocity.X < 0)
		{
			_enemySprite.FlipH = true;
			_attackBox.Scale = new Vector2(-1, 1);
		}
		if (_enemyVelocity.X > 0)
		{
			_enemySprite.FlipH = false;
			_attackBox.Scale = new Vector2(1, 1);
		}
		_animationToPlay = _damageStateAnimation + _moveStateAnimation;

		_enemyAnim.Play(_animationToPlay);
	}
	public Vector2 GetEnemyPosition()
	{
		return _enemyBody.GlobalPosition;
	}

	public void SetEnemyPath(List<(Vector2, int)> pathCoords)
	{
		int i = 0;
		foreach ((Vector2, int) point in pathCoords)
		{
			if (i > 0 && point.Item1.Y == pathCoords[i - 1].Item1.Y)
			{
				if (point.Item1.X > pathCoords[i - 1].Item1.X)
				{
					_pathCoordinates.Add((new Vector2(point.Item1.X + 8, point.Item1.Y), point.Item2));
				}
				else
				{
					_pathCoordinates.Add((new Vector2(point.Item1.X - 8, point.Item1.Y), point.Item2));
				}
			}
			else
			{
				if (i == 0)
				{
					_pathCoordinates.Add((new Vector2(point.Item1.X - 8, point.Item1.Y), point.Item2));
				}
				else
				{
					_pathCoordinates.Add(point);
				}
			}
			i++;
		}
	}

	private void GetNextTarget()
	{

		if (_target == Vector2.Zero)
		{
			_target = _pathCoordinates[_pathSequence].Item1;
		}
		if (MathF.Abs(_enemyBody.GlobalPosition.X - _target.X) < 8 && MathF.Abs(_enemyBody.GlobalPosition.Y - _target.Y) < 8)
		{
			_pathSequence++;
			if (_pathSequence < _pathCoordinates.Count)
			{
				_target = _pathCoordinates[_pathSequence].Item1;
			}
		}

		if (_target.Y > _enemyBody.GlobalPosition.Y) //// Disables Ladder ceiling blocker if the enemy's next target is lower than where it currently is
		{
			_enemyBody.SetCollisionMaskValue(7, false);
			if (!_isYTargetAdjusted)
			{
				_target.Y += 15;
				_isYTargetAdjusted = true;
			}
		}
		else
		{
			_isYTargetAdjusted = false;
			_enemyBody.SetCollisionMaskValue(7, true);
		}
	}
	public bool GetEnemyTargetedStatus()
	{
		return _isBeingTargeted;
	}
	public void SetEnemyTargetedStatus(bool isTargeted)
	{
		_isBeingTargeted = isTargeted;
	}
	public void EngageWithMinion()
	{
		//_attackMovementReduction = (int)(_walkSpeed * (.01f * mvmtPenalty));
		_isInAttackMode = true;
	}



	public void ExitAttackMode()
	{
		_isInAttackMode = false;
		_isBeingTargeted = false;
		_minionDetector.SetAttackMode(false);
	}

	// private void OnCrushingHit(int movePenalty)
	// {
	// 	_isCrushed = true;
	// 	_crushMovePenalty += movePenalty;
	// }

	// private void OnFrozen(int movePenalty)
	// {
	// 	_isFrozen = true;
	// 	_frozenMovePenalty += movePenalty;
	// 	_freezeTimer.Start();

	// }

	// public void MakeProjectileStickToEnemy(PlayerProjectile proj)
	// {
	// 	// Node parent = proj.GetParent();
	// 	// if (parent != null)
	// 	// {
	// 	// 	GD.Print("Added SHuriken!");
	// 	// 	parent.CallDeferred(MethodName.RemoveChild, proj);
	// 	// 	CallDeferred(MethodName.AddChild, proj);
	// 	// }

	// 	proj.CallDeferred(Node.MethodName.Reparent, this);
	// }

	private void OnStickShurikenToBody(PlayerProjectile projectile)
	{
		projectile.CallDeferred(Node.MethodName.Reparent, this);
	}

	private void OnHurtBoxAugmentMovement(int damageType, float damageTime, int movePenalty, bool isCrushed)
	{
		if (_hurtTimer.IsStopped())
		{
			_damageType dmgType = (_damageType)damageType;
			switch (dmgType)
			{
				case _damageType.physical:
					_damageStateAnimation = "Hurt";
					break;
				case _damageType.magic:
					_damageStateAnimation = "Hurt";
					break;
				case _damageType.fire:
					_damageStateAnimation = "Fire";
					break;
				case _damageType.ice:
					_damageStateAnimation = "Ice";
					_isFrozen = true;
					break;
				case _damageType.poison:
					_damageStateAnimation = "Poison";
					break;
				case _damageType.electric:
					_damageStateAnimation = "Elec";
					break;
			}
			if (damageTime > 0) { _hurtTimer.WaitTime = (double)damageTime; } else { _hurtTimer.WaitTime = .8f; }
			_hurtTimer.Start();
		}
		if (isCrushed)
		{
			_crushMovePenalty += _crushVulnerability;
			_isCrushed = true;
		}
		_frozenMovePenalty = movePenalty;

	}
	private void OnLadderDetectorBodyEntered(Node2D body)
	{
		_isTouchingLadder = true;
	}

	private void OnLadderDetectorBodyExited(Node2D body)
	{
		_isTouchingLadder = false;
	}
	private void OnHurtTimerTimeout()
	{
		_isFrozen = false;
		_frozenMovePenalty = 0;
		_damageStateAnimation = "";
	}

	private void OnAttackTimerTimeout()
	{
		_isPaused = false;
	}

	private void OnAttackBoxStopToAttack()
	{
		_isPaused = true;
		_attackTimer.Start();
	}
}

