using System.Reflection.Metadata.Ecma335;
using Godot;

public partial class GlobalResources : Node
{
	public static GlobalResources Instance { get; private set; }
	[Export] public Timer _specialTimer;
	[Export] private PackedScene _popUpScene;
	private Popup _myPopUp;
	public enum gameState
	{
		normal, special, gameOver
	}
	public gameState _currentGameState;

	private int _burgerScore, _tempBurgerScore, _score, _tempScore, _gamePhase, _burgerStrikes;
	private float _multiplier;
	private double _specialTime;
	private bool _isCountingBurger, _isCountingScore;

	public override void _EnterTree()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}

		Instance = this;
	}

	public override void _Ready()
	{
		GlobalSignals.Instance.GameOver += TriggerGameOver;
		GlobalSignals.Instance.RestartGame += TriggerRestart;
		GlobalSignals.Instance.InitiateSpecialTime += ActivateSpecialTime;
		GlobalSignals.Instance.OnMainSceneStarted += OnMainSceneStarted;
	}

	public void OnMainSceneStarted()
	{
		AudioManager.Instance.PlayRandomMusicTrack();
		_multiplier = 1;
	}
	public override void _PhysicsProcess(double delta)
	{
		CallCountdownSounds();
		if (_isCountingScore)
		{
			if (_tempScore < _score)
			{
				_tempScore += 1;
				if (_tempScore % 50 != 0) { AudioManager.Instance.PlaySFX(AudioManager.Instance._scoreSFX, AudioManager.Instance._audioLibrary.coinTick); }
			}
		}

	}
	public void GeneratePopUp(Vector2 position, int score, Popup.scoreType type)
	{
		_myPopUp = _popUpScene.Instantiate<Popup>();
		_myPopUp.SetPopUpStats(position, score, type);
		PopUpHandler.Instance.AddChild(_myPopUp);
	}

	public void CallCountdownSounds()
	{
		if (_specialTimer.TimeLeft <= 3 && _specialTimer.TimeLeft >= 2.95 ||
		 _specialTimer.TimeLeft <= 2 && _specialTimer.TimeLeft >= 1.95 ||
		 _specialTimer.TimeLeft <= 1 && _specialTimer.TimeLeft >= .95)
		{
			AudioManager.Instance.PlaySFX(AudioManager.Instance._timerFX, AudioManager.Instance._audioLibrary.timeTick);
		}
	}

	public void TriggerGameOver()
	{
		_currentGameState = gameState.gameOver;
		AudioManager.Instance.PlayMusic(AudioManager.Instance._audioLibrary.deathTheme);
		_specialTimer.Stop();
	}

	public void TriggerRestart()
	{
		_currentGameState = gameState.normal;
		ResetGameValues();
		AudioManager.Instance.PlayRandomMusicTrack();
	}

	public int GetScore()
	{
		if (_isCountingScore) { return _tempScore; }
		else { return _score; }
	}
	public int GetBurgerScore()
	{
		if (_isCountingBurger) { return _tempBurgerScore; }
		else { return _burgerScore; }
	}

	public void IncreaseMultiplier(float mult) { _multiplier += mult; }
	public float GetMultiplier() { return _multiplier; }
	public int GetGamePhase() { return _gamePhase; }
	public void AddBurgerStrike() { _burgerStrikes += 1; }
	public int GetBurgerStrikes() { return _burgerStrikes; }

	public void CountNewBurgerScore(int count)
	{
		_tempBurgerScore = _burgerScore;
		_burgerScore += count;
		GeneratePopUp(new Vector2(82, 70), count, Popup.scoreType.burger);
		_isCountingBurger = true;
	}
	public void IncreaseBurgerScore(int count)
	{
		_isCountingBurger = false;
	}

	public void CountScoreIncrease()
	{
		_tempScore = _score;
		int increase = (int)(_burgerScore * _multiplier);
		_score += increase;
		GeneratePopUp(new Vector2(82, 260), increase, Popup.scoreType.master);
		_isCountingScore = true;
	}

	public void IncreaseScore()
	{
		_isCountingScore = false;
		_burgerScore = 0;
		_multiplier = 1;
		CalculateGamePhase();
	}

	private void CalculateGamePhase()
	{
		_gamePhase = (_score / 100) + 1;
	}

	public int GetSpecialTime()
	{
		if (_specialTimer.IsStopped())
		{
			return  (int)_specialTime;
		}
		else
		{
			return (int)_specialTimer.TimeLeft;
		}
		
	}
	public void CountSpecialTimeIncrease(double time)
	{
		GeneratePopUp(new Vector2(480, 70), (int)time, Popup.scoreType.special);
		if (_specialTimer.IsStopped())
		{
			_specialTime += time;
		}
		else
		{
			_specialTimer.Start(_specialTimer.TimeLeft + time);
		}
	}

	public void ActivateSpecialTime()
	{
		_specialTimer.Start(_specialTime);
		_currentGameState = gameState.special;
	}

	public void ResetIndividualBurgerScore()
	{
		_burgerScore = 0;
		_multiplier = 1;
	}


	public void ResetGameValues()
	{
		_burgerScore = 0;
		_specialTimer.Stop();
		_specialTime = 0;
		_score = 0;
		_multiplier = 1;
		_gamePhase = 0;
	}
	private void OnSpecialTimerTimeout()
	{
		_specialTime = 0;
		_currentGameState = gameState.normal;
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.ResumeNormalMode);
	}
}
