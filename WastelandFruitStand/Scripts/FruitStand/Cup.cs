using Godot;
using System;

public partial class Cup : Control
{
	//[Signal] public delegate void TransmitDirtinessEventHandler(float dirt);
	[Export] private TextureProgressBar fillBar;
	[Export] private Sprite2D cupSprite, meterSprite, meterFXSprite;
	[Export] private Texture2D progBarGreen, progBarRed, recycledTexture, paperTexture, waxTexture, plasticTexture;
	[Export] private Label debugLabel;
	public int sellValue, placementIndex;
	private int fillValue, maxFillValue, initialFillValue, overfill, fillFrame;
	public bool pourTriggered, isPoured;
	private bool statusCalled;
	public enum cupType
	{
		recycled, paper, wax, plastic
	}
	public cupType currentCupType;

	public override void _Ready()
	{
		//initialFillValue = 24;
		fillValue = 0;
		maxFillValue = 65;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (pourTriggered && !isPoured)
		{
			//fillBar.Value += 1;
			FillCup(2);
			AnimateCupMeter();
		}
		else
		{
			if (fillValue >= 50 && fillValue <= 55)
			{
				if (!statusCalled)
				{
					statusCalled = true;
					PerfectCupFill();
					sellValue = 15;
				}
			}
			if (fillValue > 56)
			{
				if (!statusCalled)
				{
					statusCalled = true;
					overfill = fillValue - 55;
					OverfilledCup(overfill);
					sellValue = 7;
				}
			}
			if (fillValue > initialFillValue)
			{
				isPoured = true;
				sellValue = 10;
			}
		}
	}

	public void DetermineCupType(int type)
	{
		switch (type)
		{
			case 0:
				cupSprite.Texture = recycledTexture;
				break;
			case 1:
				cupSprite.Texture = paperTexture;
				break;
			case 2:
				cupSprite.Texture = waxTexture;
				break;
			case 3:
				cupSprite.Texture = plasticTexture;
				break;
		}
	}

	public void FillCup(int juicePoints)
	{
		fillValue += juicePoints;
		fillBar.Value = fillValue;

		if (fillValue > maxFillValue)
		{
			fillValue = maxFillValue;
		}
	}

	private void AnimateCupMeter()
	{
		fillFrame = fillValue / (maxFillValue / 10);
		meterSprite.Frame = fillFrame;
	}

	public void SelectCup()
	{
		cupSprite.Modulate = new Color(1, 0, 1, 1);
	}
	public void DeselectCup()
	{
		cupSprite.Modulate = new Color(1, 1, 1, 1);
	}

	private void PerfectCupFill()
	{
		GD.Print("PERFECT POUR!");
		fillBar.TextureProgress = progBarGreen;
	}

	private void OverfilledCup(int over)
	{
		GD.Print("OVERPOURED BY " + over);
		fillBar.TextureProgress = progBarRed;
		GlobalSignals.Instance.EmitOverfilledCup(over);
	}

	public int SellCup()
	{
		QueueFree();
		return sellValue;
	}
}
