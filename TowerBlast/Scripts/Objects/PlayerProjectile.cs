using Godot;
using Godot.NativeInterop;
using System;
using Game.Damage;
using Game.Upgrades;

public partial class PlayerProjectile : Node2D
{
	[Export] private CharacterBody2D _projBody;
	[Export] private Sprite2D _projSprite;
	[Export] private AnimationPlayer _projAnim;
	[Export] private int _gravity;
	[Export] private PackedScene _explosionScene;
	[Export] private Area2D _enemyDetector;
	[Export] private Timer _destroyTimer;
	[Export] private CollisionShape2D _projCollider, _enemyDetectCollider;
	private BasicEnemyMovement _targetEnemy;
	private Explosion _myExplosion;
	private WeaponType _myWeaponType;
	//private int _damage, _firedmg, _icedmg, _elecdmg, _poisondmg, _crushchance, _critchance, _armorpiercing, _speed, _lifetime, _maxArcHeight, _penetration, _durability;
	private int _speed;
	private Vector2 _myInitialVelocity, _myVelocity, _target;
	private Damage _damages;
	private string _flightAnimation;
	private bool _hasBounced, _hasInitialAnimPlayed, _isDestroying;
	public bool _isStuckInEnemy;

	public override void _Ready()
	{
		_projAnim.AnimationFinished += OnAnimationFinished;
	}

	public override void _PhysicsProcess(double delta)
	{
		DetermineProjectileBehavior();
		_projBody.Velocity = _myVelocity;
		_projBody.MoveAndSlide();
		if (_isDestroying)
		{
			_projCollider.Disabled = true;
			_projSprite.Visible = false;
			_isDestroying = false;
		}
	}

	public void SetWeaponType(WeaponType weapon, bool isFlipped)
	{
		_myWeaponType = weapon;
		DetermineWeaponStats(isFlipped);
	}

	public WeaponType GetWeaponType()
	{
		return _myWeaponType;
	}
	private void SetWeaponStats(int speed, string projanim, bool isFlipped, Vector2 target)
	{
		_speed = speed;
		_flightAnimation = projanim;
		if (isFlipped)
		{
			_target = new Vector2(target.X * (-1), target.Y);
		}
		else
		{
			_target = target;
		}

		if (_myWeaponType == WeaponType.hammer || _myWeaponType == WeaponType.club ||
		_myWeaponType == WeaponType.potion)
		{
			_myVelocity.Y = target.Y;
		}

	}

	// public void SetVelocity(Vector2 velocity)
	// {
	// 	_myVelocity
	// }

	public void SetWeaponDamage(Damage myDamage)
	{
		_damages = myDamage;
		//GD.Print("Set Damage to" + _damages.baseDamage);
		_damages.durability = 2;
	}
	public void SetTargetEnemy(BasicEnemyMovement enemy)
	{
		_targetEnemy = enemy;
	}


	public Damage GetDamage()
	{
		//GD.Print("Transmitting damage as" + _damages.baseDamage);
		return _damages;
	}

	private void DetermineWeaponStats(bool isFlipped)
	{
		switch (_myWeaponType)
		{
			case WeaponType.sword:
				SetWeaponStats(35, "Sword", isFlipped, new Vector2(100, 0));
				break;
			case WeaponType.hammer:
				SetWeaponStats(35, "Hammer", isFlipped, new Vector2(50, -100));
				break;
			case WeaponType.axe:
				SetWeaponStats(35, "Axe", isFlipped, new Vector2(25, 0));

				break;
			case WeaponType.dagger:
				SetWeaponStats(55, "Dagger", isFlipped, new Vector2(100, 0));

				break;
			case WeaponType.spear:
				SetWeaponStats(25, "Spear", isFlipped, new Vector2(100, 0));

				break;
			case WeaponType.club:
				SetWeaponStats(25, "Club", isFlipped, new Vector2(50, -100));

				break;
			case WeaponType.bomb:
				SetWeaponStats(0, "Bomb", isFlipped, Vector2.Zero);

				break;
			case WeaponType.scepter:
				SetWeaponStats(35, "MagicOrb", isFlipped, new Vector2(60, 0));

				break;
			case WeaponType.bow:
				SetWeaponStats(45, "Arrow", isFlipped, new Vector2(100, 0));

				break;
			case WeaponType.shield:
				SetWeaponStats(0, "ShieldDrop", isFlipped, Vector2.Zero);

				break;
			case WeaponType.potion:
				SetWeaponStats(35, "Potion", isFlipped, new Vector2(40, -100));

				break;
			case WeaponType.shuriken:
				SetWeaponStats(45, "Shuriken", isFlipped, new Vector2(100, 0));

				break;
		}
	}

	private void DetermineProjectileBehavior()
	{
		switch (_myWeaponType)
		{
			case WeaponType.sword:
				FlyStraight();
				break;
			case WeaponType.hammer:
				FlyInArc();
				///bounce off floor
				break;
			case WeaponType.axe:
				FlyStraight();
				///Stop 
				break;
			case WeaponType.dagger:
				FlyStraight();
				break;
			case WeaponType.spear:
				FlyStraight();
				break;
			case WeaponType.club:
				FlyInArc();
				break;
			case WeaponType.bomb:
				DropInPlace();
				break;
			case WeaponType.scepter:
				FlyStraight();
				break;
			case WeaponType.bow:
				FlyStraight();
				break;
			case WeaponType.shield:
				DropInPlace();
				break;
			case WeaponType.potion:
				FlyInArc();
				break;
			case WeaponType.shuriken:
				FlyStraight();
				break;
		}
	}


	private void FlyStraight()
	{
		if (!_isStuckInEnemy)
		{
			_myVelocity = (_target - _projBody.GlobalPosition.Normalized()) * _speed / 10;
			_projAnim.Play(_flightAnimation);
			if (_myVelocity.X < 0)
			{
				_projSprite.FlipH = true;
			}
		}
		else
		{
			_myVelocity = _targetEnemy.GetEnemyVelocity();
			if (!_enemyDetectCollider.Disabled)
			{
				GD.Print("Shutting off enemy detector");
				_projAnim.Stop();
				//_enemyDetector.Monitoring = false;
				_enemyDetectCollider.Disabled = true;
				_projCollider.Disabled = true;
			}
		}

	}

	private void FlyInArc()
	{
		_myVelocity.X = (_target.X - _projBody.GlobalPosition.Normalized().X) * _speed / 10;
		if (!_projBody.IsOnFloor())
		{
			_myVelocity.Y += _gravity;
		}
		else
		{
			if (_myWeaponType == WeaponType.club || _myWeaponType == WeaponType.potion)
			{
				ProjectileExplodes();
			}
			if (_myWeaponType == WeaponType.hammer)
			{
				if (!_hasBounced)
				{
					_hasBounced = true;
					_myVelocity.Y = _target.Y;
				}
				else
				{
					QueueFree();
				}
			}
		}

		if (_myVelocity.X < 0)
		{
			_projSprite.FlipH = true;
		}
		_projAnim.Play(_flightAnimation);
	}

	private void ProjectileExplodes()
	{
		_myExplosion = _explosionScene.Instantiate<Explosion>();
		_myExplosion = _explosionScene.Instantiate<Explosion>();
		_myExplosion.GlobalPosition = _projBody.GlobalPosition;
		_myExplosion.SetDamage(_damages);
		_myExplosion.SnapToNearestFloor();
		_myExplosion.Explode();
		CallDeferred("AddExplosionToParent", _myExplosion);
		QueueFree();
	}

	private void AddExplosionToParent(Explosion explosion)
	{
		ProjectileManager.Instance.AddChild(_myExplosion);
	}

	private void DropInPlace()
	{
		if (!_hasInitialAnimPlayed)
		{
			_projAnim.Play(_flightAnimation);
			_hasInitialAnimPlayed = true;
		}
		if (!_projBody.IsOnFloor())
		{
			_myVelocity.Y += _gravity;
		}
		else
		{
			_myVelocity.Y = 0;
		}
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "ShieldDrop")
		{
			_projAnim.Play("ShieldHold");
		}
	}

	public void OnEnemyHurtboxCollide()
	{
		if (_myWeaponType == WeaponType.bomb && _projBody.IsOnFloor())
		{
			ProjectileExplodes();
		}
		if (_myWeaponType == WeaponType.sword || _myWeaponType == WeaponType.axe ||
		_myWeaponType == WeaponType.dagger || _myWeaponType == WeaponType.spear ||
		_myWeaponType == WeaponType.bow || _myWeaponType == WeaponType.hammer)
		{
			if (_damages.penetration > 0)
			{
				_damages.penetration--;
			}
			else
			{
				// _destroyTimer.Start();
				// _isDestroying = true;
				QueueFree();
			}
		}
	}

	private void OnEnemyDetectorBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Enemy"))
		{

			// if (_myWeaponType == WeaponType.bomb && _projBody.IsOnFloor())
			// {
			// 	ProjectileExplodes();
			// }
			// if (_myWeaponType == WeaponType.sword || _myWeaponType == WeaponType.axe ||
			// _myWeaponType == WeaponType.dagger || _myWeaponType == WeaponType.spear ||
			// _myWeaponType == WeaponType.bow || _myWeaponType == WeaponType.hammer)
			// {
			// 	if (_damages.penetration > 0)
			// 	{
			// 		_damages.penetration--;
			// 	}
			// 	else
			// 	{
			// 		_destroyTimer.Start();
			// 		_isDestroying = true;
			// 	}
			// }
			// if (_myWeaponType == WeaponType.shuriken)
			// {
			// 	// _isStuckInEnemy = true;

			// 	_targetEnemy = body.GetNode<BasicEnemyMovement>("..");
			// 	// _targetEnemy.MakeProjectileStickToEnemy(this);
			// }

		}
		else if (body.IsInGroup("EnemyAttack"))
		{
			if (_myWeaponType == WeaponType.shield)
			{
				if (_damages.durability > 0)
				{
					_damages.durability--;
					_projAnim.Play("ShieldBlock");
				}
				else
				{
					QueueFree();
				}
			}
		}
	}

	private void OnDestroyTimerTimeout()
	{
		QueueFree();
	}
}
