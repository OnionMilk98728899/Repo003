using Godot;
using System;

public partial class GUI : Control
{
	[Export] private CupSystem cupSystem;
	[Export] private CleaningSystem cleaningSystem;
	[Export] private PitcherSystem pitcherSystem;
	[Export] private SliceSystem sliceSystem;
	[Export] private Texture2D pressedButtonTexture, normalTexture, focusedTexture;
	[Export] private RichTextLabel myDisplay1;
	[Export] private AnimationPlayer mainAnim;
	[Export] private Timer clickBuffer, buttonPressTimer;
	[Export] private TextureButton cupsButton, juiceButton, pourButton, cleanButton;
	[Export] private Sprite2D standDirt;
	private TextureButton selectedButton;
	private PlayerInventory playerInventory;
	private PlayerActionController playerAction;
	private bool isPressed, standActive, clickDisabled;
	private float dirtCoefficient;
	private int dirtFrame;
	public override void _Ready()
	{
		GlobalSignals.Instance.OverfilledCup += OnOverfilledCup;
		playerInventory = GetNode<PlayerInventory>("/root/TownScene/PlayerPackage/PlayerCharacter/PlayerInventory");
		playerAction = GetNode<PlayerActionController>("/root/TownScene/PlayerPackage/PlayerCharacter/PlayerActionController");
		selectedButton = cupsButton;
		if (playerAction == null)
		{
			GD.PrintErr("Player refs not found");
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		myDisplay1.Text = "Fruit: " + playerInventory.GetFruitQuantity() + "\t $" + playerInventory.GetCurrencyQuantity();
		if (standActive)
		{
			NavigateButtons();
			SelectCups();
			SellCup();
		}

		MakeStandDirty();

		if (isPressed)
		{
			selectedButton.TextureFocused = pressedButtonTexture;
			selectedButton.TextureNormal = pressedButtonTexture;
		}
		else
		{
			selectedButton.TextureFocused = focusedTexture;
			selectedButton.TextureNormal = normalTexture;
		}
	}

	// public override void _Input(InputEvent @event)
	// {
	// 	if (sliceSystem.slicingActive)
	// 	{
	// 		if (@event.IsActionPressed("Up") || @event.IsActionPressed("Down"))
	// 		{
	// 			GetViewport().SetInputAsHandled();
	// 		}
	// 	}
	// }\

	public override void _UnhandledInput(InputEvent @event)
	{
		if (standActive)
		{
			if (@event.IsActionPressed("Action2") && buttonPressTimer.IsStopped())
			{
				isPressed = true;
				selectedButton.EmitSignal("pressed");
				buttonPressTimer.Start();
			}
		}
	}


	private void SelectCups()
	{
		if (cupSystem.pouringActive && playerAction.GetAction2Status()
		&& !pitcherSystem.pourTriggered && clickBuffer.IsStopped()
		&& selectedButton == pourButton)
		{
			if (!cupSystem.GetNextCupToFill().isPoured)
			{
				cupSystem.GetNextCupToFill().pourTriggered = true;
				pitcherSystem.pourTriggered = true;
				clickBuffer.Start();
			}
			else
			{
				GD.PrintErr("Sorry, Already pored that Cup");
			}

		}
		if (cupSystem.pouringActive && playerAction.GetAction2Status()
		&& pitcherSystem.pourTriggered && clickBuffer.IsStopped()
		 && selectedButton == pourButton)
		{
			cupSystem.GetNextCupToFill().pourTriggered = false;
			pitcherSystem.pourTriggered = false;
		}
	}
	private void SellCup()
	{
		if (playerAction.GetAction3Status())
		{
			if (cupSystem.GetCupCountOnTable() > 0)
			{
				if (cupSystem.GetNextCupToFill().isPoured)                 
				{
					cupSystem.RemoveCupFromList();
					playerInventory.AddCurrency(cupSystem.GetNextCupToFill().SellCup());
					cupSystem.ResetSelectedCupIndex();
					GlobalSignals.Instance.EmitSellCupOfJuice();
				}
			}
			else
			{
				GD.PrintErr("No Cups to sell!");
			}
		}
	}

	private void NavigateButtons()
	{
		if (Input.IsActionJustPressed("ui_up") || Input.IsActionJustPressed("ui_down"))
		{
			selectedButton = GetViewport().GuiGetFocusOwner() as TextureButton;

			if (selectedButton != pourButton)
			{
				cupSystem.pouringActive = false;
			}
			if (selectedButton != juiceButton)
			{
				sliceSystem.slicingActive = false;
			}
		}
	}

	public void ActivateFruitStandInterface(bool active)
	{
		standActive = active;
		if(active)
		{
			mainAnim.Play("FruitStand_Enter");
			cupsButton.GrabFocus();
			selectedButton = GetViewport().GuiGetFocusOwner() as TextureButton;
		}
		else
		{
			mainAnim.Play("FruitStand_Exit");
		}
	}

	private void MakeStandDirty()
	{
		dirtCoefficient += .001f * dirtCoefficient;
		if (dirtCoefficient < 0) { dirtCoefficient = 0; }
		if(dirtCoefficient  > 100){ dirtCoefficient = 100; }

		dirtFrame = (int)MathF.Round(dirtCoefficient / 10);
		if (dirtFrame > 10)
		{
			dirtFrame = 10;
		}

		standDirt.Frame = dirtFrame;
	}

	private void OnSetCupsButtonPressed()
	{
		cupSystem.SetACup();
	}
	private void OnJuiceButtonPressed()
	{
		if(playerInventory.GetFruitQuantity() > 0 && !sliceSystem.slicingActive)
		{
			//playerInventory.AddFruit(-1);
			sliceSystem.InitializeSliceGame();
			sliceSystem.slicingActive = true;
		}
		else if(playerInventory.GetFruitQuantity() <= 0)
		{
			GD.PrintErr("You dont have any fruit!");
		}


	}

	private void OnPourButtonPressed()
	{
		if (!cupSystem.pouringActive)
		{
			cupSystem.GetNextCupToFill().SelectCup();
			clickBuffer.Start();
		}
	}
	private void OnCleanButtonPressed()
	{
		cleaningSystem.WipeTable();
	}

	private void OnOverfilledCup(int overfill)
	{
		dirtCoefficient += (float)overfill;
	}

	public float GetDirtCoefficient()
	{
		return dirtCoefficient;
	}

	private void OnClickBufferTimeout()
	{
		// No action necessary on timeout
	}

	private void OnSlicedFruitMessMade(int spillage)
	{
		dirtCoefficient += spillage;
	}

	private void OnRagWipe()
	{
		dirtCoefficient -=15;
	}

	private void OnButtonPressTimerTimeout()
	{
		isPressed = false;
	}

}
