using Godot;

public partial class GlobalSignals : Node
{
    public static GlobalSignals Instance { get; private set; }
	[Signal] public delegate void GameOverEventHandler();
	[Signal] public delegate void GenerateNewOrderEventHandler();
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
