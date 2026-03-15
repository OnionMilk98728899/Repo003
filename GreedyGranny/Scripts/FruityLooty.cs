using Godot;
using System;

public partial class FruityLooty : Control
{
	[Signal] public delegate void QuitMinigameEventHandler();
	[Export] private PackedScene coinToGenerate;
	//[Export] private Vector2 machinePosition;
	private Sprite2D slot1, slot2, slot3;
	private RichTextLabel coinCount, winLose;
	private AnimationPlayer slotAnim1, slotAnim2, slotAnim3;
	private string winLoseText;
	private TextureButton  spinButton, betButton, quitButton, currentButton;
	private int[] mySlotArray;
	private int ante,  roundWinnings, playerCoins, slotVal1, slotVal2, slotVal3;
	private bool pairFound, pairOfDudsFound;
	private double winMultiplier;
	private float roughWinnings;
	private SlotInterface slotInterface;
	private Timer spinTimer, textTimer;
	private ItemManager itemMan;

	public override void _Ready()
	{
		itemMan = GetNode<ItemManager>("../../../ItemManager");
		slotInterface = GetNode<SlotInterface>("..");
		slot1 = GetNode<Sprite2D>("Slot1");
		slot2 = GetNode<Sprite2D>("Slot2");
		slot3 = GetNode<Sprite2D>("Slot3");
		coinCount = GetNode<RichTextLabel>("CoinCount");
		winLose = GetNode<RichTextLabel>("WinLose");
		slotAnim1 = GetNode<AnimationPlayer>("SlotAnim1");
		slotAnim2 = GetNode<AnimationPlayer>("SlotAnim2");
		slotAnim3 = GetNode<AnimationPlayer>("SlotAnim3");
		spinButton = GetNode<TextureButton>("SpinButton");
		betButton = GetNode<TextureButton>("BetButton");
		quitButton = GetNode<TextureButton>("QuitButton");
		spinTimer = GetNode<Timer>("SpinTimer");
		textTimer = GetNode<Timer>("TextTimer");

		winLoseText = "Let's Play!";
	}

	public override void _PhysicsProcess(double delta)
	{	
		coinCount.Text = playerCoins.ToString();	
		winLose.Text = winLoseText;

		if(spinTimer.TimeLeft > .1){
			if(spinTimer.TimeLeft < 2){slotAnim1.Play("SpinSlot1");}
			if(spinTimer.TimeLeft < 1.8){slotAnim2.Play("SpinSlot2");}
			if(spinTimer.TimeLeft < 1.6){slotAnim3.Play("SpinSlot3");}
		}
		if(spinTimer.TimeLeft <= .1 ){
			slotAnim1.Stop();
			slotAnim2.Stop();
			slotAnim3.Stop();
			slot1.Frame = slotVal1;
			slot2.Frame = slotVal2;
			slot3.Frame = slotVal3;
			
		}
	}

	public void GetButtonFocus(){
		betButton.GrabFocus();
		currentButton = betButton;
	}

	public void SpinSlots(){

		spinTimer.Start();
		slotVal1 = (int)(GD.Randi() % 10);
		slotVal2 = (int)(GD.Randi() % 10);
		slotVal3 = (int)(GD.Randi() % 10);

		CalculateWinnings();
	}

	public void IncreaseWager(){

		if(playerCoins > 0){
			ante++;
			playerCoins -= 1;
			SetCoinValues();
			winLoseText = $"wager:\n{ante}";
		}else{
			textTimer.Start();
			winLoseText = "No more coins!";
		}

	}

	private void CalculateWinnings(){

		winMultiplier = 1;

		if(slotVal1 == slotVal2 || slotVal1 == slotVal3){

			if(slotVal1 < 6){
				winMultiplier = 2;
			}
			if(slotVal1 == 6 || slotVal1 == 7 ){
				winMultiplier = .5;

			}
			if(slotVal1 == 8){
				winMultiplier = 3;
			}
			if(slotVal1 == 9){
				winMultiplier = 4;
			}
			
		}
		if(slotVal2 == slotVal3){

			if(slotVal2 < 6){
				winMultiplier = 2;
			}
			if(slotVal2 == 6 || slotVal2 == 7 ){
				winMultiplier = .5;

			}
			if(slotVal2 == 8){
				winMultiplier = 3;
			}
			if(slotVal2 == 9){
				winMultiplier = 4;
			}
			
		}

		if(slotVal1 == 6 || slotVal2 == 6 || slotVal3 == 6 || slotVal1 == 7 || slotVal2 ==7 || slotVal3 == 7){

			winMultiplier *= .5;

		}

		if(slotVal1 == slotVal2 && slotVal1 == slotVal3){

			if(slotVal1 < 6){
				winMultiplier = 4;
			}
			if(slotVal1 == 6 || slotVal1 == 7 ){
				winMultiplier = .1;
			}
			if(slotVal1 == 8){
				winMultiplier = 6;
			}
			if(slotVal1 == 9){
				winMultiplier = 8;
			}
		}

		// roughWinnings = (float) (ante * winMultiplier);
		// roundWinnings = (int)Mathf.Round(roughWinnings);

		// ante = 0;

		// if(winMultiplier > 1){
		// 	winLoseText = "Winner!\n "+ roundWinnings;
		// }else{
		// 	winLoseText = "Try Again!";
		// }

		// GeneratePayout();
	}

	private void OnSpinButtonPressed()
	{
		if(ante == 0){
			winLoseText = "Place your bet!";
		}else{
			SpinSlots();
		}
		
	}

	private void GeneratePayout(){

		for (int i = 0; i <= roundWinnings; i++)
		{
			collectible_item coin = coinToGenerate.Instantiate<collectible_item>();
			//coin.SetCollisionLayerValue(4, false);
			//coin.collisionDelayTimer.Start();
			coin.Position = new Vector2(slotInterface.Position.X, slotInterface.Position.Y -45);
			coin.ThrowInRandomDirection();

			itemMan.AddChild(coin);
		}


	}

	private void OnBetButtonPressed()
	{
		IncreaseWager();
	}

	private void OnQuitButtonPressed()
	{
		EmitSignal("QuitMinigame");
	}

	public void GetCoinValues(){
		
		playerCoins = slotInterface.GetCoinsThisLevel();
		
	}

	public void SetCoinValues(){

		slotInterface.SetCoinsThisLevel(playerCoins);
	}

	private void OnSpinTimerTimeout()
	{
		roughWinnings = (float) (ante * winMultiplier);
		roundWinnings = (int)Mathf.Round(roughWinnings);

		ante = 0;

		if(winMultiplier > 1){
			winLoseText = "Winner!\n "+ roundWinnings;
		}else{
			winLoseText = "Try Again!";
		}

		GeneratePayout();

	}

	private void OnTextTimerTimeout(){
		winLoseText = $"wager:\n{ante}";
	}
}












