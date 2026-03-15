using Godot;
using System;

public partial class CleaningSystem : Control
{
	[Signal] public delegate void RagWipeEventHandler();
	[Export] private GUI gui;
	[Export] private Sprite2D ragSprite;
	[Export] private AnimationPlayer ragAnim;
	[Export] private Label debugLabel;
	[Export] private Timer wipeTimer;
	private bool hasWiped;

	public override void _Ready()
	{

	}


	public override void _PhysicsProcess(double delta)
	{
		debugLabel.Text = gui.GetDirtCoefficient().ToString();
	}

	public void WipeTable()
	{
		if (!hasWiped)
		{
			int random = GD.RandRange(1, 6);
			string animPath = $"Wipe{random}";
			ragAnim.Play(animPath);
			wipeTimer.Start();
			hasWiped = true;
			EmitSignal(SignalName.RagWipe);
		}

	}
	private void OnWipeTimerTimeout()
	{
		hasWiped = false;
	}

}
