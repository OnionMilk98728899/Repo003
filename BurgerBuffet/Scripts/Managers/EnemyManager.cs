using Godot;
using System;

public partial class EnemyManager : Node2D
{
	[Export] private Timer _spawnTimer;
	[Export] private PackedScene _meanieScene;
	private Vector2 _BOARD_ORIGIN_POSITION = new Vector2(152, 54), _spawnPosition;
	private Meanie _myMeanie;
	private int _phase;
	private (int, int) _boardSquare;

	public override void _Ready()
	{
		_spawnTimer.Start();
	}

	private void SpawnMeanie()
	{
		if (BoardManager.Instance._occupiedSquares.Count < 45)
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


}
