using Godot;
using System;

public partial class ScoreManager : Node2D
{
	public int totalCoins, coinsThisLevel;
	private GUI myGUI;

	public override void _Ready()
	{
		myGUI = GetNode<GUI>("../../GUI");
	}

	public int AddCoins(int coins){
		
		coinsThisLevel += coins;
		myGUI.ChangeCoinCount(coinsThisLevel);
		return coinsThisLevel;

	}

	public void SetCoins(int coins){

		coinsThisLevel = coins;
		myGUI.ChangeCoinCount(coinsThisLevel);

	}
}
