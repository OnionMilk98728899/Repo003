using Game.Damage;
using Game.Upgrades;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class RangedTrap : Node2D
{
	[Export]
	private Texture2D _ballistaTexture, _spearTexture, _shurikenTexture, _cannonTexture,
	_bigCannonTexture, _tripleCannonTexture, _magicTexture, _lightningMagicTexture,
	_fireMagicTexture, _iceMagicTexture, _poisonMagicTexture;
	[Export] private CharacterBody2D _trapBody;
	[Export] private Sprite2D _trapSprite;
	[Export] private AnimationPlayer _trapAnim;
	[Export] private Timer _attackBufferTimer, _debugTimer, _projectileBufferTimer;
	[Export] private float _speedInterval;
	[Export] private PackedScene _projectileScene;
	//[Export] private int _damage;
	[Export] private Label _debugLabel;
	[Export] private Area2D _rangedArea;
	private Damage _damages;
	private TrapProjectile _myProjectile;
	private BasicEnemyMovement _enemyBodyToAdd, _enemyBodyToRemove, _furthestEnemy, _targetEnemy;
	private List<BasicEnemyMovement> _enemyList = new List<BasicEnemyMovement>(),
	_highestEnemies = new List<BasicEnemyMovement>(),
	_tempEnemyList = new List<BasicEnemyMovement>();
	private Vector2 _highestY = Vector2.Zero, _targetEnemyPosition, _targetVelocity, _targetDirection;
	private string _targetList, _aimDirection, _shootAnim;
	private bool _isFiring;

	public override void _Ready()
	{
		GlobalSignals.Instance.RemoveEnemyFromArea += RemoveDisposedEnemyFromList;
		DetermineTrapBehavior();
		_trapSprite.Frame = 0;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (_enemyList.Count > 0)
		{
			DetermineTarget();
			FireProjectile();
		}

		if (_myTrapType == TrapType.ballista || _myTrapType == TrapType.spear)
		{
			AimInTargetDirection();
		}
	}
	public enum _attackMode
	{
		hitFirst, hitStrongest, hitRandom
	}
	public TrapType _myProjectileType;
	public TrapType _myTrapType;
	private _attackMode _myAttackMode;

	public void DetermineTrapBehavior()
	{
		switch (_myTrapType)
		{
			case TrapType.ballista:
				SetTrapType(_ballistaTexture, 80, TrapType.ballista, "Ballista");
				break;
			case TrapType.cannon:
				SetTrapType(_cannonTexture, 10, TrapType.cannon, "Cannon");
				break;
			case TrapType.magic:
				SetTrapType(_magicTexture, 10, TrapType.magic, "MagicTrap");
				break;
			case TrapType.spear:
				SetTrapType(_spearTexture, 80, TrapType.spear, "Spear");
				break;
			case TrapType.shuriken:
				SetTrapType(_shurikenTexture, 10, TrapType.shuriken, "Shuriken");
				break;
			case TrapType.bigcannon:
				SetTrapType(_bigCannonTexture, 10, TrapType.bigcannon, "BigCannon");
				break;
			case TrapType.tricannon:
				SetTrapType(_tripleCannonTexture, 10, TrapType.tricannon, "TriCannon");
				break;
			case TrapType.fireorb:
				SetTrapType(_fireMagicTexture, 10, TrapType.fireorb, "MagicTrapFire");
				break;
			case TrapType.iceorb:
				SetTrapType(_iceMagicTexture, 10, TrapType.iceorb, "MagicTrapIce");
				break;
			case TrapType.lightningorb:
				SetTrapType(_lightningMagicTexture, 10, TrapType.lightningorb, "MagicTrapElectric");
				break;
			case TrapType.poisonorb:
				SetTrapType(_poisonMagicTexture, 10, TrapType.poisonorb, "MagicTrapPoison");
				break;
		}
		SetDamage(PlayerStatistics.Instance._trapDamage[(int)_myTrapType]);
	}

	private void SetTrapType(Texture2D texture, int hframes, TrapType projType, string shootAnim)
	{
		_trapSprite.Texture = texture;
		_trapSprite.Hframes = hframes;
		_myProjectileType = projType;
		_shootAnim = shootAnim;
	}

	private void SetDamage(Damage damages)
	{
		_damages = damages;
	}

	private void RemoveDisposedEnemyFromList(BasicEnemyMovement deadEnemy, Vector2 position)
	{
		if (_enemyList.Count == 0)
			return;

		_enemyList.Remove(deadEnemy);
	}

	private void DetermineTarget()
	{
		if (_enemyList.Count > 0)
		{
			switch (_myAttackMode)
			{
				case _attackMode.hitFirst:
					_highestY = Vector2.Zero;
					_highestY.X = 300;
					foreach (BasicEnemyMovement body in _enemyList.ToList())
					{
						if (body.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition.Y < _highestY.Y)
						{
							_highestY = body.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition;

						}
						if (body == null || !GodotObject.IsInstanceValid(body))
						{
							_enemyList.Remove(body);
						}

					}
					foreach (BasicEnemyMovement body in _enemyList)
					{
						if (body.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition.Y == _highestY.Y)
						{
							_highestEnemies.Add(body);

						}
					}

					float xVel = 0;
					if (_highestEnemies.Count > 0)
					{
						foreach (BasicEnemyMovement body in _highestEnemies.ToList())
						{

							if (body == null || !GodotObject.IsInstanceValid(body))
							{
								_enemyList.Remove(body);
								_highestEnemies.Remove(body);
							}
							else
							{
								xVel += body.GetNode<CharacterBody2D>("EnemyBody").Velocity.X;
							}
						}
					}

					if (xVel > 0)
					{
						_furthestEnemy = _highestEnemies[0];
						foreach (BasicEnemyMovement body in _highestEnemies)
						{
							if (body.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition.X > _furthestEnemy.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition.X)
							{
								_furthestEnemy = body;
							}
						}
					}
					if (xVel < 0)
					{
						_furthestEnemy = _highestEnemies[0];
						foreach (BasicEnemyMovement body in _highestEnemies)
						{
							if (body.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition.X < _furthestEnemy.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition.X)
							{
								_furthestEnemy = body;
							}
						}
					}

					_targetList = "";

					foreach (BasicEnemyMovement body in _highestEnemies)
					{
						_targetList += Mathf.Round(body.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition.X) + ", " +
						Mathf.Round(body.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition.Y) + "\n";
					}

					if (_furthestEnemy == null || !GodotObject.IsInstanceValid(_furthestEnemy))
					{
						_enemyList.Remove(_furthestEnemy);
						_highestEnemies.Remove(_furthestEnemy);
						return;
					}
					else
					{
						_targetEnemy = _furthestEnemy;
						_targetEnemyPosition = _furthestEnemy.GetNode<CharacterBody2D>("EnemyBody").GlobalPosition;
						_targetVelocity = _furthestEnemy.GetEnemyVelocity();
						_targetEnemyPosition.Y -= 8; /// Offsets target from floor position

						// _targetList = Mathf.Round(_targetEnemy.X) + ", " +
						//  Mathf.Round(_targetEnemy.Y);
					}

					_highestEnemies.Clear();

					break;
				case _attackMode.hitStrongest:
					break;
				case _attackMode.hitRandom:
					break;
			}
		}
	}

	private void AimInTargetDirection()
	{
		if (_targetEnemy != null)
		{
			//float angle = _trapBody.GlobalPosition.AngleToPoint(_targetEnemy.GlobalPosition);

			Vector2 direction = _targetEnemy.GetEnemyPosition() - _trapBody.GlobalPosition;
			float angleRadians = direction.Angle();
			float angleDegrees = Mathf.RadToDeg(angleRadians);

			if (!_trapAnim.IsPlaying())
			{
				if (angleDegrees > -22.5 && angleDegrees <= 22.5)
				{
					///Face Right
					_trapSprite.Frame = 0;
					_aimDirection = "_Right";
				}
				else if (angleDegrees > 22.5 && angleDegrees <= 67.5)
				{
					///Face Down-Right
					_trapSprite.Frame = 10;
					_aimDirection = "_DownRight";
				}
				else if (angleDegrees > 67.5 && angleDegrees <= 112.5)
				{
					///Face Down
					_trapSprite.Frame = 20;
					_aimDirection = "_Down";
				}
				else if (angleDegrees > 112.5 && angleDegrees <= 157.5)
				{
					//Face Down-Left
					_trapSprite.Frame = 30;
					_aimDirection = "_DownLeft";

				}
				else if (angleDegrees > 157.5 || angleDegrees <= -157.5)
				{
					///Face Left
					_trapSprite.Frame = 40;
					_aimDirection = "_Left";
				}
				else if (angleDegrees > -157.5 && angleDegrees <= -112.5)
				{
					//Face left-up
					_trapSprite.Frame = 50;
					_aimDirection = "_UpLeft";
				}
				else if (angleDegrees > -112.5 && angleDegrees <= -67.5)
				{
					///Face up
					_trapSprite.Frame = 60;
					_aimDirection = "_Up";
				}
				else if (angleDegrees > -67.5 && angleDegrees <= -22.5)
				{
					///Face Up-right
					_trapSprite.Frame = 70;
					_aimDirection = "_UpRight";
				}
			}

		}

	}

	private void FireProjectile()
	{
		if (_attackBufferTimer.IsStopped() && !_isFiring)
		{
			_isFiring = true;

			if (_myTrapType != TrapType.tricannon)
			{
				_ = InstantiateProjectile(0);
			}
			else
			{
				_ = FireBurstAsync(3, 300);
			}

			if(_myTrapType == TrapType.ballista || _myTrapType == TrapType.spear)
			{
				if(_aimDirection != null)
				{
					_trapAnim.Play(_shootAnim + _aimDirection);
				}
				
			}
			else
			{
				_trapAnim.Play(_shootAnim);
			}
		}
	}

	private async Task FireBurstAsync(int count, int spacingMs)
	{
		for (int i = 0; i < count; i++)
		{
			await InstantiateProjectile(spacingMs);
		}
		_attackBufferTimer.Start();

		_isFiring = false;
	}
	private async Task InstantiateProjectile(int waitTimeMs)
	{
		_myProjectile = _projectileScene.Instantiate<TrapProjectile>();
		_myProjectile.SetOriginPosition(Vector2.Zero);
		_myProjectile.SetProjectileType(_myProjectileType);
		_myProjectile.SetParentTrap(this);
		_myProjectile.SetTargetEnemy(_targetEnemy);
		_myProjectile.SetTargetPosition(_targetEnemyPosition);
		_myProjectile.SetTargetVelocity(_targetVelocity);
		_myProjectile.SetDamage(_damages);
		AddChild(_myProjectile);

		if (waitTimeMs > 0)
		{
			_projectileBufferTimer.WaitTime = waitTimeMs / 1000f;
			_projectileBufferTimer.Start();
			await ToSignal(_projectileBufferTimer, "timeout");
		}
		else
		{
			_attackBufferTimer.Start();
			_isFiring = false;
		}
	}

	public Vector2 GetTargetPosition()
	{
		return _targetEnemyPosition;
	}

	public BasicEnemyMovement GetTargetEnemy()
	{
		return _targetEnemy;
	}


	private void OnRangeDetectorBodyEntered(Node2D body)
	{
		if (body.GetNode<BasicEnemyMovement>("..") != null)
		{
			_enemyBodyToAdd = body.GetNode<BasicEnemyMovement>("..");
			_enemyList.Add(_enemyBodyToAdd);
		}

	}

	private void OnRangeDetectorBodyExited(Node2D body)
	{
		if (body.GetNode<BasicEnemyMovement>("..") != null)
		{
			_enemyBodyToRemove = body.GetNode<BasicEnemyMovement>("..");
			_enemyList.Remove(_enemyBodyToRemove);
		}

	}

	private void OnStaticDetectorBodyEntered(Node2D body)
	{
		if (body.GetNode<BasicEnemyMovement>("..") != null)
		{

		}
	}

	private void OnDebugTimerTimeout()
	{
		_debugLabel.Text = "";
	}

}
