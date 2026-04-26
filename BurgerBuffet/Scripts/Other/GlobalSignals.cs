using System.ComponentModel;
using Godot;

public partial class GlobalSignals : Node
{
    public static GlobalSignals Instance { get; private set; }
	[Signal] public delegate void RestartGameEventHandler();
	[Signal] public delegate void GameOverEventHandler();
	[Signal] public delegate void GenerateNewOrderEventHandler();
	[Signal] public delegate void InitiateSpecialTimeEventHandler();
	[Signal] public delegate void ResumeNormalModeEventHandler();
	[Signal] public delegate void AddTimeToSpecialTimeEventHandler(double time);
	[Signal] public delegate void SceneReadyEventHandler(Node scene);
	[Signal] public delegate void OnMainSceneStartedEventHandler();
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
}
