using Godot;
using System;

public partial class PitcherSystem : Control
{
	[Export] private int maxJuiceLevel;
	[Export] private AnimationPlayer pitcherAnim, fxAnim;
	[Export] private Sprite2D pitcherSprite;
	[Export] private TextureProgressBar fillLevelBar;
	[Export] private RichTextLabel fillLabel;
	[Export] private CupSystem cupSystem;
	[Export] private Timer fillTimer;
	public int juiceLevel, newJuiceLevel;
	private int fillOffset, matchFillToFrame;
	private float augmenter;
	public bool pourTriggered;
	private bool pourEntered, pourExited = true;
	public override void _Ready()
	{
		matchFillToFrame = 0;

	}
	public override void _PhysicsProcess(double delta)
	{
		FillProgressBarVisually();
		OffsetVisualFillLevel();
		fillLevelBar.Value = juiceLevel;
		fillLabel.Text = juiceLevel.ToString();

		if (pourTriggered)
		{
			if (juiceLevel > 0)
			{
				pourExited = false;
				juiceLevel -= 1;
				if (!pourEntered)
				{
					pourEntered = true;
					pitcherAnim.Play("PitcherPourEnter1");
				}
			}
			else
			{
				GD.PrintErr("No Juice in Pitcher!");
			}

		}
		else
		{
			pourEntered = false;
			if (!pourExited)
			{
				pitcherAnim.Play("PitcherPourExit1");
				pourExited = true;
			}
			if (pourExited && !pitcherAnim.IsPlaying()) { pitcherSprite.Frame = matchFillToFrame; }
		}

	}

	public void AddJuiceToPitcher(int juiceUnits)
	{
		fillTimer.Start();
		newJuiceLevel += juiceUnits;
		if (newJuiceLevel > maxJuiceLevel)
		{
			newJuiceLevel = maxJuiceLevel;
		}
		if (newJuiceLevel < 0)
		{
			newJuiceLevel = 0;
		}
	}

	private void FillProgressBarVisually()
	{
		if (!fillTimer.IsStopped())
		{
			if (juiceLevel < newJuiceLevel)
			{
				juiceLevel += 2;
			}


		}
	}

	private void OffsetVisualFillLevel()
	{
		if (juiceLevel >= 500)
		{
			augmenter = -1.3f;
		}
		else
		{
			augmenter = 1;
		}
		fillOffset = (int)(augmenter * (.3 * MathF.Abs(juiceLevel - 500)));
		fillLevelBar.Value = juiceLevel + fillOffset;
	}

	private void GetNextCupToFill()
	{

	}

	public int GetJuiceLevel()
	{
		return juiceLevel;
	}

}
