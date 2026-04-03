using Godot;

public partial class GlobalResources : Node
{
	public static GlobalResources Instance { get; private set; }
	[Export] public Timer _specialTimer;
	public enum gameState
	{
		normal, special, gameOver
	}
	public gameState _currentGameState;

	private int _burgerCount, _score, _gamePhase;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		GlobalSignals.Instance.GameOver += TriggerGameOver;
		GlobalSignals.Instance.RestartGame += TriggerRestart;
		GlobalSignals.Instance.InitiateSpecialTime += AddTimeToSpecialTimeAndExecute;
		//AudioManager.Instance.PlayMusic(AudioManager.Instance._audioLibrary.mainTheme);
	}

	public void TriggerGameOver()
	{
		_currentGameState = gameState.gameOver;
	}

	public void TriggerRestart()
	{
		_currentGameState = gameState.normal;
		
	}
	public void SetScore(int points)
	{
		_score += points;
	}
	public int GetScore()
	{
		return _score;
	}

	public int GetBurgerCount()
	{
		return _burgerCount;
	}
	public void SetBurgerCount(int count)
	{
		_burgerCount += count;
	}

	public int GetSpecialTime()
	{
		return (int)_specialTimer.TimeLeft;
	}
	public void AddTimeToSpecialTimeAndExecute(double time)
	{
		if (_specialTimer.IsStopped())
		{
			_currentGameState = gameState.special;
			_specialTimer.Start();
		}
		else
		{
			double newTime = _specialTimer.TimeLeft + time;
			_specialTimer.Start(newTime);
		}
	}

	public void ResetGameValues()
	{
		_burgerCount = 0;
		_specialTimer.Stop();
		_score = 0;
	}
	private void OnSpecialTimerTimeout()
	{
		_currentGameState = gameState.normal;
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.ResumeNormalMode);
	}
}
