using Godot;
using System;

public partial class GUI : Control
{
	private int coinCount;
	private RichTextLabel coinCountText;
	private PlayerCam playerCam;
	private granny myGranny;
	private Vector2 targetPosition;

	public override void _Ready()
	{
		playerCam = GetNode<PlayerCam>("../PlayerCam");
		myGranny = GetNode<granny>("../Granny");
		coinCountText = GetNode<RichTextLabel>("PlayerStats/CoinCountText");

	}

	public override void _PhysicsProcess(double delta)
	{
		targetPosition = playerCam.anchorPosition;
		// targetPosition.X = Mathf.Round(playerCam.GlobalPosition.X);
		// targetPosition.Y = Mathf.Round(playerCam.GlobalPosition.Y);
		GlobalPosition = targetPosition;
		coinCountText.Text = "\t" + coinCount.ToString();
	}

	public void ChangeCoinCount(int coins){
		coinCount = coins;
	}



}
