using Godot;
using System;
using Game.Damage;
using Game.Upgrades;

public partial class TrapProjectile : Node2D
{
	[Export]
	private Texture2D _arrowTexture, _spearTexture, _shurikenTexture, _cannonballTexture, _bigCannonballTexture, _magicOrbTexture,
	_fireOrbTexture, _iceOrbTexture, _lightningOrbTexture, _poisonOrbTexture;
	[Export] private CharacterBody2D _projectileBody;
	[Export] CollisionShape2D _arrowDetector, _floorDetector, _arrowShape, _orbDetector, _orbShape;
	[Export] private Sprite2D _projectileSprite;
	[Export] private PackedScene _explosionScene;
	[Export] private AnimationPlayer _projAnim;
	[Export] private Timer _destroyTimer;
	private RangedTrap _myParentTrap;
	private Damage _damages;
	private Explosion _myExplosion;
	private RangedTrap _sourceTrap;
	private int _damage, _speed;
	private float _elapsedTime = 1, _elapsedTime2, _gravity = 150, _firstPosition, _secondPosition, _horizontalOffset;
	private double _lifetime, _maxLifetime;
	private TrapType _myProjType;
	private Vector2 _targetEnemyPosition, _fixedTarget, _myVelocity, _targetVelocity, _myDirection, _myPosition, _arcVelocity = new Vector2(100, -10), _startPosition,
	_currentFrame, _nextFrame;
	private BasicEnemyMovement _targetEnemy;
	private string _flightAnimation;
	private bool _isFixedTargetSet, _istargetMovingLeft, _isEnemyDirectionDetermined, _isExploding;
	//private float _targetSpeed;
	private float _ascentDuration = .7f;  // Time to reach peak
	private float _descentDuration = .7f; // Time to descend to target
	private float _peakHeight = 300f;      // Height above the higher of start/target

	public override void _Ready()
	{
		GlobalSignals.Instance.RemoveEnemyFromArea += UntargetDisposedEnemy;
		_myParentTrap = GetNode<RangedTrap>("..");
		SetInitialStats();
		_startPosition = _projectileBody.GlobalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{

		SetFlightBehavior(delta);
		_projectileBody.Velocity = _myVelocity;
		_projectileBody.MoveAndSlide();
		_lifetime += delta;
		if (_lifetime >= _maxLifetime)
		{
			QueueFree();
			//GD.Print("Destroyed Arrow on Timeout!");
		}
	}

	private void SetInitialStats()              //////// each Magic Projectile should be set to explode
	{
		switch (_myProjType)
		{
			case TrapType.ballista:
				_projectileSprite.Texture = _arrowTexture;
				_maxLifetime = .5;
				_projectileSprite.Hframes = 1;
				_speed = 350;
				_damage = 25;
				_floorDetector.Disabled = true;
				_orbDetector.Disabled = true;
				_orbShape.Disabled = true;
				break;
			case TrapType.spear:
				_projectileSprite.Texture = _spearTexture;
				_maxLifetime = 1;
				_projectileSprite.Hframes = 1;
				_speed = 275;
				_floorDetector.Disabled = true;
				_orbDetector.Disabled = true;
				_orbShape.Disabled = true;
				break;
			case TrapType.shuriken:
				_projectileSprite.Texture = _shurikenTexture;
				_maxLifetime = .5;
				_projectileSprite.Hframes = 1;
				_speed = 350;
				_floorDetector.Disabled = true;
				_orbDetector.Disabled = true;
				_orbShape.Disabled = true;
				break;
			case TrapType.cannon:
				_projectileSprite.Texture = _cannonballTexture;
				_speed = 5;
				_maxLifetime = 3;
				_projectileSprite.Hframes = 1;
				_arrowDetector.Disabled = true;
				_floorDetector.Disabled = true;
				_arrowShape.Disabled = true;
				_orbDetector.Disabled = true;
				_orbShape.Disabled = true;
				break;
			case TrapType.bigcannon:
				_projectileSprite.Texture = _bigCannonballTexture;
				_speed = 5;
				_maxLifetime = 3;
				_projectileSprite.Hframes = 1;
				_arrowDetector.Disabled = true;
				_floorDetector.Disabled = true;
				_arrowShape.Disabled = true;
				_orbDetector.Disabled = true;
				_orbShape.Disabled = true;
				break;
			case TrapType.magic:
				_flightAnimation = "MagicOrb";
				_destroyTimer.WaitTime = .5;
				_projectileSprite.Texture = _magicOrbTexture;
				_projectileSprite.Hframes = 17;
				_maxLifetime = 2;
				_speed = 140;
				_damage = 40;
				_floorDetector.Disabled = true;
				_arrowDetector.Disabled = true;
				_arrowShape.Disabled = true;
				break;
			case TrapType.fireorb:
				_flightAnimation = "FireOrb";
				_projectileSprite.Texture = _fireOrbTexture;
				_projectileSprite.Hframes = 17;
				_maxLifetime = 2;
				_speed = 100;
				_floorDetector.Disabled = true;
				_arrowDetector.Disabled = true;
				_arrowShape.Disabled = true;
				break;
			case TrapType.iceorb:
				_flightAnimation = "IceOrb";
				_projectileSprite.Texture = _iceOrbTexture;
				_projectileSprite.Hframes = 17;
				_maxLifetime = 2;
				_speed = 100;
				_floorDetector.Disabled = true;
				_arrowDetector.Disabled = true;
				_arrowShape.Disabled = true;
				break;
			case TrapType.lightningorb:
				_flightAnimation = "LightningOrb";
				_projectileSprite.Texture = _lightningOrbTexture;
				_projectileSprite.Hframes = 17;
				_maxLifetime = 2;
				_speed = 100;
				_floorDetector.Disabled = true;
				_arrowDetector.Disabled = true;
				_arrowShape.Disabled = true;
				break;
			case TrapType.poisonorb:
				_flightAnimation = "PoisonOrb";
				_projectileSprite.Texture = _poisonOrbTexture;
				_projectileSprite.Hframes = 17;
				_maxLifetime = 2;
				_speed = 100;
				_floorDetector.Disabled = true;
				_arrowDetector.Disabled = true;
				_arrowShape.Disabled = true;
				break;
		}
	}

	private void SetFlightBehavior(double delta)
	{

		if (_myProjType == TrapType.ballista || _myProjType == TrapType.spear || 
		_myProjType == TrapType.shuriken)
		{
			FlyStraight();
			RotateProjectile();

			if (GodotObject.IsInstanceValid(_targetEnemy))
			{
				_targetEnemyPosition = _targetEnemy.GetEnemyPosition();
			}

		}
		else if (_myProjType == TrapType.cannon || _myProjType == TrapType.bigcannon|| _myProjType == TrapType.tricannon)
		{

			FlyInArc2(delta);

		}
		else if (_myProjType == TrapType.magic || _myProjType == TrapType.fireorb ||
		_myProjType == TrapType.iceorb || _myProjType == TrapType.lightningorb ||
		_myProjType == TrapType.poisonorb)
		{
			if (!_isExploding)
			{
				FlyStraight();
				_projAnim.Play(_flightAnimation);

				if (GodotObject.IsInstanceValid(_targetEnemy))
				{
					_targetEnemyPosition = _targetEnemy.GetEnemyPosition();
				}
			}
		}
	}
	public void SetParentTrap(RangedTrap ranged)
	{
		_sourceTrap = ranged;
	}

	public void SetOriginPosition(Vector2 position)
	{
		_projectileBody.GlobalPosition = position;
	}

	public void SetProjectileType(TrapType type)
	{
		_myProjType = type;
	}

	public TrapType GetProjectileType()
	{
		return _myProjType;
	}
	public void SetTargetPosition(Vector2 target)
	{
		_targetEnemyPosition = target;
	}

	public void SetTargetVelocity(Vector2 velocity)
	{
		_targetVelocity = velocity;
	}

	public void SetTargetEnemy(BasicEnemyMovement enemy)
	{
		_targetEnemy = enemy;
	}
	public void SetDamage(Damage myDamage)
	{
		_damages = myDamage;
	}


	public Damage GetDamage()
	{
		return _damages;
	}
	private void FlyStraight()
	{
		_myVelocity = (_targetEnemyPosition - _projectileBody.GlobalPosition).Normalized() * _speed;
	}

	private void RotateProjectile()
	{
		if (_myProjType == TrapType.ballista || _myProjType == TrapType.spear)
		{
			_myDirection = _projectileBody.GlobalPosition.DirectionTo(_targetEnemyPosition);
			_projectileBody.Rotation = _myDirection.Angle();
		}
	}

	private void FlyInArc2(double delta)
	{
		_elapsedTime += (float)delta;

		float totalDuration = _ascentDuration + _descentDuration;
		float t = Mathf.Clamp(_elapsedTime / totalDuration, 0f, 1f);

		float currentXDistance = _targetEnemyPosition.X - _projectileBody.GlobalPosition.X;
		//_myPosition.X = _projectileBody.GlobalPosition.X + currentXDistance + _targetSpeed;
		_myPosition.X = _projectileBody.GlobalPosition.X + currentXDistance + _targetVelocity.X;

		if (_elapsedTime <= _ascentDuration)
		{
			float ascentT = _elapsedTime / _ascentDuration;
			float peakY = Mathf.Min(_startPosition.Y, _targetEnemyPosition.Y) - _peakHeight;
			_myPosition.Y = Mathf.Lerp(_startPosition.Y, peakY, ascentT);
		}
		else
		{
			float descentT = (_elapsedTime - _ascentDuration) / _descentDuration;
			float peakY = Mathf.Min(_startPosition.Y, _targetEnemyPosition.Y) - _peakHeight;
			_myPosition.Y = Mathf.Lerp(peakY, _targetEnemyPosition.Y, descentT);

		}
		if (_myPosition.Y > 0 && MathF.Abs(_projectileBody.GlobalPosition.Y) - MathF.Abs(_targetEnemyPosition.Y) < 16)
		{
			_floorDetector.Disabled = false;
		}

		_myVelocity = _myPosition - GlobalPosition;
	}

	private void UntargetDisposedEnemy(BasicEnemyMovement myEnemy, Vector2 position)
	{
		if (_targetEnemy == myEnemy)
		{
			if (GodotObject.IsInstanceValid(_destroyTimer))
			{
				//_destroyTimer.Start();	
				_targetEnemyPosition = _targetEnemy.GetEnemyPosition();	
			}
		}
	}

	private void AddExplosionToParent()
	{
		ProjectileManager.Instance.AddChild(_myExplosion);
	}
	private void OnArrowDetectorBodyEntered(Node2D body)
	{
		_destroyTimer.Start();
	}
	private void OnDestroyTimerTimeout()
	{
		QueueFree();
	}

	private void OnFloorDetectorTriggered(Node2D body)
	{
		_myExplosion = _explosionScene.Instantiate<Explosion>();
		_myExplosion.GlobalPosition = _projectileBody.GlobalPosition;
		_myExplosion.SetDamage(_damages);
		_myExplosion.SnapToNearestFloor();
		_myExplosion.Explode();
		CallDeferred("AddExplosionToParent");
		QueueFree();

	}

	private void OnOrbDetectorBodyEntered(Node2D body)
	{
		_myVelocity = Vector2.Zero;
		_isExploding = true;
		_projAnim.Play(_flightAnimation + "Explosion");
		_destroyTimer.Start();
	}


}
