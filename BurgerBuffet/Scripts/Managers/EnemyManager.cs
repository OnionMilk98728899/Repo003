using Godot;
using System;

public partial class EnemyManager : Node2D
{
	[Export] private Timer _spawnTimer;
	[Export] private PackedScene _meanieScene;
	private Vector2 _BOARD_ORIGIN_POSITION = new Vector2(152, 54), _spawnPosition;
	private Meanie _myMeanie;
	private int _phase, _maxMeanies;
	private (int, int) _boardSquare;

	public override void _Ready()
	{
		GlobalSignals.Instance.GameOver += OnGameOver;
		GlobalSignals.Instance.RestartGame += OnRestartGame;
		_spawnTimer.Start();
	}

	private void SpawnMeanie()
	{
		_maxMeanies = Mathf.Clamp(GlobalResources.Instance.GetGamePhase() * 5, 0, 99);
		if (BoardManager.Instance._occupiedMeanieSquares.Count < _maxMeanies)
		{
			_boardSquare = BoardManager.Instance.OccupyRandomAvailableBoardSquare(true);

			_spawnPosition.X = _BOARD_ORIGIN_POSITION.X + (_boardSquare.Item1 * 16);
			_spawnPosition.Y = _BOARD_ORIGIN_POSITION.Y + (_boardSquare.Item2 * 16);

			_myMeanie = _meanieScene.Instantiate<Meanie>();
			_myMeanie.SetBoardSquare(_boardSquare);
			_myMeanie.GlobalPosition = _spawnPosition;
			AddChild(_myMeanie);
		}

	}
	private void OnSpawnTimerTimeout()
	{
		SpawnMeanie();
		_spawnTimer.Start();
	}

	private void WipeMeanies()
	{
		foreach(Node child in GetChildren())
		{
			if(child is Meanie)
			{
				child.QueueFree();
			}
		}
	}

	private void OnGameOver()
	{
		_spawnTimer.Stop();
		
	}

	private void OnRestartGame()
	{
		_spawnTimer.Start();
		WipeMeanies();
	}


}
