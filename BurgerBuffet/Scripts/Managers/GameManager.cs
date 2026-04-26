using Godot;
using System;

public partial class GameManager : Node2D
{
	public static GameManager Instance { get; private set; }
	private string _mainScene = "res://Scenes/main_scene.tscn", _loadingScene = "res://Scenes/loading_scene.tscn",
	_introScene = "res://Scenes/intro_scene.tscn";
	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}

	public override void _Ready()
	{
		GameLoader.Instance.LoadScene(_loadingScene);
		
	}

	public void OnLoadSceneFinished()
	{
		GameLoader.Instance.LoadScene(_introScene);

	}

	public void OnIntroSceneFinished()
	{
		GameLoader.Instance.LoadScene(_mainScene);
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.OnMainSceneStarted);
	}

}
