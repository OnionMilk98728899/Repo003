using Game.Damage;
using Game.Palette;
using Game.Upgrades;

//using GColor = Godot.Color;

using Godot;
using System;
using System.Collections.Generic;

public partial class Hurtbox : Node2D
{
	[Signal] public delegate void TakeDamageEventHandler(int damage, int armorDamage);
	[Signal] public delegate void AugmentMovementEventHandler(int dmgType, float damageTime, int movePenalty, bool isCrushing);
	[Signal] public delegate void DestroyUnitEventHandler();
	[Signal] public delegate void CrushingHitSpeedPenaltyEventHandler(int speedReduction);
	[Signal] public delegate void FrozenSpeedPenaltyEventHandler(int speedReduction);
	[Signal] public delegate void SetHealthAndArmorEventHandler(int maxHealth, int maxArmor);
	[Signal] public delegate void StickShurikenToBodyEventHandler(PlayerProjectile projectile);
	[Signal] public delegate void PlayerKnockoutEventHandler();
	[Export] private int _currentHealth, _maxHealth, _currentArmor, _maxArmor, _physicalResist, _fireResist, _iceResist, _poisonResist, _elecResist, _magicResist;
	[Export] Area2D _hurtBoxArea;
	[Export] private PackedScene _popUpLabelScene;
	private PopUp _popUpLabel;
	[Export] private Label _debugLabel;
	[Export] private int _damageLabelSpeed, _specialLabelSpeed;
	private Vector2 _labelOffset;
	//private int[] _damage;
	private Damage _damages;
	private WeaponType _damagingWeapon;
	private TrapType _damagingTrap;
	private int _burnCounter, _minionHealthDamage, _minionArmorDamage, _totalDamage, _totalArmorDamage, _crushCoefficient, _shurikenCounter;
	private bool _isDestroyTriggered, _isCrushingHit;



	public enum _unitType
	{
		enemy, player
	}

	[Export] private _unitType _myUnitType;

	public override void _Ready()
	{
		EmitSignal("SetHealthAndArmor", _maxHealth, _maxArmor);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_myUnitType == _unitType.enemy)
		{
			//_debugLabel.Text = _currentHealth.ToString(); 
		}

	}
	public int GetHealth()
	{
		return _currentHealth;
	}

	public void TakeDamageFromMinion(int damage)
	{

		if (_currentArmor > 0)
		{
			if (_currentArmor > damage)
			{
				_currentArmor -= damage;
				_minionArmorDamage = damage;
			}
			else
			{
				_minionArmorDamage = _currentArmor;
				int remainder = damage - _currentArmor;
				_currentArmor = 0;
				_currentHealth -= remainder;
				_minionHealthDamage = remainder;

			}
		}
		else
		{
			_currentHealth -= damage;
			_minionHealthDamage = damage;
		}

		EmitSignal(SignalName.TakeDamage, _minionHealthDamage, _minionArmorDamage);

		if (_currentHealth <= 0 && !_isDestroyTriggered)
		{
			_isDestroyTriggered = true;
			EmitSignal(SignalName.DestroyUnit);

		}
	}

	private void CalculateDamage(Damage damages)
	{
		_totalDamage = 0;
		_totalArmorDamage = 0;

		if (GD.RandRange(1, 100) <= damages.criticalHit)           // ### Calculates critical hit cefficient
		{
			_damages.baseDamage = damages.baseDamage * 2;
			_damages.magicDamage = damages.magicDamage * 2;
			_damages.fireDamage = damages.fireDamage * 2;
			_damages.iceDamage = damages.iceDamage * 2;
			_damages.poisonDamage = damages.poisonDamage * 2;
			_damages.elecDamage = damages.elecDamage * 2;
			OutputDamageLabel("crit!", _specialLabelSpeed, GameColors.red);
		}
		else
		{
			_damages.baseDamage = damages.baseDamage;
			_damages.magicDamage = damages.magicDamage;
			_damages.fireDamage = damages.fireDamage;
			_damages.iceDamage = damages.iceDamage;
			_damages.poisonDamage = damages.poisonDamage;
			_damages.elecDamage = damages.elecDamage;
		}
		if (GD.RandRange(1, 100) <= damages.crushingHit)        // ### Calculates crushing hit coefficient
		{
			ReduceOverallResistances(GD.RandRange(3, 7));
			_isCrushingHit = true;
			//Reduce Enemy Speed
			OutputDamageLabel("X", _damageLabelSpeed, GameColors.red);
		}
		else
		{
			_isCrushingHit = false;
		}

		if (_damages.magicDamage > 0)
		{
			int revisedMagicDmg = _damages.magicDamage * ((100 - _magicResist) / 100);
			_currentHealth -= revisedMagicDmg;
			_currentArmor -= revisedMagicDmg;
			_totalDamage += revisedMagicDmg;
			_totalArmorDamage += revisedMagicDmg;
			EmitSignal(SignalName.AugmentMovement, (int)DamageType.magic, damages.damageTime, damages.movePenalty, _isCrushingHit);
			OutputDamageLabel(revisedMagicDmg.ToString(), _damageLabelSpeed, GameColors.purple);

		}
		if (_damages.fireDamage > 0)
		{
			int revisedFireDmg = _damages.fireDamage * ((100 - _fireResist) / 100);
			_currentHealth -= revisedFireDmg;
			_currentArmor -= revisedFireDmg;
			_totalDamage += revisedFireDmg;
			_totalArmorDamage += revisedFireDmg;
			EmitSignal(SignalName.AugmentMovement, (int)DamageType.fire, damages.damageTime, damages.movePenalty, _isCrushingHit);
			OutputDamageLabel(revisedFireDmg.ToString(), _damageLabelSpeed, GameColors.orange);
		}
		if (_damages.iceDamage > 0)
		{
			int revisedIceDmg = _damages.iceDamage * ((100 - _iceResist) / 100);
			_currentHealth -= revisedIceDmg;
			_currentArmor -= revisedIceDmg;
			_totalDamage += revisedIceDmg;
			_totalArmorDamage += revisedIceDmg;
			EmitSignal(SignalName.AugmentMovement, (int)DamageType.ice, damages.damageTime, damages.movePenalty, _isCrushingHit);
			OutputDamageLabel(revisedIceDmg.ToString(), _damageLabelSpeed, GameColors.blue);
		}
		if (_damages.poisonDamage > 0)
		{
			int revisedPoisonDmg = _damages.poisonDamage * ((100 - _poisonResist) / 100);
			_currentHealth -= revisedPoisonDmg;
			_currentArmor -= revisedPoisonDmg;
			_totalDamage += revisedPoisonDmg;
			_totalArmorDamage += revisedPoisonDmg;
			EmitSignal(SignalName.AugmentMovement, (int)DamageType.poison, damages.damageTime, damages.movePenalty, _isCrushingHit);
			OutputDamageLabel(revisedPoisonDmg.ToString(), _damageLabelSpeed, GameColors.green);
		}
		if (_damages.elecDamage > 0)
		{
			int revisedElecDmg = _damages.elecDamage * ((100 - _elecResist) / 100);
			_currentHealth -= revisedElecDmg;
			_currentArmor -= revisedElecDmg;
			_totalDamage += revisedElecDmg;
			_totalArmorDamage += revisedElecDmg;
			EmitSignal(SignalName.AugmentMovement, (int)DamageType.electric, damages.damageTime, damages.movePenalty, _isCrushingHit);
			OutputDamageLabel(revisedElecDmg.ToString(), _damageLabelSpeed, GameColors.yellow);
		}


		if (_damages.baseDamage > 0)
		{
			int revisedBaseDmg = _damages.baseDamage * ((100 - _physicalResist) / 100);  // ### Revised damage accounts for resistances

			if (_damagingWeapon == WeaponType.shuriken || _damagingTrap == TrapType.shuriken)
			{
				revisedBaseDmg = _damages.baseDamage * ((100 - _physicalResist) / 100) + (5 * _shurikenCounter);
			}
			if (_currentArmor > 0)
			{

				if (_currentArmor >= revisedBaseDmg)
				{
					_currentArmor -= revisedBaseDmg;
					float healthThru = revisedBaseDmg * (damages.armorPiercing / 100);
					_currentHealth -= (int)healthThru;
					_totalDamage += (int)healthThru;
					_totalArmorDamage += revisedBaseDmg;
				}
				else
				{
					_totalArmorDamage += _currentArmor;
					float healthThru = _currentArmor * (damages.armorPiercing / 100);
					int remainder = revisedBaseDmg - _currentArmor;
					_currentArmor = 0;
					_currentHealth -= remainder + (int)healthThru;
					_totalDamage += remainder + (int)healthThru;
				}
			}
			else
			{
				_currentHealth -= revisedBaseDmg;
				_totalDamage += revisedBaseDmg;
			}

			EmitSignal(SignalName.AugmentMovement, (int)DamageType.physical, damages.damageTime, damages.movePenalty, _isCrushingHit);

			if (_shurikenCounter > 0)
			{
				OutputDamageLabel(revisedBaseDmg.ToString(), _damageLabelSpeed, GameColors.green);
			}
			else
			{
				OutputDamageLabel(revisedBaseDmg.ToString(), _damageLabelSpeed, GameColors.white);
			}

		}


	}

	private void OutputDamageLabel(string label, int speed, string color)
	{
		_popUpLabel = _popUpLabelScene.Instantiate<PopUp>();
		//_popUpLabel.GlobalPosition = GlobalPosition;
		_labelOffset = new Vector2(GlobalPosition.X, GlobalPosition.Y - 32);
		_popUpLabel.SetProperties(_labelOffset, 1, speed, label, color);
		CallDeferred("AddLabelEffectToParent", _popUpLabel);
	}

	private void AddLabelEffectToParent(PopUp label)
	{
		EffectsManager.Instance.AddChild(label);
	}

	private void ReduceOverallResistances(int reduction)
	{
		_physicalResist -= reduction;
		_magicResist -= reduction;
		_fireResist -= reduction;
		_iceResist -= reduction;
		_poisonResist -= reduction;
		_elecResist -= reduction;
	}

	private void OnHurtboxEntered(Node2D body)
	{
		if (_myUnitType == _unitType.enemy)
		{
			if (body.IsInGroup("Projectile"))
			{
				_damagingWeapon = WeaponType.none;
				_damagingTrap = body.GetNode<TrapProjectile>("..").GetProjectileType();
				_damages = body.GetNode<TrapProjectile>("..").GetDamage();
				//AudioManager.Instance.PlaySFX(AudioManager.Instance._audioLibrary.Hit);

			}
			if (body.IsInGroup("Explosion"))
			{
				_damages = body.GetNode<Explosion>("..").GetDamage();
			}
			if (body.IsInGroup("StaticTrap"))
			{
				_damages = body.GetNode<StaticTrap>("..").GetDamage();
			}
			if (body.IsInGroup("Weapon"))
			{
				PlayerProjectile thisWeapon = body.GetNode<PlayerProjectile>("..");
				_damagingWeapon = thisWeapon.GetWeaponType();
				_damagingTrap = TrapType.none;
				thisWeapon.OnEnemyHurtboxCollide();

				if (thisWeapon.GetWeaponType() == WeaponType.shuriken && !thisWeapon._isStuckInEnemy)
				{
					_shurikenCounter++;
					thisWeapon._isStuckInEnemy = true;
					thisWeapon.SetTargetEnemy(GetNode<BasicEnemyMovement>("../.."));
					EmitSignal(SignalName.StickShurikenToBody, thisWeapon);

				}

				if (body.GetNode<PlayerProjectile>("..").GetWeaponType() != WeaponType.shield)
				{
					_damages = body.GetNode<PlayerProjectile>("..").GetDamage();
					//AudioManager.Instance.PlaySFX(AudioManager.Instance._audioLibrary.Hit);
				}

			}

			CalculateDamage(_damages);
			EmitSignal(SignalName.TakeDamage, _totalDamage, _totalArmorDamage);
		}

		if (_myUnitType == _unitType.player)
		{

			if (body.IsInGroup("EnemyAttack"))
			{
				EmitSignal(SignalName.PlayerKnockout);
				OutputDamageLabel($"-{5}", _damageLabelSpeed, GameColors.white);
				_currentHealth -= 5;
			}
		}

		if (_currentHealth <= 0)
		{
			if (_myUnitType == _unitType.enemy)
			{
				EmitSignal(SignalName.DestroyUnit);
			}
			if (_myUnitType == _unitType.player)
			{
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.PlayerDeath);
			}

		}
	}
}
