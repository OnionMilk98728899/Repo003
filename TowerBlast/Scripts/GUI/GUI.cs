using Godot;
using System;

public partial class GUI : Control
{
	[Export] private TextureButton _buildButton, _upgradeButton, _demolishButton, _weaponShopButton, _exitBuildModeButton, _buildFloorButton, _weaponUpgrade1Button, _back3Button;
	[Export] private UpgradeMenu _upgradeMenu;
	[Export] private BuildModeManager _buildModeManager;
	[Export] private PathfinderManager _pathfinderMan;
	[Export] private WeaponShopMenu _weaponMenu;
	[Export] private RichTextLabel _exitCurrentModeLabel;
	[Export] private AnimationPlayer _guiAnim;
	private RichTextLabel _currentButtonText;
	private Control _buttonParent;
	private TextureButton _selectedButton;
	private bool _isFocusGrabbed, _isGUIActive, _isInMainTier, _isInBuildTier, _isInUpgradeTier, _areWeaponUpgradesGenerated, _isInWeaponStoreTier;
	//public FocusModeEnum FocusMode { get; set; }


	public override void _Ready()
	{
		GlobalSignals.Instance.EnterExitBuildMode += ActivateDeactivateBuildMenu;
		_buttonParent = GetNode<Control>("MainTierButtons");
		_exitCurrentModeLabel.Visible = false;
		_isGUIActive = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isGUIActive)
		{
			GrabInitialButtonFocus();
			ShowButtonLabel(_buttonParent);

		}
		else
		{
			GetViewport().GuiReleaseFocus();
		}

	}

	public void ShowButtonLabel(Node buttonParent)
	{
		foreach (Node child in buttonParent.GetChildren())
		{
			if (child is TextureButton button)
			{
				button.FocusMode = FocusModeEnum.All;
				if (button.HasFocus())
				{
					RevealHideTextForButton(button, true);
				}
				else
				{
					RevealHideTextForButton(button, false);
				}
			}
		}
	}


	private void RevealHideTextForButton(TextureButton button, bool isVisible)
	{
		foreach (Node child in button.GetChildren())
		{
			if (child is RichTextLabel text)
			{
				text.Visible = isVisible;
			}
		}
	}


	private void ActivateDeactivateBuildMenu(bool isActive)
	{

	}

	private void GrabInitialButtonFocus()
	{
		if (!_isFocusGrabbed)
		{
			_buildButton.GrabFocus();
			_isFocusGrabbed = true;
		}
	}

	private void OnBuildButtonPressed()
	{
		_guiAnim.Play("EnterBuildTier");
		_buildFloorButton.GrabFocus();
	}

	private void OnUpgradeButtonPressed()
	{
		_guiAnim.Play("EnterUpgradeTier");
		_weaponUpgrade1Button.GrabFocus();
		_upgradeMenu.PopulateUpgrades();
	}
	private void OnDemolishButtonPressed()
	{

	}
	private void OnWeaponShopButtonPressed()
	{
		_guiAnim.Play("EnterWeaponShopTier");
		_back3Button.GrabFocus();
		if (!_areWeaponUpgradesGenerated)
		{
			_weaponMenu.RandomizeUpgradeChoices();
		}
	}

	private void OnExitBuildModeButtonPressed()
	{
		GlobalSignals.Instance.EmitSignal("EnterExitBuildMode", false);
		_pathfinderMan._isPathfinderActive = true;
		_isGUIActive = false;
		ResetStatsForNewWave();

	}

	private void ResetStatsForNewWave()
	{
		_areWeaponUpgradesGenerated = false;
	}

}
