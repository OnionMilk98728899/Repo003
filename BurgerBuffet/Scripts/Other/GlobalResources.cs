using Godot;

public partial class GlobalResources : Node
{
	public static GlobalResources Instance { get; private set; }
	[Export] public Timer _specialTimer;
	public enum gameState
	{
		normal, special
	}
	public gameState _currentGameState;



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

	public void TriggerSpecialTime()
	{
		_currentGameState = gameState.special;
		_specialTimer.Start();
	}
	private void OnSpecialTimerTimeout()
	{
		_currentGameState = gameState.normal;
	}
}
