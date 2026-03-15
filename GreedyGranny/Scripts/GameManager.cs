using Godot;
using System;

public partial class GameManager : Node2D
{
	private ScoreManager scoreMan;
	private SlotInterface slotInterface;
	private int playerLevelScore;
	

	public override void _Ready()
	{
		scoreMan = GetNode<ScoreManager>("ScoreManager");
		slotInterface = GetNode<SlotInterface>("SlotInterface");

		//Engine.TimeScale = .5;
	}
	// public int GetLevelScore(){

	// 	playerLevelScore = scoreMan.coinsThisLevel;
	// 	return playerLevelScore;
	// }

	// public void SetLevelScore(){

	// }

	private void OnSlotsGetScore()
	{
		slotInterface.coinsThisLevel = scoreMan.coinsThisLevel;
	}


	private void OnSlotsSetScore(int coins)
	{
		scoreMan.SetCoins(coins);
	}
}



