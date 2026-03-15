using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class PathfinderManager : Node2D
{
	[Export] private TileMap _mainTileMap, _ladderTileMap, _wallTileMap;
	[Export] private jewel _myJewel;
	private List<(Vector2, int)> _pathToTry = new List<(Vector2, int)>(),
	 _ladderList = new List<(Vector2, int)>(),
	 _wallList = new List<(Vector2, int)>(),
	 _viablePathList = new List<(Vector2, int)>(),
	 _viablePathsThisFloor = new List<(Vector2, int)>(),
	  _finalPathToUse = new List<(Vector2, int)>();
	private (Vector2, int) _myPathCoordinate, ladderToClimb;
	private Vector2 _pathfindCursorLocation, _roomOriginPoint, _tileCheckCursor, _cursorOrigin, TOWER_START_COORDS = new Vector2(328, -8);
	private Vector2I _cellCoords;
	private List<(int, int)> _laddersAndWallsPerFloor = new List<(int, int)>();
	private (int, int) _ladderWallCountThisFloor;
	private int _currentFloor, _directionalMultiplier, _laddersInTheTower = 0, _moveSequence = 0, _pathSequence = 0;
	private int TOWER_TILE_WIDTH = 15, TILE_LENGTH_IN_PIXELS = 16, _viableLadderLoadAttempts = 0;
	public bool _isPathfinderActive;
	private bool _isLookingForBackwardPath, _isBacktracking, _isViablePathSearchSuccessful, _hasReachedDeadEndPath;


	public override void _Ready()
	{
		_pathfindCursorLocation = TOWER_START_COORDS;
		_currentFloor = 1;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isPathfinderActive)
		{
			ScanForLaddersAndWalls();
			GD.Print("Ladder list count = " + _ladderList.Count);

			while (_viableLadderLoadAttempts < 60 && !_isViablePathSearchSuccessful)
			{
				LoadViablePath();
				_viableLadderLoadAttempts++;
			}
			if (_isViablePathSearchSuccessful)
			{
				GD.Print($"Found a Path on {_viableLadderLoadAttempts} tries!");
				_viablePathList.Add(( new Vector2( 196, _viablePathList[_viablePathList.Count-1].Item1.Y) , 0));
				foreach ((Vector2, int) pathNode in _viablePathList)
				{
					GD.Print($"{pathNode.Item2} node is position {pathNode.Item1}");
				}
				MyGlobalResources._enemyPath = _viablePathList;
			}
			else
			{
				_viablePathList.Add((new Vector2(88, -8), 0));
				GD.Print("No Viable Path found");
			}

			_isPathfinderActive = false;
		}

	}

	private void ScanForLaddersAndWalls()
	{
		if (_currentFloor % 2 == 0)
		{
			_directionalMultiplier = 1;
		}
		else
		{
			_directionalMultiplier = -1;
		}
		for (int i = 1; i < MyGlobalResources._currentTowerLevels; i++)
		{
			for (int j = 0; j < TOWER_TILE_WIDTH; j++)
			{
				_pathfindCursorLocation.X += TILE_LENGTH_IN_PIXELS * _directionalMultiplier;
				_cellCoords = _ladderTileMap.LocalToMap(_pathfindCursorLocation);

				int sourceId = _ladderTileMap.GetCellSourceId(0, _cellCoords);

				if (sourceId != -1)
				{
					_ladderWallCountThisFloor.Item1++;
					_laddersInTheTower++;
					_ladderList.Add((_pathfindCursorLocation, _currentFloor));
				}

				sourceId = _wallTileMap.GetCellSourceId(0, _cellCoords);
				if (sourceId != -1)
				{
					//A wall tile is detected
					_ladderWallCountThisFloor.Item2++;
					_wallList.Add((_pathfindCursorLocation, _currentFloor));
				}

			}
			_laddersAndWallsPerFloor.Add(_ladderWallCountThisFloor);
			_ladderWallCountThisFloor = (0, 0);
			_pathfindCursorLocation.Y -= 3 * TILE_LENGTH_IN_PIXELS;
			_currentFloor++;
			if (_currentFloor % 2 == 0)
			{
				_directionalMultiplier = 1;
			}
			else
			{
				_directionalMultiplier = -1;
			}
		}

	}

	private void LoadViablePath()
	{
		_cursorOrigin = TOWER_START_COORDS;
		for (int i = 1; i < MyGlobalResources._currentTowerLevels; i++) //// for each floor
		{
			_moveSequence++;
			foreach ((Vector2, int) pathNode in _ladderList)  //// for each ladder on that floor
			{
				if (pathNode.Item2 == i && pathNode.Item1.X != _cursorOrigin.X) //// If this is a ladder node on the target floor, and it is NOT the source ladder if backtracking
				{
					if (_cursorOrigin.X < pathNode.Item1.X) { _directionalMultiplier = 1; }
					else { _directionalMultiplier = -1; }

					bool isWallBlockingPath = false;
					_pathfindCursorLocation = _cursorOrigin;

					for (int j = 0; j < Mathf.Abs(_cursorOrigin.X - pathNode.Item1.X); j += TILE_LENGTH_IN_PIXELS) /// for each tile between the origin point and ladder
					{
						_pathfindCursorLocation.X += TILE_LENGTH_IN_PIXELS * _directionalMultiplier;
						_cellCoords = _ladderTileMap.LocalToMap(_pathfindCursorLocation);
						int sourceId = _wallTileMap.GetCellSourceId(0, _cellCoords);
						if (sourceId != -1) { isWallBlockingPath = true; }                                  /// detect obstructions
					}

					if (!isWallBlockingPath) { _viablePathsThisFloor.Add((pathNode.Item1, _moveSequence)); }
				}
			}

			if (_viablePathsThisFloor.Count == 0)
			{
				/// _cursorOrigin DOESN'T MOVE
				if (_isLookingForBackwardPath)
				{
					_hasReachedDeadEndPath = true;
					break;
				}
				else
				{
					i -= 2;
					_isLookingForBackwardPath = true;
				}
			}
			if (_viablePathsThisFloor.Count == 1)
			{
				_cursorOrigin = _viablePathsThisFloor[0].Item1;   //// Finds the viable ladder from this floor
				AddLadderNodesToPath(_cursorOrigin, _isLookingForBackwardPath);
				if (_isLookingForBackwardPath)
				{
					//// no modification to the ladder position == "climbs" down it
					i--;
					_isLookingForBackwardPath = false;
				}
				else
				{

					_cursorOrigin.Y -= 3 * TILE_LENGTH_IN_PIXELS;     //// "climbs" up it
				}
			}
			if (_viablePathsThisFloor.Count > 1)
			{
				int randint = GD.RandRange(0, _viablePathsThisFloor.Count - 1);
				_cursorOrigin = _viablePathsThisFloor[randint].Item1;    //// Finds a random viable ladder from this floor
				AddLadderNodesToPath(_cursorOrigin, _isLookingForBackwardPath);
				if (_isLookingForBackwardPath)
				{
					//// no modification to the ladder position == "climbs" down it
					_isLookingForBackwardPath = false;
					i--;
				}
				else
				{
					_cursorOrigin.Y -= 3 * TILE_LENGTH_IN_PIXELS;     //// "climbs" up it
				}
			}

			_viablePathsThisFloor.Clear();
		}

		if (!_hasReachedDeadEndPath)
		{
			_isViablePathSearchSuccessful = true;
		}
		else
		{
			_isViablePathSearchSuccessful = false;
		}
	}

	private void AddLadderNodesToPath(Vector2 ladderNode, bool isGoingBackward)
	{
		_pathSequence++;
		if (isGoingBackward)
		{
			_viablePathList.Add(((new Vector2(ladderNode.X, ladderNode.Y - 3 * TILE_LENGTH_IN_PIXELS)), _pathSequence));
			_pathSequence++;
			_viablePathList.Add((ladderNode, _pathSequence));
		}
		else
		{
			_viablePathList.Add((ladderNode, _pathSequence));
			_pathSequence++;
			_viablePathList.Add(((new Vector2(ladderNode.X, ladderNode.Y - 3 * TILE_LENGTH_IN_PIXELS)), _pathSequence));
		}
	}

	// private void PrintTilesInTestMode()
	// {
	// 	int r = GD.RandRange(8, 17);
	// 	Vector2I printCoord = new Vector2I(r, 9);
	// 	if (_isLookingForBackwardPath) { printCoord = new Vector2I(11, 7); }
	// 	_mainTileMap.SetCell(0, _cellCoords, 0, printCoord);

	// }

}
