using Godot;
using System;

public partial class IntroScene : Node2D
{
	[Export] private AnimationPlayer _introAnim;
	public override void _Ready()
	{
		_introAnim.Play("IntroScreen");
		AudioManager.Instance.PlayMusic(AudioManager.Instance._audioLibrary.introTrack);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			GameManager.Instance.OnIntroSceneFinished();
		}
	}


}
