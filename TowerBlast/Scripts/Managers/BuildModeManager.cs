using Godot;
using System;
using System.Collections.Generic;
using System.Transactions;

public partial class BuildModeManager : Node2D
{
	[Signal] public delegate void ActivateBuildFloorModeEventHandler(bool isActive);
	[Signal] public delegate void MoveGridModeCursorEventHandler(Vector2 cursorPos);
	[Export] private TileMap _mainTileMap, _wallTileMap, _ladderTileMap, _tempFloorMap;
	[Export] private TrapManager _trapMan;
	[Export] private Sprite2D _cursorSprite;
	[Export] private int _buildLadderPrice, _buildFloorPrice, _buildWallPrice;
	[Export] private jewel _myJewel;
	[Export] private PathfinderManager pathfinderMan;
	[Export] private BackgroundGenerator _myBackgroundGen;
	[Export] private Texture2D _greenLadderTexture, _greenWallTexture, _redLadderTexture, _redWallTexture, _whiteTrapCursor, _redTrapCursor;
	private TileMap _tileMapToBuildOn;
	private Vector2 _buildModeCursorPosition, _mapPosition;
	private Vector2I _cellCoords, _tileToBuild,
	LADDER_TILE_ATLAS_COORDS = new Vector2I(10, 7),
	WALL_TILE_ATLAS_COORDS = new Vector2I(5, 6),
	BACKGROUND_TILE_ATLAS_COORDS = new Vector2I(6, 6),
	FLOOR_TILE_ATLAS_COORDS = new Vector2I(6, 5),
	LEFT_FLOOR_NUB_ATLAS_COORDS = new Vector2I(4, 5),
	RIGHT_FLOOR_NUB_ATLAS_COORDS = new Vector2I(4, 5);

	private bool _isCursorRepositioned, _isCursorOverLadderOrWall, _isCursorOverTrap, _isViableBuildPosition, _isTrapMenuActivated;
	public bool _isBuildModeGridActive, _isBuildLadderModeActive, _isBuildWallModeActive, _isBuildTrapModeActive,
	 _isBuildFloorModeActive, _isUpgradeModeActive, _isDemoModeActive;
	private int LADDER_LAYER_ID, WALL_LAYER_ID, LEFT_WALL_BOUNDARY = 88, RIGHT_WALL_BOUNDARY = 304, _forbiddenObjectsDetected, _destroyableObjectsDetected;

	public override void _Ready()
	{
		GlobalSignals.Instance.DisableTrapMenu += ExitTrapMenuMode;
		_buildModeCursorPosition = new Vector2(88, -8);

	}
	public override void _PhysicsProcess(double delta)
	{
		if (_isBuildLadderModeActive || _isBuildWallModeActive || _isBuildTrapModeActive)
		{
			_isBuildModeGridActive = true;
		}
		else
		{
			_isBuildModeGridActive = false;
		}
		if (_isBuildModeGridActive)
		{
			HandleBuildModeDirectionalInput();

			if (Input.IsActionJustPressed("ui_accept"))
			{
				if (_isBuildLadderModeActive || _isBuildWallModeActive)
				{
					BuildLadderOrWall();
				}
				if (_isBuildTrapModeActive)
				{
					EnterTrapMenuMode();
				}
			}
		}
		if (_isBuildFloorModeActive)
		{
			if (Input.IsActionJustPressed("ui_accept"))
			{
				BuildFloor();
				_myBackgroundGen.AddAdiitionalBackgroundTiles((MyGlobalResources._currentTowerLevels * 3) + 5);
			}
		}
		if (_isDemoModeActive || _isUpgradeModeActive)
		{
			HandleBuildModeDirectionalInput();

			if (Input.IsActionJustPressed("ui_accept"))
			{

			}
		}
		SetCursorTexture();
	}

	private void HandleBuildModeDirectionalInput()
	{
		if (Input.IsActionJustPressed("ui_left") && _buildModeCursorPosition.X > 88)
		{
			_buildModeCursorPosition.X -= 16;

		}
		else if (Input.IsActionJustPressed("ui_right") && _buildModeCursorPosition.X < 304)
		{
			_buildModeCursorPosition.X += 16;

		}
		else if (Input.IsActionJustPressed("ui_up") && _buildModeCursorPosition.Y > -(MyGlobalResources._currentTowerLevels - 1) * 48)
		{
			_buildModeCursorPosition.Y -= 48;

		}
		else if (Input.IsActionJustPressed("ui_down") && _buildModeCursorPosition.Y < -8)
		{
			_buildModeCursorPosition.Y += 48;
		}

		if (_cursorSprite.GlobalPosition.Y != _buildModeCursorPosition.Y)
		{
			EmitSignal("MoveGridModeCursor", _buildModeCursorPosition);
			GD.Print("Emitting Cam Change signal!");
		}
		_cursorSprite.GlobalPosition = _buildModeCursorPosition;

	}

	public void RepositionBuildModeCursor()
	{
		if (!_isCursorRepositioned)
		{
			_forbiddenObjectsDetected = 0;
			_buildModeCursorPosition = new Vector2(104, -8);

			_isCursorRepositioned = true;
		}

		_isCursorRepositioned = false;
	}

	private void SetCursorTexture()
	{
		if (_forbiddenObjectsDetected > 0)
		{
			if (_isBuildWallModeActive)
			{
				_cursorSprite.Texture = _redWallTexture;
			}
			if (_isBuildLadderModeActive)
			{
				_cursorSprite.Texture = _redLadderTexture;
			}
			if (_isBuildTrapModeActive)
			{
				_cursorSprite.Texture = _redTrapCursor;
			}
		}
		else
		{
			if (_isBuildWallModeActive)
			{
				_cursorSprite.Texture = _greenWallTexture;
			}
			if (_isBuildLadderModeActive)
			{
				_cursorSprite.Texture = _greenLadderTexture;
			}
			if (_isBuildTrapModeActive)
			{
				//GD.Print("Trap Mode entered Viable!");
				_cursorSprite.Texture = _whiteTrapCursor;
			}
		}
		if (!_isBuildLadderModeActive && !_isBuildWallModeActive && !_isBuildTrapModeActive && !_isTrapMenuActivated)
		{
			_cursorSprite.Texture = null;
		}
	}

	private void CommitTileToTilemap(TileMap myMap, Vector2 tilePosition, int sourceID)
	{
		_mapPosition = _mainTileMap.ToLocal(tilePosition);
		_cellCoords = _mainTileMap.LocalToMap(_mapPosition);

		myMap.SetCell(0, _cellCoords, sourceID, _tileToBuild);
	}

	private void BuildFloor()
	{
		//for each tile on the top floor
		//tiles on #of floors *48
		if (MyGlobalResources._playerGoldQuantity >= _buildFloorPrice)
		{
			int xCoord = 0;
			int yCoord = -(MyGlobalResources._currentTowerLevels * 48);
			_tileToBuild = FLOOR_TILE_ATLAS_COORDS;
			for (int i = 0; i < 19; i++)
			{
				xCoord = (3 + i) * 16;
				if (i == 0)
				{
					_tileToBuild = LEFT_FLOOR_NUB_ATLAS_COORDS;
				}
				else if (i == 1 || i == 17)
				{
					_tileToBuild = WALL_TILE_ATLAS_COORDS;
				}
				else if (i == 18)
				{
					_tileToBuild = RIGHT_FLOOR_NUB_ATLAS_COORDS;
				}
				else
				{
					_tileToBuild = FLOOR_TILE_ATLAS_COORDS;
				}
				CommitTileToTilemap(_mainTileMap, new Vector2(xCoord, yCoord), 0);
			}
			xCoord = 192;
			_myJewel.GlobalPosition = new Vector2(xCoord, yCoord);
			//GD.Print("Jewel Position = " + _myJewel.GlobalPosition);

			_tileToBuild = BACKGROUND_TILE_ATLAS_COORDS;
			for (int i = 0; i < 2; i++)
			{
				yCoord -= 16;
				for (int j = 0; j < 17; j++)
				{
					xCoord = (4 + j) * 16;

					if (j == 0 || j == 16)
					{
						_tileToBuild = WALL_TILE_ATLAS_COORDS;
					}
					else
					{
						_tileToBuild = BACKGROUND_TILE_ATLAS_COORDS;
					}
					CommitTileToTilemap(_mainTileMap, new Vector2(xCoord, yCoord), 0);
					//GD.Print("Printed Tile at " + xCoord + " , " + yCoord);
				}
			}

			xCoord = 0;
			yCoord -= 16;
			for (int i = 0; i < 17; i += 2)
			{
				xCoord = (4 + i) * 16;

				if (i == 0 || i == 16)
				{
					_tileToBuild = WALL_TILE_ATLAS_COORDS;
				}

				else
				{
					_tileToBuild = BACKGROUND_TILE_ATLAS_COORDS;
				}
				CommitTileToTilemap(_mainTileMap, new Vector2(xCoord, yCoord), 0);
			}



			MyGlobalResources._playerGoldQuantity -= _buildFloorPrice;
			//GD.Print("Current Gold: " + MyGlobalResources._playerGoldQuantity);
			MyGlobalResources._currentTowerLevels++;
		}
		else
		{
			GD.PrintErr("You haven't the gold to purchase that floor, sire!");
		}


	}

	private void BuildLadderOrWall()
	{
		if (_isViableBuildPosition)
		{
			if (_isBuildLadderModeActive)
			{
				if (MyGlobalResources._playerGoldQuantity >= _buildLadderPrice)
				{
					_tileToBuild = LADDER_TILE_ATLAS_COORDS;
					_tileMapToBuildOn = _ladderTileMap;
					MyGlobalResources._playerGoldQuantity -= _buildLadderPrice;
				}
				else
				{
					GD.PrintErr("You haven't the gold to purchase that ladder, sire!");
				}
			}
			if (_isBuildWallModeActive)
			{
				if (MyGlobalResources._playerGoldQuantity >= _buildWallPrice)
				{
					_tileToBuild = WALL_TILE_ATLAS_COORDS;
					_tileMapToBuildOn = _wallTileMap;
					MyGlobalResources._playerGoldQuantity -= _buildWallPrice;
				}
				else
				{
					GD.PrintErr("You haven't the gold to purchase that wall, sire!");
				}
			}

			Vector2 myTilePosition = _buildModeCursorPosition;
			for (int i = 0; i < 3; i++)
			{
				CommitTileToTilemap(_tileMapToBuildOn, myTilePosition, 0);
				myTilePosition.Y -= 16;
			}
			BuildTemporaryFloor();
			MyGlobalResources._ladderWallTrapLocationList.Add(_buildModeCursorPosition);
		}
		else
		{
			GD.PrintErr("Not a viable build position!");
		}
	}

	private void BuildTemporaryFloor()
	{
		if (_isBuildLadderModeActive)
		{
			Vector2 myFloorTilePosition = new Vector2(_buildModeCursorPosition.X - 16, _buildModeCursorPosition.Y - 32);
			_tileToBuild = new Vector2I(7, 5);
			for (int i = 0; i < 3; i++)
			{
				_tileToBuild = new Vector2I(8, 10);
				CommitTileToTilemap(_mainTileMap, myFloorTilePosition, 0);
				_tileToBuild = new Vector2I(7, 5);
				CommitTileToTilemap(_tempFloorMap, myFloorTilePosition, 0);
				myFloorTilePosition.X += 16;
			}
		}
	}

	private void EnterTrapMenuMode()
	{
		if (_isViableBuildPosition)
		{
			if (!_isTrapMenuActivated)
			{
				_isBuildTrapModeActive = false;
				_trapMan.ActivateTrapBuyingMenu(true, _buildModeCursorPosition);
			}
		}
		else
		{
			GD.PrintErr("My leige, you cannot build a trap thusly!");
		}
	}

	private void ExitTrapMenuMode(bool isDisabled)
	{
		_isBuildTrapModeActive = !isDisabled;
	}



	private void DemolishComponent()
	{
		if (_destroyableObjectsDetected == 1)
		{
			if (_isCursorOverLadderOrWall)
			{
				CommitTileToTilemap(_tileMapToBuildOn, _buildModeCursorPosition, -1);
			}


		}
		else if (_destroyableObjectsDetected > 1)
		{

		}
		else if (_destroyableObjectsDetected == 0)
		{

		}
	}

	private void OnCursorAreaBodyEntered(Node2D body)       ///Detects area directly on top of the cursor, and the tile above it (mostly detects traps)
	{
		if (body.IsInGroup("Ladders"))
		{
			if (_isDemoModeActive)
			{
				_tileMapToBuildOn = _ladderTileMap;
				_destroyableObjectsDetected++;
				_isCursorOverLadderOrWall = true;
			}
			if (_isBuildTrapModeActive)
			{
				_forbiddenObjectsDetected++;
				//GD.Print("Added foreign object + : " + _forbiddenObjectsDetected);

			}

		}
		if (body.IsInGroup("Walls"))
		{
			if (_isDemoModeActive)
			{
				_tileMapToBuildOn = _wallTileMap;
				_destroyableObjectsDetected++;
				_isCursorOverLadderOrWall = true;
			}
			if (_isBuildTrapModeActive)
			{
				_forbiddenObjectsDetected++;
				//GD.Print("Added foreign object + : " + _forbiddenObjectsDetected);

			}
		}
		if (body.IsInGroup("Traps"))
		{
			if (_isDemoModeActive)
			{
				_destroyableObjectsDetected++;
				_isCursorOverTrap = true;
			}
			if (_isBuildWallModeActive)
			{
				_forbiddenObjectsDetected++;
			}
			if (_isBuildTrapModeActive)
			{

			}

		}

	}

	private void OnCursorAreaBodyExited(Node2D body)
	{
		if (body.IsInGroup("Walls") || body.IsInGroup("Ladders"))
		{
			_destroyableObjectsDetected--;
			_isCursorOverLadderOrWall = false;
			if (_isBuildTrapModeActive)
			{
				_forbiddenObjectsDetected--;
				//GD.Print("Lost foreign object - : " + _forbiddenObjectsDetected);
			}
		}
		if (body.IsInGroup("Traps"))
		{
			_destroyableObjectsDetected--;
			if (_isBuildTrapModeActive)
			{
				_forbiddenObjectsDetected--;
				//GD.Print("Lost foreign object - : " + _forbiddenObjectsDetected);
			}
		}
		if (_destroyableObjectsDetected < 0)
		{
			_destroyableObjectsDetected = 0;
			_isCursorOverLadderOrWall = false;
			_isCursorOverTrap = false;
		}
	}

	private void OnForbiddenObjectDetectorEntered(Node2D body)  ///Detects horizontal forbidden objects (should detect other ladders/walls for build ladder/wall modes)
	{
		if (body.IsInGroup("Walls") || body.IsInGroup("Ladders"))
		{
			if (_isBuildLadderModeActive || _isBuildWallModeActive)
			{
				_forbiddenObjectsDetected++;
			}
		}
		if (_forbiddenObjectsDetected > 0)
		{
			_isViableBuildPosition = false;
		}
	}


	private void OnForbiddenObjectDetectorExited(Node2D body)
	{
		if (body.IsInGroup("Walls") || body.IsInGroup("Ladders"))
		{
			if (_isBuildLadderModeActive || _isBuildWallModeActive)
			{
				_forbiddenObjectsDetected--;
			}
		}
		if (_forbiddenObjectsDetected <= 0)
		{
			_forbiddenObjectsDetected = 0;
			_isViableBuildPosition = true;
		}
	}


	private void OnForbiddenObjectDetector2Entered(Node2D body) ///Detects vertical forbidden objects  (mostly ladder nodes for wall/ladder build modes)
	{
		if (body.IsInGroup("Ladders"))
		{
			if (_isBuildLadderModeActive || _isBuildWallModeActive)
			{
				_forbiddenObjectsDetected++;
			}
		}
		if (body.IsInGroup("Walls") || body.IsInGroup("Traps"))
		{
			if (_isBuildLadderModeActive)
			{
				_forbiddenObjectsDetected++;
			}
		}
		if (_forbiddenObjectsDetected > 0)
		{
			_isViableBuildPosition = false;
		}
	}


	private void OnForbiddenObjectDetector2Exited(Node2D body)
	{
		if (body.IsInGroup("Ladders"))
		{
			if (_isBuildLadderModeActive || _isBuildWallModeActive)
			{
				_forbiddenObjectsDetected--;
			}
		}
		if (body.IsInGroup("Walls") || body.IsInGroup("Traps"))
		{
			if (_isBuildLadderModeActive)
			{
				_forbiddenObjectsDetected--;
			}
		}
		if (_forbiddenObjectsDetected <= 0)
		{
			_forbiddenObjectsDetected = 0;
			_isViableBuildPosition = true;
		}
	}
}



