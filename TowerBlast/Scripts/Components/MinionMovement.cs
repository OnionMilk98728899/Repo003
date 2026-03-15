using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;

public partial class MinionMovement : Node2D
{
	[Signal] public delegate void KillMinionEventHandler();
	[Export] private Texture2D _wormSheet, _ratSheet, _miteSheet;
	[Export] private Sprite2D _minionSprite;
	[Export] private CharacterBody2D _minionBody;
	[Export] private Timer _attackTimer;
	[Export] public Label _debugLabel;
	private Vector2 _minionVelocity, _enemyVelocity;
	private BasicEnemyMovement _targetEnemy, _closestEnemy;
	private List<BasicEnemyMovement> _enemyList = new List<BasicEnemyMovement>();
	private Hurtbox _enemyHurtbox;
	//private Vector2 _targetEnemy;
	private StaticTrap._minionType _myMinionType;
	[Export] private int _speed, _damage, _life;
	private float _horizontalDist, _verticalDist;
	private bool _isEnemyDetected, _isApproachingEnemy, _isEngaged, _isEnemyMovingAway = true, _isEnemyMovementDecided;

	public override void _Ready()
	{

	}
	public override void _PhysicsProcess(double delta)
	{
		if (_isEnemyDetected && _targetEnemy == null)
		{
			FindClosestApproachingEnemy();
		}
		if (_isApproachingEnemy && _targetEnemy != null)
		{
			ApproachEnemy();
		}
		if (_isEngaged && _targetEnemy != null)
		{
			EngageEnemy();
		}

		_minionBody.Velocity = _minionVelocity;
		_minionBody.MoveAndSlide();
		_debugLabel.Text = $"{_life}";
	}
	public void SetMinionPosition(Vector2 position)
	{
		_minionBody.GlobalPosition = position;
	}

	public void SetDebugLabelOffSet(int offset)
	{
		_debugLabel.Position = new Vector2(_debugLabel.Position.X, _debugLabel.Position.Y - offset);
	}
	public void SetMinionType(StaticTrap._minionType minType)
	{
		_myMinionType = minType;

		switch (_myMinionType)
		{
			case StaticTrap._minionType.mites:
				SetMinionBehavior(_miteSheet, 60, 10, 10);
				break;
			case StaticTrap._minionType.rat:
				SetMinionBehavior(_ratSheet, 40, 10, 10);
				break;
			case StaticTrap._minionType.worm:
				SetMinionBehavior(_wormSheet, 30, 10, 10);
				break;
		}
	}

	private void ApproachEnemy()
	{
		_minionVelocity = (_targetEnemy.GetEnemyPosition() - _minionBody.GlobalPosition).Normalized() * _speed;

		if (MathF.Abs(_targetEnemy.GetEnemyPosition().X - _minionBody.GlobalPosition.X) < 2 &&
		MathF.Abs(_targetEnemy.GetEnemyPosition().Y - _minionBody.GlobalPosition.Y) < 8)
		{
			_isEngaged = true;
			_isApproachingEnemy = false;
		}
	}

	private void EngageEnemy()
	{
		_minionVelocity = Vector2.Zero;
		_targetEnemy.EngageWithMinion();

		if (_attackTimer.IsStopped())
		{
			_attackTimer.Start();
		}
	}

	public bool GetEngagedStatus()
	{
		return _isEngaged;
	}

	public bool GetMovingAwayStatus()
	{
		return _isEnemyMovingAway;
	}

	public BasicEnemyMovement GetTargetId()
	{
		return _targetEnemy;
	}

	private void SetMinionBehavior(Texture2D texture, int speed, int damage, int life)
	{
		_minionSprite.Texture = texture;
		_speed = speed;
		_damage = damage;
		_life = life;
	}

	private void FindClosestApproachingEnemy()
	{
		float closestDist = 1000;

		foreach (BasicEnemyMovement enemy in _enemyList)
		{
			if (MathF.Abs(_minionBody.GlobalPosition.Y - enemy.GetEnemyPosition().Y) < 8)
			{
				_enemyVelocity = enemy.GetEnemyVelocity();
				if (_enemyVelocity.X <= 0)
				{
					if (_minionBody.GlobalPosition.X - enemy.GetEnemyPosition().X < 0)
					{
						_isEnemyMovingAway = false;
					}
					else
					{
						_isEnemyMovingAway = true;
					}
				}
				else
				{
					if (_minionBody.GlobalPosition.X - enemy.GetEnemyPosition().X < 0)
					{
						_isEnemyMovingAway = true;
					}
					else
					{
						_isEnemyMovingAway = false;
					}
				}

				if (!_isEnemyMovingAway)
				{
					float dist = MathF.Abs(_minionBody.GlobalPosition.X - enemy.GetEnemyPosition().X);
					if (dist < closestDist)
					{
						closestDist = dist;
						_closestEnemy = enemy;
					}
				}
			}


		}

		if (_closestEnemy != null && !_closestEnemy.GetEnemyTargetedStatus())
		{
			_targetEnemy = _closestEnemy;
			_targetEnemy.SetEnemyTargetedStatus(true);
			_enemyHurtbox = _targetEnemy.GetNode<Hurtbox>("EnemyBody/Hurtbox");
			_isApproachingEnemy = true;

			if (!_targetEnemy.GetNode<MinionDetector>("EnemyBody/MinionDetector").GetAttackMode())
			{
				_targetEnemy.GetNode<MinionDetector>("EnemyBody/MinionDetector").EngageWithFreshlySpawnedMinion(_minionBody);
			}

		}
	}
	private void OnEnemyDetected(Node2D body)
	{

		if (body.IsInGroup("Enemy"))
		{
			_enemyList.Add(body.GetNode<BasicEnemyMovement>(".."));
			_isEnemyDetected = true;
		}

	}

	private void OnEnemyDetectorExited(Node2D body)
	{
		if (body.IsInGroup("Enemy"))
		{
			_enemyList.Remove(body.GetNode<BasicEnemyMovement>(".."));
			if (_enemyList.Count == 0)
			{
				_isEnemyDetected = false;
			}
		}
	}
	public void OnEnemyKilled(BasicEnemyMovement enemyBody)
	{
		_enemyList.Remove(enemyBody);
		if (_enemyList.Count == 0)
		{
			_isEnemyDetected = false;
		}
		if (_targetEnemy == enemyBody)
		{
			_targetEnemy = null;
			_isEngaged = false;
			_isApproachingEnemy = false;
		}

	}

	public void DamageMinion(int damage)
	{
		_life -= damage;

		if (_life <= 0)
		{
			_targetEnemy.ExitAttackMode();
			EmitSignal("KillMinion");
		}
	}

	private void OnAttackTimerTimeout()
	{

		if (_enemyHurtbox != null && _enemyHurtbox.GetHealth() > 0)
		{
			_enemyHurtbox.TakeDamageFromMinion(_damage);
		}
	}

}
