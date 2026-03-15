using Godot;
using System;

public partial class SlotInterface : Control
{
	[Signal] public delegate void SetScoreEventHandler(int coins);
	[Signal] public delegate void GetScoreEventHandler();

	public int coinsThisLevel;
	private PlayerCam playerCam;
	private AnimationPlayer myAnim;
	private FruityLooty myFruityLooty;

	public override void _Ready(){
		playerCam = GetNode<PlayerCam>("../../PlayerCam");
		myAnim =  GetNode<AnimationPlayer>("MyAnim");
		myFruityLooty = GetNode<FruityLooty>("FruityLooty");
	}

	public override void _PhysicsProcess(double delta){
	   
		Position = playerCam.Position;

	}

	public void ActivateDeactivateGame(bool active){

		if(active){
			myAnim.Play("GodotScreenSlideIn");
			myFruityLooty.GetButtonFocus();
			myFruityLooty.GetCoinValues();

		}else{
			myAnim.Play("GodotScreenSlideOut");
		}

	}

	public int GetCoinsThisLevel(){
		EmitSignal("GetScore");
		return coinsThisLevel;
	}

	public void SetCoinsThisLevel(int coins){
		EmitSignal("SetScore", coins);
		coinsThisLevel = coins;
	}

	private void OnFruityLootyQuitMinigame()
	{
		ActivateDeactivateGame(false);
	}
}



