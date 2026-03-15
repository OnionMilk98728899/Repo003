using Godot;
using System;
using Game.Upgrades;


public partial class TrapMenuManager : Control
{

	[Signal] public delegate void BuildRangedTrapEventHandler(int trapNum);
	[Signal] public delegate void BuildStaticTrapEventHandler(int trapNum);
	[Signal] public delegate void UpgradeRangedTrapEventHandler(int trapNum);
	[Signal] public delegate void UpgradeStaticTrapEventHandler(int trapNum);
	[Export] private int _ballistaCost, _cannonCost, _spikeCost, _swarmCost, _magicOrbCost, _treasureCost, _spearCost, _shurikenCost, _triCannonCost, _bigCannonCost,
	_fireOrbCost, _iceOrbCost, _poisonOrbCost, _elecOrbCost, _fireTileCost, _iceTileCost, _poisonTileCost, _elecTileCost, _ratTrapCost, _bugTrapCost; 
	private RangedTrap _myRanged;
	[Export] private PackedScene _rangedTrapScene;

	private void OnBallistaPressed()
	{

		if (CheckGoldForTrapFunds(_ballistaCost))
		{
			EmitSignal(SignalName.BuildRangedTrap, (int)TrapType.ballista);
		}

		

		// _myRanged = _rangedTrapScene.Instantiate<RangedTrap>();
		// _myRanged.GlobalPosition = new Vector2(104,-16);
		// AddChild(_myRanged);

	}

	private void OnCannonPressed()
	{
		if (CheckGoldForTrapFunds(_cannonCost))
		{
			EmitSignal(SignalName.BuildRangedTrap, (int)TrapType.cannon);

		}
		
	}
	private void OnMagicOrbPressed()
	{

			if (CheckGoldForTrapFunds(_magicOrbCost))
		{
			EmitSignal(SignalName.BuildRangedTrap, (int)TrapType.magic);
		}
		
	}

	private void OnSpikesPressed(){
				if (CheckGoldForTrapFunds(_spikeCost))
		{
			EmitSignal(SignalName.BuildStaticTrap, (int)TrapType.spikes);
		}
		
	} 


	private void OnSwarmPressed()
	{
				if (CheckGoldForTrapFunds(_swarmCost))
		{
			EmitSignal(SignalName.BuildStaticTrap, (int)TrapType.swarm);
		}
		
	}

	private void OnGoldChestPressed()
	{
				if (CheckGoldForTrapFunds(_treasureCost))
		{
			EmitSignal(SignalName.BuildStaticTrap, (int)TrapType.treasure);
		}
		
	}


	private void OnSpearPressed()
	{
				if (CheckGoldForTrapFunds(_spearCost))
		{
			EmitSignal(SignalName.UpgradeRangedTrap, (int)TrapType.spear);
		}
		
	}


	private void OnShurikenPressed()
	{
				if (CheckGoldForTrapFunds(_shurikenCost))
		{
			EmitSignal(SignalName.UpgradeRangedTrap, (int)TrapType.shuriken);
		}
	}


	private void OnBigCannonPressed()
	{
				if (CheckGoldForTrapFunds(_bigCannonCost))
		{
			EmitSignal(SignalName.UpgradeRangedTrap, (int)TrapType.bigcannon);
	}
		}
		


	private void OnTriCannonPressed()
	{
				if (CheckGoldForTrapFunds(_triCannonCost))
		{
			EmitSignal(SignalName.UpgradeRangedTrap, (int)TrapType.tricannon);
		}
		
	}

	private void OnFireTilePressed()
	{
				if (CheckGoldForTrapFunds(_fireTileCost))
		{
			EmitSignal(SignalName.UpgradeStaticTrap, (int)TrapType.firetile);
		}
		
	}

	private void OnIceTilePressed()
	{
				if (CheckGoldForTrapFunds(_iceTileCost))
		{
			EmitSignal(SignalName.UpgradeStaticTrap, (int)TrapType.icetile);
		}
		

	}

	private void OnElectricTilePressed()
	{
				if (CheckGoldForTrapFunds(_elecTileCost))
		{
			EmitSignal(SignalName.UpgradeStaticTrap, (int)TrapType.lightningtile);
		}
		

	}

	private void OnPoisonTilePressed()
	{
				if (CheckGoldForTrapFunds(_poisonTileCost))
		{
			EmitSignal(SignalName.UpgradeStaticTrap, (int)TrapType.poisontile);
		}
		
	}

	private void OnFireOrbPressed()
	{
				if (CheckGoldForTrapFunds(_fireOrbCost))
		{
			EmitSignal(SignalName.UpgradeRangedTrap, (int)TrapType.fireorb);
		}
		
	}

	private void OnIceOrbPressed()
	{
				if (CheckGoldForTrapFunds(_iceOrbCost))
		{
			EmitSignal(SignalName.UpgradeRangedTrap, (int)TrapType.iceorb);
		}
		
	}

	private void OnElectricOrbPressed()
	{
				if (CheckGoldForTrapFunds(_elecOrbCost))
		{
			EmitSignal(SignalName.UpgradeRangedTrap, (int)TrapType.lightningorb);
		}
		
	}

	private void OnPoisonOrbPressed()
	{	
				if (CheckGoldForTrapFunds(_poisonOrbCost))
		{
			EmitSignal(SignalName.UpgradeRangedTrap, (int)TrapType.poisonorb);
		}
		
	}

	private void OnRatSwarmPressed()
	{
				if (CheckGoldForTrapFunds(_ratTrapCost))
		{
			EmitSignal(SignalName.UpgradeStaticTrap, (int)TrapType.ratswarm);
		}
		
	}

	private void OnBugSwarmPressed()
	{
				if (CheckGoldForTrapFunds(_bugTrapCost))
		{
			EmitSignal(SignalName.UpgradeStaticTrap, (int)TrapType.bugswarm);
		}
		
	}

	private bool CheckGoldForTrapFunds(int value)
	{
		if(MyGlobalResources._playerGoldQuantity >= value)
		{
			MyGlobalResources._playerGoldQuantity -= value;
			return true;
		}
		else
		{
			return false;
		}
	}
}



