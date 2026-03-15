using Game.Damage;
using Game.Upgrades;
using Godot;


public partial class PlayerStatistics : Node2D
{
	public static PlayerStatistics Instance { get; private set; }
	public Damage[] _weaponDamage = new Damage[12],  _trapDamage = new Damage[21];
	public (int, int) _currentWeaponIndex;
	private float _newDmg;

	public override void _Ready()
	{
		Instance = this;
		_trapDamage[0].baseDamage = 15;
	}

	public void WeaponUpgrade(Upgrade upgrade, int weaponIndex)
	{
		if (upgrade.op == Operation.add)
		{
			WeaponUpgradeAdd(upgrade);
		}
		else if (upgrade.op == Operation.multiply)
		{
			WeaponUpgradeMultiply(upgrade);
		}
		else if (upgrade.op == Operation.buy)
		{
			BuyWeapon(upgrade, weaponIndex);
			//WeaponUpgradeAdd(upgrade);
		}
	}

	private void BuyWeapon(Upgrade upgrade, int weaponIndex)
	{
		if (weaponIndex == 1)
		{
			_currentWeaponIndex.Item1 = (int)upgrade.weaponTypes[0];
			GD.Print("Purchased : " + upgrade.weaponTypes[0]);
		}
		if (weaponIndex == 2)
		{
			_currentWeaponIndex.Item2 = (int)upgrade.weaponTypes[0];
		}

		_weaponDamage[(int)upgrade.weaponTypes[0]].baseDamage += (int)upgrade.value;
	}




	private void WeaponUpgradeAdd(Upgrade upgrade)
	{
		foreach (WeaponType weapon in upgrade.weaponTypes)
		{
			foreach (StatType stat in upgrade.statTypes)
			{
				switch (stat)
				{
					case StatType.baseDamage:
						_weaponDamage[(int)weapon].baseDamage += (int)upgrade.value;
						break;
					case StatType.fireDamage:
						_weaponDamage[(int)weapon].fireDamage += (int)upgrade.value;
						break;
					case StatType.iceDamage:
						_weaponDamage[(int)weapon].iceDamage += (int)upgrade.value;
						break;
					case StatType.poisonDamage:
						_weaponDamage[(int)weapon].poisonDamage += (int)upgrade.value;
						break;
					case StatType.elecDamage:
						_weaponDamage[(int)weapon].elecDamage += (int)upgrade.value;
						break;
					case StatType.armorPiercing:
						_weaponDamage[(int)weapon].armorPiercing += (int)upgrade.value;
						break;
					case StatType.crushingHit:
						_weaponDamage[(int)weapon].crushingHit += (int)upgrade.value;
						break;
					case StatType.criticalHit:
						_weaponDamage[(int)weapon].criticalHit += (int)upgrade.value;
						break;
					case StatType.stun:
						_weaponDamage[(int)weapon].stun += (int)upgrade.value;
						break;
					case StatType.penetration:
						_weaponDamage[(int)weapon].penetration += (int)upgrade.value;
						break;
					case StatType.range:
						_weaponDamage[(int)weapon].range += (int)upgrade.value;
						break;
					case StatType.durability:
						_weaponDamage[(int)weapon].range += (int)upgrade.value;
						break;
					case StatType.explode:
						_weaponDamage[(int)weapon].explode = (int)upgrade.value;
						break;
				}
			}
		}
	}

	private void WeaponUpgradeMultiply(Upgrade upgrade)
	{
		foreach (WeaponType weapon in upgrade.weaponTypes)
		{
			foreach (StatType stat in upgrade.statTypes)
			{
				switch (stat)
				{
					case StatType.baseDamage:
						_newDmg = _weaponDamage[(int)weapon].baseDamage * upgrade.value;
						_weaponDamage[(int)weapon].baseDamage = (int)_newDmg;
						break;
					case StatType.fireDamage:
						_newDmg = _weaponDamage[(int)weapon].fireDamage * upgrade.value;
						_weaponDamage[(int)weapon].fireDamage = (int)_newDmg;
						break;
					case StatType.iceDamage:
						_newDmg = _weaponDamage[(int)weapon].iceDamage * upgrade.value;
						_weaponDamage[(int)weapon].iceDamage = (int)_newDmg;
						break;
					case StatType.poisonDamage:
						_newDmg = _weaponDamage[(int)weapon].poisonDamage * upgrade.value;
						_weaponDamage[(int)weapon].poisonDamage = (int)_newDmg;
						break;
					case StatType.elecDamage:
						_newDmg = _weaponDamage[(int)weapon].elecDamage * upgrade.value;
						_weaponDamage[(int)weapon].elecDamage = (int)_newDmg;
						break;
					case StatType.armorPiercing:
						_newDmg = _weaponDamage[(int)weapon].armorPiercing * upgrade.value;
						_weaponDamage[(int)weapon].armorPiercing = (int)_newDmg;
						break;
					case StatType.crushingHit:
						_newDmg = _weaponDamage[(int)weapon].crushingHit * upgrade.value;
						_weaponDamage[(int)weapon].crushingHit = (int)_newDmg;
						break;
					case StatType.criticalHit:
						_newDmg = _weaponDamage[(int)weapon].criticalHit * upgrade.value;
						_weaponDamage[(int)weapon].criticalHit = (int)_newDmg;
						break;
					case StatType.stun:
						_newDmg = _weaponDamage[(int)weapon].stun * upgrade.value;
						_weaponDamage[(int)weapon].stun = (int)upgrade.value;
						break;
					case StatType.range:
						_newDmg = _weaponDamage[(int)weapon].range * upgrade.value;
						_weaponDamage[(int)weapon].range = (int)upgrade.value;
						break;
					case StatType.durability:
						_newDmg = _weaponDamage[(int)weapon].durability * upgrade.value;
						_weaponDamage[(int)weapon].range = (int)upgrade.value;
						break;
				}
			}
		}
	}

	private void TrapUpgradeAdd(Upgrade upgrade)
	{
		foreach (TrapType trap in upgrade.trapTypes)
		{
			foreach (StatType stat in upgrade.statTypes)
			{
				switch (stat)
				{
					case StatType.baseDamage:
						_trapDamage[(int)trap].baseDamage += (int)upgrade.value;
						break;
					case StatType.fireDamage:
						_trapDamage[(int)trap].fireDamage += (int)upgrade.value;
						break;
					case StatType.iceDamage:
						_trapDamage[(int)trap].iceDamage += (int)upgrade.value;
						break;
					case StatType.poisonDamage:
						_trapDamage[(int)trap].poisonDamage += (int)upgrade.value;
						break;
					case StatType.elecDamage:
						_trapDamage[(int)trap].elecDamage += (int)upgrade.value;
						break;
					case StatType.armorPiercing:
						_trapDamage[(int)trap].armorPiercing += (int)upgrade.value;
						break;
					case StatType.crushingHit:
						_trapDamage[(int)trap].crushingHit += (int)upgrade.value;
						break;
					case StatType.criticalHit:
						_trapDamage[(int)trap].criticalHit += (int)upgrade.value;
						break;
					case StatType.stun:
						_trapDamage[(int)trap].stun += (int)upgrade.value;
						break;
					case StatType.penetration:
						_trapDamage[(int)trap].penetration += (int)upgrade.value;
						break;
					case StatType.range:
						_trapDamage[(int)trap].range += (int)upgrade.value;
						break;
					case StatType.durability:
						_trapDamage[(int)trap].range += (int)upgrade.value;
						break;
					case StatType.explode:
						_trapDamage[(int)trap].explode = (int)upgrade.value;
						break;
				}
			}
		}
	}

	private void TrapUpgradeMultiply(Upgrade upgrade)
	{
		foreach (TrapType trap in upgrade.trapTypes)
		{
			foreach (StatType stat in upgrade.statTypes)
			{
				switch (stat)
				{
					case StatType.baseDamage:
						_newDmg = _trapDamage[(int)trap].baseDamage * upgrade.value;
						_trapDamage[(int)trap].baseDamage = (int)_newDmg;
						break;
					case StatType.fireDamage:
						_newDmg = _trapDamage[(int)trap].fireDamage * upgrade.value;
						_trapDamage[(int)trap].fireDamage = (int)_newDmg;
						break;
					case StatType.iceDamage:
						_newDmg = _trapDamage[(int)trap].iceDamage * upgrade.value;
						_trapDamage[(int)trap].iceDamage = (int)_newDmg;
						break;
					case StatType.poisonDamage:
						_newDmg = _trapDamage[(int)trap].poisonDamage * upgrade.value;
						_trapDamage[(int)trap].poisonDamage = (int)_newDmg;
						break;
					case StatType.elecDamage:
						_newDmg = _trapDamage[(int)trap].elecDamage * upgrade.value;
						_trapDamage[(int)trap].elecDamage = (int)_newDmg;
						break;
					case StatType.armorPiercing:
						_newDmg = _trapDamage[(int)trap].armorPiercing * upgrade.value;
						_trapDamage[(int)trap].armorPiercing = (int)_newDmg;
						break;
					case StatType.crushingHit:
						_newDmg = _trapDamage[(int)trap].crushingHit * upgrade.value;
						_trapDamage[(int)trap].crushingHit = (int)_newDmg;
						break;
					case StatType.criticalHit:
						_newDmg = _trapDamage[(int)trap].criticalHit * upgrade.value;
						_trapDamage[(int)trap].criticalHit = (int)_newDmg;
						break;
					case StatType.stun:
						_newDmg = _trapDamage[(int)trap].stun * upgrade.value;
						_trapDamage[(int)trap].stun = (int)upgrade.value;
						break;
					case StatType.range:
						_newDmg = _trapDamage[(int)trap].range * upgrade.value;
						_trapDamage[(int)trap].range = (int)upgrade.value;
						break;
					case StatType.durability:
						_newDmg = _trapDamage[(int)trap].durability * upgrade.value;
						_trapDamage[(int)trap].range = (int)upgrade.value;
						break;
				}
			}
		}
	}
}