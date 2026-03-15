using Game.Upgrades;
using Godot;
using System;
using System.Collections.Generic;

public partial class UpgradeMenu : Control
{
	[Export]
	private TextureButton _weaponUpgrade1Button, _weaponUpgrade2Button, _weaponUpgrade3Button, _trapUpgrade1Button, _trapUpgrade2Button, _trapUpgrade3Button,
	_reRollbutton, _back2Button, _upgradeButton, _weaponIndexButton1, _weaponIndexButton2;
	[Export]
	private RichTextLabel _weapon1Label, _weapon1Description, _weapon2Label, _weapon2Description, _weapon3Label, _weapon3Description,
	 _trap1Label, _trap1Description, _trap2Label, _trap2Description, _trap3Label, _trap3Description;
	[Export] private AnimationPlayer _guiAnim;
	[Export] private GUI _myGUI;
	[Export] private GameStats _gameStats;
	private List<Upgrade> _weaponUpgradeList = new List<Upgrade>(),
	_tempWeaponUpgradeList = new List<Upgrade>(),
	 _trapUpgradeList = new List<Upgrade>();
	private Upgrade _1stWeaponUpgrade, _2ndWeaponUpgrade, _3rdWeaponUpgrade, _1stTrapUpgrade, _2ndTrapUpgrade, _3rdTrapUpgrade, _upgradeToModify;

	public override void _PhysicsProcess(double delta)
	{
		_myGUI.ShowButtonLabel(this);
	}

	private void OnBackButton2Pressed()
	{
		_guiAnim.Play("ExitUpgradeTier");
		_upgradeButton.GrabFocus();
	}

	private void OnWeapon1ButtonPressed()
	{
		_guiAnim.Play("EnterWeaponIndexSelect");
		_weaponIndexButton1.GrabFocus();
		_upgradeToModify = _1stWeaponUpgrade;
		//PlayerStatistics.Instance.WeaponUpgrade(_1stTrapUpgrade);
	}

	private void OnWeapon2ButtonPressed()
	{
		_guiAnim.Play("EnterWeaponIndexSelect");
		_weaponIndexButton1.GrabFocus();
		_upgradeToModify = _2ndWeaponUpgrade;
	}

	private void OnWeapon3ButtonPressed()
	{
		_guiAnim.Play("EnterWeaponIndexSelect");
		_weaponIndexButton1.GrabFocus();
		_upgradeToModify = _3rdWeaponUpgrade;
	}

	private void OnTrap1ButtonPressed()
	{

	}

	private void OnTrap2ButtonPressed()
	{

	}

	private void OnTrap3ButtonPressed()
	{

	}

	private void OnReRollButtonPressed()
	{

	}

	private void OnWeaponIndexButton1Pressed()
	{
		PlayerStatistics.Instance.WeaponUpgrade(_upgradeToModify, 1);
		_guiAnim.Play("ExitWeaponIndexSelect");
		_back2Button.GrabFocus();
		_gameStats.SetWeaponIcon(1, (int)_upgradeToModify.weaponTypes[0]);
	}

	private void OnWeaponIndexButton2Pressed()
	{
		PlayerStatistics.Instance.WeaponUpgrade(_upgradeToModify, 2);
		_guiAnim.Play("ExitWeaponIndexSelect");
		_back2Button.GrabFocus();
		_gameStats.SetWeaponIcon(2, (int)_upgradeToModify.weaponTypes[0]);
	}

	private Upgrade GetRandomWeaponUpgrade()
	{
		foreach(Upgrade upgrade in UpgradeManager.Instance.GetUpgrades())
		{
			_weaponUpgradeList.Add(upgrade);
		}

		int rand = GD.RandRange(0, _weaponUpgradeList.Count - 1);

		return _weaponUpgradeList[rand];
	}


	// private Upgrade GetRandomWeaponUpgrade()
	// {
	// 	foreach (Upgrade upgrade in UpgradeManager.Instance.GetUpgrades())
	// 	{
	// 		if (upgrade.weaponTypes[0] != WeaponType.none)
	// 		{
	// 			_weaponUpgradeList.Add(upgrade);
	// 			GD.Print("Added weapon to list");
	// 		}
	// 	}

	// 	_tempWeaponUpgradeList.Clear();

	// 	foreach (Upgrade upgrade1 in _weaponUpgradeList)
	// 	{
	// 		if (upgrade1.op != Operation.buy)
	// 		{
	// 			_tempWeaponUpgradeList.Add(upgrade1);
	// 		}
	// 		else
	// 		{
	// 			if ((int)upgrade1.weaponTypes[0] != PlayerStatistics.Instance._currentWeaponIndex.Item1 &&
	// 		 (int)upgrade1.weaponTypes[0] != PlayerStatistics.Instance._currentWeaponIndex.Item2)
	// 			{
	// 				_tempWeaponUpgradeList.Add(upgrade1);
	// 			}
	// 		}
	// 	}

	// 	_weaponUpgradeList.Clear();
	// 	_weaponUpgradeList.AddRange(_tempWeaponUpgradeList);


	// 	int randweapon = GD.RandRange(0, 11);

	// 	_tempWeaponUpgradeList.Clear();

	// 	foreach (Upgrade upgrade in _weaponUpgradeList)
	// 	{
	// 		if ((int)upgrade.weaponTypes[0] == randweapon)
	// 		{
	// 			_tempWeaponUpgradeList.Add(upgrade);
	// 		}
	// 	}

	// 	_weaponUpgradeList.Clear();
	// 	_weaponUpgradeList.AddRange(_tempWeaponUpgradeList);
	// 	int randUpgrade = GD.RandRange(0, _weaponUpgradeList.Count - 1);

	// 	return _weaponUpgradeList[randUpgrade];
	// }

	public void PopulateUpgrades()
	{
		_1stWeaponUpgrade = GetRandomWeaponUpgrade();
		_weapon1Label.Text = $"[center]{_1stWeaponUpgrade.id}[/center]";
		_weapon1Description.Text = _1stWeaponUpgrade.description;
		_2ndWeaponUpgrade = GetRandomWeaponUpgrade();
		_weapon2Label.Text = $"[center]{_2ndWeaponUpgrade.id}[/center]";
		_weapon2Description.Text = _2ndWeaponUpgrade.description;
		_3rdWeaponUpgrade = GetRandomWeaponUpgrade();
		_weapon3Label.Text = $"[center]{_3rdWeaponUpgrade.id}[/center]";
		_weapon3Description.Text = _3rdWeaponUpgrade.description;
	}
}
