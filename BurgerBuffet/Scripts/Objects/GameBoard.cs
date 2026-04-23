using Godot;
using System;

public partial class GameBoard : Node2D
{

	[Export] private AnimationPlayer _boardAnim;
	[Export] private Sprite2D _boardSprite;

	public override void _Ready()
	{
		GlobalSignals.Instance.InitiateSpecialTime += OnSpecialTImeEntered;
		GlobalSignals.Instance.ResumeNormalMode += OnNormalModeResumed;
		GlobalSignals.Instance.GameOver += OnNormalModeResumed;
	}

	private void OnSpecialTImeEntered()
	{
		_boardAnim.Play("ToSpecial");
	}

	public void RemainInSpecialMode()
	{
		_boardSprite.Frame = 4;
	}

	private void OnNormalModeResumed()
	{
		_boardAnim.Play("ToNormal");
	}

	private void OnBoardBoundaryEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GameOver);
			AudioManager.Instance.PlaySFX(AudioManager.Instance._sfxPlayer, AudioManager.Instance._audioLibrary.deathCrash);
		}
	}
}



