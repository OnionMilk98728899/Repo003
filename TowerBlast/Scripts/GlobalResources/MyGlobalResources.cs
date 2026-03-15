using Godot;
using System;
using System.Collections.Generic;
using Game.Damage;
using Game.Upgrades;

public partial class MyGlobalResources : Node
{
	public static MyGlobalResources Instance { get; private set; }
	public static int _currentTowerLevels = 2;
	public static int _playerGoldQuantity = 50000;
	public static List<Vector2> _ladderWallTrapLocationList = new List<Vector2>();
	public static List<(Vector2, int)> _enemyPath;
	
	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}

	public void SetPlayerGold(int newgold)
	{
		_playerGoldQuantity += newgold;
	}
	public int GetPlayerGold()
	{
		return _playerGoldQuantity;
	}

}
