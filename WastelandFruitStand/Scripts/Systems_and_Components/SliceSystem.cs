using Godot;
using System;

public partial class SliceSystem : Control
{
	[Signal] public delegate void FruitMessEventHandler(int dirt);
	[Export] private PitcherSystem pitcherSystem;
	[Export] private AnimationPlayer knifeAnim, fruitAnim, fxAnim, mainAnim;
	[Export] private Sprite2D fruitSprite, lineSprite;
	[Export] private Timer fruitTimer;
	private int juicePoints, dirtCoefficient;
	private PlayerInventory playerInventory;
	private PlayerActionController playerAction;
	public bool slicingActive, fruitMoving, fruitBouncing;
	private bool readyToChop;

	public override void _Ready()
	{
		playerInventory = GetNode<PlayerInventory>("/root/TownScene/PlayerPackage/PlayerCharacter/PlayerInventory");
		playerAction = GetNode<PlayerActionController>("/root/TownScene/PlayerPackage/PlayerCharacter/PlayerActionController");

	}

	public override void _PhysicsProcess(double delta)
	{
		if (slicingActive && playerInventory.GetFruitQuantity() > 0)
		{
			PlaySliceGame();
			fruitSprite.Visible = true;
		}
		else
		{
			fruitSprite.Visible = false;
			readyToChop = false;
		}
	}

	public void InitializeSliceGame()
	{
		slicingActive = true;
	}

	private void PlaySliceGame()
	{
		fruitMoving = true;


		if (playerAction.GetAction2Status())
		{
			if (readyToChop)
			{
				knifeAnim.Play("Chop");
				fxAnim.Play("Chop");
				fruitTimer.Start();

				if (!fruitTimer.IsStopped())
				{
					fruitAnim.Pause();
				}
			}
			else
			{
				readyToChop = true;
			}
		}

		if (fruitTimer.IsStopped())
		{
			fruitAnim.Play("Move");
		}
	}

	public void OnKnifeStruckTable()
	{
		dirtCoefficient = 0;
		float distance = lineSprite.GlobalPosition.X - fruitSprite.GlobalPosition.X;
		if (distance <= 10 && distance >= -10)
		{
			if (distance <= 2 && distance >= -2)
			{
				fruitAnim.Play("PerfectSlice");
				juicePoints = 150;
			}
			else
			{
				fruitAnim.Play("PerfectSlice");
				juicePoints = 100;
				dirtCoefficient = 2;
			}
		}
		else if (distance < -10 && distance >= -20)
		{
			fruitAnim.Play("WeakSliceLeft");
			juicePoints = 65;
			dirtCoefficient = 5;
		}
		else if (distance < -20 && distance >= -30)
		{
			fruitAnim.Play("WhiffLeft");
			juicePoints = 0;

		}
		else if (distance > 10 && distance <= 20)
		{
			fruitAnim.Play("WeakSliceRight");
			juicePoints = 65;
			dirtCoefficient = 5;
		}
		else if (distance > 20 && distance <= 30)
		{
			fruitAnim.Play("WhiffRight");
			juicePoints = 20;
		}

		GD.Print("Distance == " + distance);
		mainAnim.Play("FruitStand_Shake");
		pitcherSystem.AddJuiceToPitcher(juicePoints);
		playerInventory.AddFruit(-1);
		EmitSignal(SignalName.FruitMess, dirtCoefficient);
	}
}
