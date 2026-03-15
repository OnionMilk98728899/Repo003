using Godot;
using System;

public partial class WeaponShopMenu : Control
{
	[Export] private Texture2D _swordButton, _daggerButton, _shieldButton, _spearButton, _shurikenButton, _bowButton, _axeButton, _hammerButton,
	 _clubButton, _scepterButton, _potionButton, _bombButton; 
	[Export] private TextureButton _buildButton,_weaponSelect1, _weaponSelect2, _weaponSelect3, _backButton3;
	[Export] private AnimationPlayer _guiAnim;
	[Export] private GUI myGUI;
	public override void _PhysicsProcess(double delta)
	{
		myGUI.ShowButtonLabel(this);
	}

	public void RandomizeUpgradeChoices()
	{
		int randR =  GD.RandRange(1,12);

		//this is where we should draw the upgrade from the JSON file
	}

	private void OnWeaponSelect1Pressed()
	{

	}
	private void OnWeaponSelect2Pressed()
	{

	}

	private void OnWeaponSelect3Pressed()
	{

	}

	private void OnBackButton3Pressed()
	{
		_guiAnim.Play("ExitWeaponShopTier");
		_buildButton.GrabFocus();
	}
}
