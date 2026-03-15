using Godot;
using System;

public partial class BuildMenu : Control
{
	//[Signal] public delegate void DisableTrapMenuEventHandler(bool isActive);
	[Export] private TextureButton _buildButton, _buildFloorButton, _buildLadderButton, _buildTrapButton, _buildWallButton, _backButton;
	[Export] private AnimationPlayer _guiAnim;
	[Export] private BuildModeManager _buildModeMan;
	[Export] private RichTextLabel _exitCurrentModeLabel;
	[Export] private GUI myGUI;
	private bool _isBuildModeActive, _isTrapSelectActive;
	private TextureButton _currentButton;


	public override void _PhysicsProcess(double delta)
	{

		myGUI.ShowButtonLabel(this);
		if (_isBuildModeActive)
		{

			
			if (!_isTrapSelectActive)
			{
				GetViewport().GuiReleaseFocus();
				///GD.Print("Releasing Focus");
				_exitCurrentModeLabel.Visible = true;
			}


			if (Input.IsActionJustPressed("escape"))
			{
				if (_isTrapSelectActive)
				{
					_isTrapSelectActive = false;
					GlobalSignals.Instance.EmitSignal("DisableTrapMenu", _isTrapSelectActive);
				}
				else
				{
					_exitCurrentModeLabel.Visible = false;
					DisableAllUnselectedBuildModes();
					_currentButton.GrabFocus();
					_isBuildModeActive = false;
				}

			}
		}
	}

	private void DisableAllUnselectedBuildModes()
	{
		_buildModeMan._isBuildFloorModeActive = false;
		_buildModeMan._isBuildWallModeActive = false;
		_buildModeMan._isBuildLadderModeActive = false;
		_buildModeMan._isBuildTrapModeActive = false;
		_buildModeMan._isUpgradeModeActive = false;
		_buildModeMan._isDemoModeActive = false;
		_buildModeMan.RepositionBuildModeCursor();
	}

	public void SetTrapSelectMenuActive(bool isActive)
	{
		_isTrapSelectActive = isActive;
	}

	private void OnBuildFloorButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeMan._isBuildFloorModeActive = true;
		_isBuildModeActive = true;
		_currentButton = _buildFloorButton;

	}
	private void OnBuildWallButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeMan._isBuildWallModeActive = true;
		_isBuildModeActive = true;
		_currentButton = _buildWallButton;
	}

	private void OnBuildLadderButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeMan._isBuildLadderModeActive = true;
		_isBuildModeActive = true;
		_currentButton = _buildLadderButton;
	}

	private void OnBuildTrapButtonPressed()
	{
		DisableAllUnselectedBuildModes();
		_buildModeMan._isBuildTrapModeActive = true;
		_isBuildModeActive = true;
		_currentButton = _buildTrapButton;
	}

	private void OnBackButtonPressed()
	{
		_guiAnim.Play("ExitBuildTier");
		_buildButton.GrabFocus();
	}
}
