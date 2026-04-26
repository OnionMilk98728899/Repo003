using Godot;
using System;

public partial class GameLoader : Control
{
	public static GameLoader Instance { get; private set; }
	private string _targetScene;
	private Godot.Collections.Array _progressArray = new Godot.Collections.Array();
	private bool _isLoading = false;

	public override void _EnterTree()
	{
		if (Instance == null) { Instance = this; }
		else { QueueFree(); }
	}

	public void LoadScene(string path)
	{
		if (_isLoading)
			return;

		_targetScene = path;
		_isLoading = true;

		//ShowLoadingScreen();

		ResourceLoader.LoadThreadedRequest(_targetScene);
	}

	public override void _Process(double delta)
	{
		if (!_isLoading)
			return;

		ResourceLoader.ThreadLoadStatus status =
			ResourceLoader.LoadThreadedGetStatus(_targetScene, _progressArray);

		float progress = 0f;
		if (_progressArray.Count > 0)
			progress = (float)_progressArray[0];

		//UpdateLoadingUI(progress);

		if (status == ResourceLoader.ThreadLoadStatus.Loaded)
		{
			PackedScene packedScene =
				(PackedScene)ResourceLoader.LoadThreadedGet(_targetScene);

			Node newScene = packedScene.Instantiate();

			SceneTree tree = GetTree();

			if (tree.CurrentScene != null)
				tree.CurrentScene.QueueFree();

			tree.Root.AddChild(newScene);
			tree.CurrentScene = newScene;

			//HideLoadingScreen();

			_isLoading = false;

			// Notify systems (your previous signal idea)
			GlobalSignals.Instance.EmitSignal(
				GlobalSignals.SignalName.SceneReady, newScene);
		}
	}

	// private void ShowLoadingScreen()
	// {
	//     // instantiate or enable UI
	// }

	// private void HideLoadingScreen()
	// {
	//     // remove UI
	// }

	// private void UpdateLoadingUI(float progress)
	// {
	//     // progress bar, etc
	// }
}
