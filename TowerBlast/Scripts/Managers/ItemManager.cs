using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public partial class ItemManager : Node2D
{
	public static ItemManager Instance { get; private set; }
	//[Export] private Timer _consolidateTimer;
	[Export] private PackedScene _coinScene;
	[Export] private TileMap _tileMap;
	private Gold _coin;
	//private Vector2 _thisCoinPos, _lastCoinPos;
	//private List<Gold> _currentGoldStack = new List<Gold>();


	public override void _Ready()
	{
		Instance = this;
		///Gold.CoinFellEventHandler += OnGoldFell;
		GlobalSignals.Instance.CoinFellFlat += OnGoldFell;
	}


	private void OnGoldFell(Vector2 goldPos, Gold sourceCoin)
	{
		StackGoldPiece(goldPos, sourceCoin);
	}


	private void StackGoldPiece(Vector2 goldPos, Gold sourceCoin)
	{
		
		int xCoordMin = (int)goldPos.X/16*16;
		int xCoordMax = xCoordMin +16;
		int yCoordMax = (int)goldPos.Y/16*16;
		int yCoordMin = yCoordMax - 16;
		bool stackFound = false;

		
		foreach(Node child in GetChildren())
		{
			if(child is Gold coin)
			{
				// GD.Print( " My range is (" + xCoordMin + "-" + xCoordMax + "),(" + yCoordMin + "-" + yCoordMax+ ")");
				// GD.Print($"This coin is {coin.GetCoinPosition()}");
				if(coin.GetCoinPosition().X > xCoordMin && coin.GetCoinPosition().X <= xCoordMax &&
				coin.GetCoinPosition().Y > yCoordMin && coin.GetCoinPosition().Y <= yCoordMax &&
				coin.GetState() == Gold._state.stacked && coin != sourceCoin)
				{
					GD.Print("Found Stack");
					stackFound = true;
					coin.AddAGoldPieceToStack();
					sourceCoin.QueueFree();
				}
			}
			if (!stackFound)
			{
				sourceCoin.BeginNewStack();	
			}
		}
	}

}
