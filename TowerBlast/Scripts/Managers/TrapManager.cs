using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Upgrades;

public partial class TrapManager : Control
{
	[Signal] public delegate void ExitTrapBuildingModeEventHandler(bool isActive);
	[Export]
	private TextureButton _ballistaButton, _cannonButton, _spikesButton, _magicOrbButton, _swarmButton, _spearButton,
	_shurikenButton, _bigCannonButton, _triCannonButton, _fireTileButton, _iceTileButton, _poisonTileButton, _elecTileButton,
	_fireOrbButton, _iceOrbButton, _elecOrbButton, _poisonOrbButton, _ratSwarmButton, _bugSwarmButton, _treasureChestButton, _warpDoorButton;
	[Export] private TrapMenuManager _trapMenuMan;
	[Export] private TrapParent _myTrapParent;
	[Export] private BuildMenu _myBuildMenu;
	[Export] private PackedScene _rangedTrapScene, _staticTrapScene;
	[Export] private Sprite2D[] _trapIcons;
	[Export] private Sprite2D _selectionCursor;
	private Sprite2D _spriteToAdd;
	private List<TextureButton> _availableTraps = new List<TextureButton>(),
	_availableBallistaUpgrades = new List<TextureButton>(), _availableCannonUpgrades = new List<TextureButton>(),
	 _availableOrbUpgrades = new List<TextureButton>(), _availableSpikeUpgrades = new List<TextureButton>(),
	 _availableSwarmUpgrades = new List<TextureButton>(), _buttonListToShow;
	private RangedTrap _myRangedTrap, _rangedTrapToReplace;
	private StaticTrap _myStaticTrap, _staticTrapToReplace;
	private Vector2 _trapPosition, _trapIconPosition, _TRAPICONOFFSET = new Vector2(14, 191), _TRAPPRINTOFFSET = new Vector2(38, 230);
	private bool _isTrapBuyingMenuActive, _isTrapAlreadyExisting;
	// public enum TrapType
	// {
	// 	ballista, spear, shuriken, cannon, bigcannon, tricannon, magic, fireorb,
	// 	iceorb, lightningorb, poisonorb, spikes, firetile, icetile, poisontile,
	// 	lightningtile, swarm, bugswarm, ratswarm, treasure, warpdoor
	// }
	public override void _Ready()
	{
		GlobalSignals.Instance.DisableTrapMenu += DisableTrapBuyingMenu;
		_availableTraps.Add(_ballistaButton);
		_availableTraps.Add(_cannonButton);
		_availableTraps.Add(_spikesButton);
		_availableTraps.Add(_magicOrbButton);
		_availableTraps.Add(_swarmButton);
		_availableTraps.Add(_treasureChestButton);
		_availableSpikeUpgrades.Add(_fireTileButton);
		_availableSpikeUpgrades.Add( _iceTileButton);
		_availableSpikeUpgrades.Add(_elecTileButton);
		_availableSpikeUpgrades.Add(_poisonTileButton);
		_availableOrbUpgrades.Add(_fireOrbButton);
		_availableOrbUpgrades.Add(_iceOrbButton);
		_availableOrbUpgrades.Add(_elecOrbButton);
		_availableOrbUpgrades.Add(_poisonOrbButton);
		_availableBallistaUpgrades.Add(_spearButton);
		_availableBallistaUpgrades.Add(_shurikenButton);
		_availableCannonUpgrades.Add(_bigCannonButton);
		_availableCannonUpgrades.Add(_triCannonButton);
		_availableSwarmUpgrades.Add(_bugSwarmButton);
		_availableSwarmUpgrades.Add(_ratSwarmButton);
		
	}


	public override void _PhysicsProcess(double delta)
	{
		if (_isTrapBuyingMenuActive)
		{

		}
	}

	public void ActivateTrapBuyingMenu(bool isActive, Vector2 cursorPos)
	{
		//GD.Print("Activated!!!!!!!!!!!!");
		_isTrapBuyingMenuActive = isActive;
		DetectTrapTypeUnderCursorPosition(cursorPos);
		ShowAvailableTrapIcons(cursorPos, _buttonListToShow);
		
	}

	private void DisableTrapBuyingMenu(bool isActive)
	{
		_isTrapBuyingMenuActive = isActive;
		HideTrapIcons();
	}

	private void DetectTrapTypeUnderCursorPosition(Vector2 cursorPos)
	{
		_buttonListToShow = _availableTraps;
		_isTrapAlreadyExisting = false;
		foreach (Node child in _myTrapParent.GetChildren())
		{
			if (child is RangedTrap ranged)
			{
				if (ranged.GlobalPosition == cursorPos)
				{
					if (ranged._myTrapType == TrapType.ballista ||ranged._myTrapType == TrapType.spear || ranged._myTrapType == TrapType.shuriken)
					{
						_buttonListToShow = _availableBallistaUpgrades;
					}else if(ranged._myTrapType == TrapType.cannon ||ranged._myTrapType == TrapType.bigcannon ||ranged._myTrapType == TrapType.tricannon)
					{
						_buttonListToShow = _availableCannonUpgrades;
					}else if(ranged._myTrapType == TrapType.magic ||ranged._myTrapType == TrapType.fireorb ||ranged._myTrapType == TrapType.iceorb
					||ranged._myTrapType == TrapType.lightningorb ||ranged._myTrapType == TrapType.poisonorb)
					{
						_buttonListToShow = _availableOrbUpgrades;
					}

					_rangedTrapToReplace = ranged;	
					_isTrapAlreadyExisting = true;
				}
			}
			if (child is StaticTrap stat)
			{
				if (stat.GlobalPosition == cursorPos)
				{
					if (stat._myTrapType == TrapType.spikes ||stat._myTrapType == TrapType.firetile || stat._myTrapType == TrapType.icetile
					 ||stat._myTrapType == TrapType.lightningtile || stat._myTrapType == TrapType.poisontile)
					{
						_buttonListToShow = _availableSpikeUpgrades;
					}else if(stat._myTrapType == TrapType.swarm ||stat._myTrapType == TrapType.bugswarm||stat._myTrapType == TrapType.ratswarm)
					{
						_buttonListToShow = _availableSwarmUpgrades;
					}else if(stat._myTrapType == TrapType.treasure)
					{	
						///_buttonListToShow = _availableBallistaUpgrades;
					}else if (stat._myTrapType == TrapType.warpdoor)
					{
						
					}
					_staticTrapToReplace = stat;
					_isTrapAlreadyExisting = true;
					
				}
			}

		}
	}
	public void AddNewTrapTypeToInventory(TrapType trap)
	{
		switch (trap)
		{
			case TrapType.cannon:
				_availableTraps.Add(_cannonButton);
				break;
			case TrapType.spikes:
				_availableTraps.Add(_spikesButton);
				break;
			case TrapType.magic:
				_availableTraps.Add(_magicOrbButton);
				break;
			case TrapType.swarm:
				_availableTraps.Add(_swarmButton);
				break;
			case TrapType.treasure:
				_availableTraps.Add(_treasureChestButton);
				break;
			case TrapType.warpdoor:
				_availableTraps.Add(_warpDoorButton);
				break;
		}

	}

	public void AddNewTrapToSecondaryInventory(TrapType trap)
	{
		switch (trap)
		{
			case TrapType.spear:
				_availableBallistaUpgrades.Add(_spearButton);
				break;
			case TrapType.shuriken:
				_availableBallistaUpgrades.Add(_shurikenButton);
				break;
			case TrapType.bigcannon:
				_availableCannonUpgrades.Add(_bigCannonButton);
				break;
			case TrapType.tricannon:
				_availableCannonUpgrades.Add(_triCannonButton);
				break;
			case TrapType.fireorb:
				_availableOrbUpgrades.Add(_fireOrbButton);
				break;
			case TrapType.iceorb:
				_availableOrbUpgrades.Add(_iceOrbButton);
				break;
			case TrapType.lightningorb:
				_availableOrbUpgrades.Add(_elecOrbButton);
				break;
			case TrapType.poisonorb:
				_availableOrbUpgrades.Add(_poisonOrbButton);
				break;
			case TrapType.firetile:
				_availableSpikeUpgrades.Add(_fireTileButton);
				break;
			case TrapType.icetile:
				_availableSpikeUpgrades.Add(_iceTileButton);
				break;
			case TrapType.lightningtile:
				_availableSpikeUpgrades.Add(_elecTileButton);
				break;
			case TrapType.poisontile:
				_availableSpikeUpgrades.Add(_poisonTileButton);
				break;
			case TrapType.ratswarm:
				_availableSwarmUpgrades.Add(_ratSwarmButton);
				break;
			case TrapType.bugswarm:
				_availableSwarmUpgrades.Add(_bugSwarmButton);
				break;
		}
	}

	public void ShowAvailableTrapIcons(Vector2 cursorPosition, List<TextureButton> buttonList)
	{

		_trapIconPosition = cursorPosition + _TRAPICONOFFSET;
		_trapPosition = cursorPosition;
		//GD.Print("availableTraps = " + _availableTraps.Count);
		_myBuildMenu.SetTrapSelectMenuActive(true);

		if (_availableTraps.Count > 0)
		{

			foreach (TextureButton trap in buttonList)
			{
				if (trap != null)
				{
					trap.GlobalPosition = _trapIconPosition;
					_trapIconPosition.X += 16;
					trap.Visible = true;
					GD.Print("Printed " + trap + " at " + _trapIconPosition);
				}
			}

			CallDeferred(nameof(DeferredGrabFocus), buttonList[0]);

		}
	}

	private void DeferredGrabFocus(TextureButton button)
	{
		button.GrabFocus();
		//GD.Print("Deferred GrabFocus, HasFocus: " + button.HasFocus());
		CallDeferred(nameof(SetButtonFocusNeighbors));
	}

	private void SetButtonFocusNeighbors()
	{
		int i = 0;
		if (_buttonListToShow.Count > 1)
		{
			foreach (TextureButton trap in _buttonListToShow)
			{
				if (i == 0)
				{
					//GD.Print($"{trap.Name} LEFT neighbor path = {GetPathTo(_availableTraps[_availableTraps.Count - 1])}");
					trap.FocusNeighborLeft = trap.GetPathTo(_buttonListToShow[_buttonListToShow.Count - 1]);
					//GD.Print($"{trap.Name} Right neighbor path = {GetPathTo(_availableTraps[i + 1])}");
					trap.FocusNeighborRight = trap.GetPathTo(_buttonListToShow[i + 1]);
				}
				else if (i == _buttonListToShow.Count - 1)
				{
					trap.FocusNeighborLeft = trap.GetPathTo(_buttonListToShow[i - 1]);
					trap.FocusNeighborRight = trap.GetPathTo(_buttonListToShow[0]);
				}
				else
				{
					trap.FocusNeighborLeft = trap.GetPathTo(_buttonListToShow[i - 1]);
					trap.FocusNeighborRight = trap.GetPathTo(_buttonListToShow[i + 1]);
				}
				i++;
			}
		}

	}

	private void HideTrapIcons()
	{
		foreach (TextureButton trap in _buttonListToShow)
		{
			trap.Visible = false;
		}
	}

	private void InstantiateRangedTrap(TrapType myTrapType)
	{
		_myRangedTrap = _rangedTrapScene.Instantiate<RangedTrap>();
		_myRangedTrap._myTrapType = myTrapType;
		_myRangedTrap.GlobalPosition = _trapPosition;
		_myTrapParent.AddChild(_myRangedTrap);
		//GD.Print("Built Trap successfully");
		GlobalSignals.Instance.EmitSignal("DisableTrapMenu",false);
	}
	private void InstantiateStaticTrap(TrapType myTrapType)
	{
		_myStaticTrap = _staticTrapScene.Instantiate<StaticTrap>();
		_myStaticTrap._myTrapType = myTrapType;
		_myStaticTrap.GlobalPosition = _trapPosition;
		GD.Print("Printed Trap At " + _trapPosition);
		_myTrapParent.AddChild(_myStaticTrap);
		//GD.Print("Built static trap succesfully");
		GlobalSignals.Instance.EmitSignal("DisableTrapMenu",false);

	}
	private void OnBuildRangedTrap(int trap)
	{
		if (!_isTrapAlreadyExisting)
		{
			TrapType myTrap = (TrapType)trap;
			InstantiateRangedTrap(myTrap);
			_isTrapAlreadyExisting = true;
			DisableTrapBuyingMenu(false);
		}
		
	}

	private void OnBuildStaticTrap(int trap)
	{
		if (!_isTrapAlreadyExisting)
		{
			TrapType myTrap = (TrapType)trap;
			InstantiateStaticTrap(myTrap);
			_isTrapAlreadyExisting = true;
			DisableTrapBuyingMenu(false);
		}
		
	}

	private void OnUpgradeRangedTrap(int trap)
	{
		TrapType myTrap = (TrapType)trap;
		_rangedTrapToReplace.QueueFree();
		InstantiateRangedTrap(myTrap);
	}

	private void OnUpgradeStaticTrap(int trap)
	{
		TrapType myTrap = (TrapType)trap;
		_staticTrapToReplace.FreePauseUnpauseTimer();
		_staticTrapToReplace.QueueFree();
		
		InstantiateStaticTrap(myTrap);
	}
}
