using Godot;
using System;

public partial class LoadingScene : Node2D
{
	[Export] private AnimationPlayer _sceneAnim;
	//[Export] private Sprite2D _godotScreenSprite;
	public override void _Ready()
	{
		_sceneAnim.Play("OnionMilkScreen");
		AudioManager.Instance.PlayMusic(AudioManager.Instance._audioLibrary.onionScreen);
	}

	public void OnOnionScreenFinished()
	{
		_sceneAnim.Play("GodotScreen");
		AudioManager.Instance.PlayMusic(AudioManager.Instance._audioLibrary.godotScreen);
		
	}

	public void OnGodotScreenFinished()
	{
		GameManager.Instance.OnLoadSceneFinished();
		
	}
}
