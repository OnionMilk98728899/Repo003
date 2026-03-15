using System;
using Godot;

public partial class GUIold : Control
{
	[Export]
	private TextureButton _buildButton, _buildFloorButton, _buildWallButton,
	_buildLadderButton, _upgradeButton, _buildTrapButton, _demolishButton, _weaponShopButton, _exitBuildModeButton;
	[Export] private BuildModeManager _buildModeManager;
	[Export] private PathfinderManager _pathfinderMan;
	[Export] private RichTextLabel _exitCurrentModeLabel;
	[Export] private AnimationPlayer _guiAnim;
	private TextureButton _selectedButton;
	private bool _isBuildMenuActive, _isFocusGrabbed, _isBuildModeActive, _isInsideSpecificBuildMode, _isDisconnectHandlerConnected;

	public override void _Ready()
	{
		GlobalSignals.Instance.EnterExitBuildMode += ActivateDeactivateBuildMenu;
		_isBuildMenuActive = true;
		_exitCurrentModeLabel.Visible = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isBuildMenuActive)
		{
			GrabInitialButtonFocus();
			NavigateInput();
			if (_isInsideSpecificBuildMode)
			{
				_isFocusGrabbed = false;
				GetViewport().GuiReleaseFocus();
				_exitCurrentModeLabel.Visible = true;
			}
			if (Input.IsActionJustPressed("escape"))
			{

				if (_isInsideSpecificBuildMode)
				{
					_exitCurrentModeLabel.Visible = false;
					DisableAllUnselectedBuildModes();
					_isInsideSpecificBuildMode = false;
					GD.Print("Selected button = " + _selectedButton);
					_buildFloorButton.GrabFocus();
				}
				else
				{
					if (_isBuildModeActive)
					{
						_isBuildModeActive = false;
						if (!_isDisconnectHandlerConnected)
						{
							_guiAnim.AnimationFinished += DelayedButtonGrab;
							_isDisconnectHandlerConnected = true;
						}
						_guiAnim.Play("Exit_BuildMode");
					}
				}
			}
		}
		else
		{
			GD.Print("Focus released");
			GetViewport().GuiReleaseFocus();
		}
	}

	private void ActivateDeactivateBuildMenu(bool isActive)
	{
		_isBuildMenuActive = isActive;
	}

	private void NavigateInput()
	{
		if (Input.IsActionJustPressed("ui_up") || Input.IsActionJustPressed("ui_down"))
		{
			//DisableAllUnselectedBuildModes();
			//GD.Print("Selected Button is  " + _selectedButton.ToString());
		}
	}
	private void GrabInitialButtonFocus()
	{
		if (!_isFocusGrabbed)
		{
			_buildButton.GrabFocus();
			GD.Print($"UpgradeButton: Visible={_buildButton.Visible}, FocusMode={_buildButton.FocusMode}, InTree={_buildButton.IsInsideTree()}");
			_selectedButton = _buildButton;
			_isFocusGrabbed = true;

		}
	}

	private void DelayedButtonGrab(StringName animName)
	{
		if (animName == "Exit_BuildMode")
		{
			_buildButton.Show();
			_buildButton.GrabFocus();

		}
		if (_isDisconnectHandlerConnected)
		{
			_guiAnim.AnimationFinished -= DelayedButtonGrab;
			_isDisconnectHandlerConnected = false;
		}

	}

	private void OnBuildButtonPressed()
	{
		_guiAnim.Play("Enter_BuildMode");
		_buildFloorButton.GrabFocus();
		_selectedButton = _buildFloorButton;
		_isBuildModeActive = true;
	}

	private void OnBuildFloorButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeManager._isBuildFloorModeActive = true;
		_isInsideSpecificBuildMode = true;
		_selectedButton = _buildFloorButton;
	}

	private void OnBuildWallButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeManager._isBuildWallModeActive = true;
		_isInsideSpecificBuildMode = true;
		_selectedButton = _buildWallButton;
	}

	private void OnBuildLadderButtonPressed()
	{

		DisableAllUnselectedBuildModes();
		_buildModeManager._isBuildLadderModeActive = true;
		_isInsideSpecificBuildMode = true;
		_selectedButton = _buildLadderButton;
	}
	private void OnBuildTrapButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeManager._isBuildTrapModeActive = true;
		_isInsideSpecificBuildMode = true;
		_selectedButton = _buildTrapButton;
	}
	private void OnUpgradeButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeManager._isUpgradeModeActive = true;
		_isInsideSpecificBuildMode = true;
	}

	private void OnDemolishButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeManager._isDemoModeActive = true;
		_isInsideSpecificBuildMode = true;
	}

	private void OnExitBuildModeButtonPressed()
	{
		GlobalSignals.Instance.EmitSignal("EnterExitBuildMode", false);
		_pathfinderMan._isPathfinderActive = true;
		_isBuildMenuActive = false;
	}

	private void DisableAllUnselectedBuildModes()
	{
		_buildModeManager._isBuildFloorModeActive = false;
		_buildModeManager._isBuildWallModeActive = false;
		_buildModeManager._isBuildLadderModeActive = false;
		_buildModeManager._isBuildTrapModeActive = false;
		_buildModeManager._isUpgradeModeActive = false;
		_buildModeManager._isDemoModeActive = false;
		_buildModeManager.RepositionBuildModeCursor();
	}


}
