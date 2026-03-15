using Game.Damage;
using Game.Upgrades;
using Godot;
using System;

public partial class WeaponManager : Node2D
{
	[Export] private AnimationPlayer _throwAnim;
	[Export] private Sprite2D _throwSprite;
	[Export] private PackedScene _projectileScene;
	[Export] private Timer _attack1Cooldown, _attack2Cooldown;
	private PlayerProjectile _myProj;

	public bool GetWeapon1UsableStatus()
	{
		return _attack1Cooldown.IsStopped();
	}

	public bool GetWeapon2UsableStatus()
	{
		return _attack2Cooldown.IsStopped();
	}

	public void PlayerAttack_1(bool spriteFlipped)
	{
		if (_attack1Cooldown.IsStopped())
		{
			_myProj = _projectileScene.Instantiate<PlayerProjectile>();
			_myProj.GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y - 8);
			ProjectileManager.Instance.AddChild(_myProj);
			_myProj.SetWeaponType((WeaponType)PlayerStatistics.Instance._currentWeaponIndex.Item1, spriteFlipped);

			//_myProj.SetWeaponType(WeaponType.shuriken, spriteFlipped);

			_myProj.SetWeaponDamage(PlayerStatistics.Instance._weaponDamage[PlayerStatistics.Instance._currentWeaponIndex.Item1]);
			_attack1Cooldown.Start();
			GlobalSignals.Instance.EmitSignal("PlayerAttack", true);
			PlayThrowAnimation((WeaponType)PlayerStatistics.Instance._currentWeaponIndex.Item1);
		}

	}
	public void PlayerAttack_2(bool spriteFlipped)
	{
		if (_attack2Cooldown.IsStopped())
		{
			_myProj = _projectileScene.Instantiate<PlayerProjectile>();
			_myProj.GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y - 8);
			ProjectileManager.Instance.AddChild(_myProj);
			_myProj.SetWeaponType((WeaponType)PlayerStatistics.Instance._currentWeaponIndex.Item2, spriteFlipped);
			_myProj.SetWeaponDamage(PlayerStatistics.Instance._weaponDamage[PlayerStatistics.Instance._currentWeaponIndex.Item2]);
			_attack2Cooldown.Start();
			GlobalSignals.Instance.EmitSignal("PlayerAttack", false);
			PlayThrowAnimation((WeaponType)PlayerStatistics.Instance._currentWeaponIndex.Item2);
		}
	}


	private void PlayThrowAnimation(WeaponType weapon)
	{
		if (!_throwAnim.IsPlaying())
		{
			if(weapon == WeaponType.bow)
			{
				_throwAnim.Play("Bow");
				GD.Print("BOW");
			}
			if(weapon == WeaponType.scepter)
			{
				_throwAnim.Play("Scepter");
				GD.Print("SCEPTER");
			}
			if(weapon == WeaponType.sword || weapon == WeaponType.hammer || weapon == WeaponType.axe || weapon == WeaponType.dagger||
			weapon == WeaponType.spear || weapon == WeaponType.club || weapon == WeaponType.potion|| weapon == WeaponType.shuriken)
			{
				_throwAnim.Play("Throw");
				GD.Print("THROW");
			}
				
		}
	}
}
