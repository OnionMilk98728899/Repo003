using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

public partial class MapGenerator2 : Node2D
{
	[Export] private TileMap tileMap;
	[Export] private TileSet tileSet;
	[Export] private int gridSize;
	[Export] private PackedScene doorScene;
	[Export] private DoorManager doorMan;
	[Export] private Timer floorResetTimer;
	private Door_2D newDoor;

	private bool notyetTriggered;
	private int tileQuantity, nodeTileQuantity, roomCount = 0, floorSize, floorCount = 0, exitNodeTileType, entranceNodeTileType,
	forbiddenExitSide1, forbiddenExitSide2, forbiddenExitSide3, forbiddenExitSide4, enemyRangeTop, enemyRangeDown, enemyRangeLeft, enemyRangeRight;
	private (int,int) myBorderTile, myNewTile, entranceNodeTile, exitNodeTile, currentRoom;
	private int[,] roomGrid, floorGrid, hallwayGrid;
	private List<(int,int)> myNodeTiles, nodeTilesToAdd, nodeTilesToDupeFrom, myDoorTiles ;
	private Godot.Collections.Array<Vector2I> enemySpawnNodeList;
	//private List<Door_2D> doorsOnFloor;
	private List<int> borderTileList;
	private bool centerTilesPopulated, doorTilesPopulated, entranceTileAdded, exitNodeRandomizerOn, nodeRandomizerOn, roomLimitReached;
	private Vector2I roomOffset;

	public override void _Ready()
	{

		GlobalSignals.Instance.ExitDoorTriggered += ExitDoorTriggered;

		roomOffset = new Vector2I(0,0);
		
		if(roomCount == 0){

			/// Here's where we will generate the Fixed area starting room but for now, 
			GenerateNewFloor();
			GenerateNewRoom();
			

		}
		GlobalSignals.Instance.EmitAdjustCharPosition(new Godot.Vector2(200,200));
	}

	public override void _PhysicsProcess(double delta){

		// if(exitNodeRandomizerOn){
		// 	exitNodeTileType = GD.RandRange(1,4);
		// 	if(exitNodeTileType != entranceNodeTileType){
		// 		exitNodeRandomizerOn = false;
		// 	}
		// }
	}

	private void ExitDoorTriggered(){

		GenerateNewRoom();
	}

	private void GenerateNewFloor(){

		floorSize = GD.RandRange(5,9);
		floorGrid = new int[floorSize, floorSize];

		for(int x=0; x< floorSize; x++){

			for(int y = 0; y< floorSize; y++){
				
				floorGrid[x,y] = 0;
			}
		}

		currentRoom	= (floorSize/2, floorSize/2);

		floorGrid[floorSize/2, floorSize/2] = 1;
		
		doorMan.CreateNewDoorList();

		floorCount++;
	}

	private void GenerateNewRoom()
	{
		myNodeTiles = new List<(int, int)>();
		nodeTilesToAdd = new List<(int, int)>();
		nodeTilesToDupeFrom = new List<(int, int)>();
		myDoorTiles = new List<(int, int)>();
		centerTilesPopulated = false;

		GenerateGrid();
		FindForbiddenRooms();

		if (!roomLimitReached)
		{
			AddBorderTiles();

			while (!centerTilesPopulated)
			{
				foreach ((int, int) tile in myNodeTiles)
				{
					if (MathF.Abs(tile.Item1 - (gridSize / 2)) <= 2 && MathF.Abs(tile.Item2 - (gridSize / 2)) <= 2)
					{
						centerTilesPopulated = true;
					}
				}
				AddCenterTiles();
			}

			ExpandNodeTiles();
			AddWallTiles();
			FinishWallsAndAddDoors();
			UpdateFloorMap();
			PrintGrid();

			roomCount++;
		}else{

			ClearFloor();
		}

	}

	private void ClearFloor(){

		tileMap.Clear();
		
		doorMan.ClearDoorList();
		floorResetTimer.Start();

	}

	private void GenerateGrid(){

		roomGrid= new int[gridSize, gridSize];

		for(int x = 0; x < gridSize; x++){

			for(int y = 0; y < gridSize; y++){

				roomGrid[x,y] = 0;

			}
		}
	}

	private void FindForbiddenRooms(){

		forbiddenExitSide1 = 0;
		forbiddenExitSide2 = 0;
		forbiddenExitSide3 = 0;
		forbiddenExitSide4 = 0;

		if (currentRoom.Item1 > 0 && currentRoom.Item2 > 0 && currentRoom.Item1 < floorSize - 1 && currentRoom.Item2 < floorSize - 1)
		{
			if (floorGrid[currentRoom.Item1, currentRoom.Item2 - 1] == 1)    ////this indicates Top
			{          
				forbiddenExitSide1 = 1;
			}
			if (floorGrid[currentRoom.Item1 - 1, currentRoom.Item2] == 1)	////this indicates Left
			{           
				forbiddenExitSide2 = 2;

			}
			if (floorGrid[currentRoom.Item1 + 1, currentRoom.Item2] == 1)	////this indicates Right
			{           
				forbiddenExitSide3 = 3;

			}
			if (floorGrid[currentRoom.Item1, currentRoom.Item2 + 1] == 1)	 ////this indicates Down
			{          
				forbiddenExitSide4 = 4;
			}

		}else{
			roomLimitReached = true;
		}
	}

	private void AddBorderTiles(){

		borderTileList = new List<int>();

		if(roomCount == 0){
			entranceNodeTile = (gridSize/2, 0);
		}

		if (entranceNodeTile.Item2 == 0)                            ////TopSide
		{
			entranceNodeTileType = 1;
			
		}
		else if (entranceNodeTile.Item1 == 0)                       ///LeftSide
		{
			entranceNodeTileType = 2;
		}
		else if (entranceNodeTile.Item1 == gridSize - 1)           ////RightSide
		{
			entranceNodeTileType = 3;
		}
		else if (entranceNodeTile.Item2 == gridSize - 1)            ///BottomSide
		{
			entranceNodeTileType = 4;
		}
		GD.Print("Adding Border Tiles and EntranceNode Tile Type is " +entranceNodeTileType);

		exitNodeRandomizerOn = true;

		while(exitNodeRandomizerOn){

			exitNodeTileType  = GD.RandRange(1,4);
			if(exitNodeTileType != entranceNodeTileType && exitNodeTileType != forbiddenExitSide1 
			&& exitNodeTileType != forbiddenExitSide2 && exitNodeTileType != forbiddenExitSide3 && exitNodeTileType != forbiddenExitSide4){

				exitNodeRandomizerOn = false;
			}
		}
		
		borderTileList.Add(exitNodeTileType);

		int borderTileCount = GD.RandRange(0, 2);

		for (int x = 0; x < borderTileCount; x++)
		{
			nodeRandomizerOn = true;
			while (nodeRandomizerOn)
			{
				int borderTileType = GD.RandRange(1, 4);
				if (borderTileType != entranceNodeTileType && borderTileType != forbiddenExitSide1
				&& borderTileType != forbiddenExitSide2 && borderTileType != forbiddenExitSide3 && borderTileType != forbiddenExitSide4)
				{
					borderTileList.Add(borderTileType);
					nodeRandomizerOn = false;
				}
			}
		}

		

		for(int x=0; x < borderTileList.Count; x++){

			switch(borderTileList[x]){
				case 1:
					myBorderTile = (GD.RandRange(4,gridSize-4), 0);    ///Top side border tile
				break;
				case 2:
					myBorderTile = (0, GD.RandRange(4,gridSize-4));    ///Left side border tile
				break;
				case 3:
					myBorderTile = (gridSize-1, GD.RandRange(4,gridSize-4));    ///Right side border tile
				break;
				case 4:
					myBorderTile = (GD.RandRange(4,gridSize-4), gridSize-1);    ///Top side border tile
				break;
				
			}

			if(x == 0){
				myDoorTiles.Add(myBorderTile);
				exitNodeTile = myBorderTile;
			}

			roomGrid[myBorderTile.Item1, myBorderTile.Item2] = 1;
			myNodeTiles.Add(myBorderTile);
			nodeTilesToDupeFrom.Add(myBorderTile);

		}

		myDoorTiles.Add(entranceNodeTile);
		roomGrid[entranceNodeTile.Item1, entranceNodeTile.Item2] = 1;
		myNodeTiles.Add(entranceNodeTile);
		nodeTilesToDupeFrom.Add(entranceNodeTile);

	}
	private void AddCenterTiles(){

		foreach((int,int) tile in nodeTilesToDupeFrom){

			int xCoord = 0;
			int yCoord = 0;

			if(tile.Item1 <= gridSize/2){
				xCoord = tile.Item1 + GD.RandRange(1,5);
			}
			if(tile.Item1 > gridSize/2){
				xCoord = tile.Item1 - GD.RandRange(1,5);
			}
			if(tile.Item2 <= gridSize/2){
				yCoord = tile.Item2 + GD.RandRange(1,5);
			}
			if(tile.Item2 > gridSize/2){
				yCoord = tile.Item2 - GD.RandRange(1,5);
			}

			roomGrid[xCoord, yCoord] = 1;
			myNewTile = (xCoord, yCoord);
			nodeTilesToAdd.Add(myNewTile);
		}

		foreach((int,int) tile in nodeTilesToDupeFrom){
			myNodeTiles.Add(tile);
		}

		nodeTilesToDupeFrom.Clear();

		foreach((int,int) tile in nodeTilesToAdd){
			nodeTilesToDupeFrom.Add(tile);
		}

		nodeTilesToAdd.Clear();
	}

	private void ExpandNodeTiles(){

		enemySpawnNodeList = new Godot.Collections.Array<Vector2I>();  // refreshes the list of nodes where enemies are allowed to spawn.

		foreach ((int, int) tile in myNodeTiles)
		{
			int xpandDegreeX = GD.RandRange(3, 5);
			int xpandDegreeY = GD.RandRange(3, 5);			/// Set the expansion degree for each node tile randomly

			for (int x = tile.Item1 - xpandDegreeX; x <= tile.Item1 + xpandDegreeX; x++)   /// for each tile within the X-axis' expansion degree...
			{	
				for (int y = tile.Item2 - xpandDegreeY; y <= tile.Item2 + xpandDegreeY; y++) /// and for each tile within the Y-axis' expansion degree
				{
					if(x >= 0 && x < gridSize && y >=0 && y < gridSize ){

						roomGrid[x,y] = 2;
						
					}
					if(x == tile.Item1 && y== tile.Item2){

						roomGrid[x,y] = 1;

						if(x > 4 && x < gridSize - 4 && y > 4 && y < gridSize - 4){
							enemySpawnNodeList.Add(new Vector2I(x,y));
						}
					}				
				}
			}
		}

		GlobalSignals.Instance.EmitGenerateEnemySpawnLocations(enemySpawnNodeList, roomOffset);
	}

	private void AddWallTiles()
	{
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				if (y > 0)
				{
					if (roomGrid[x, y] == 2 && roomGrid[x, y - 1] == 0)
					{
						roomGrid[x, y - 1] = 3;
						if (y > 1) { roomGrid[x, y - 2] = 3; }
						if (y > 2) { roomGrid[x, y - 3] = 4; }
					}
				}
				if (x > 0)
				{
					if (roomGrid[x, y] == 2||roomGrid[x, y] == 3||roomGrid[x, y] == 4)
					{
						if (roomGrid[x - 1, y] == 0)
						{
							roomGrid[x - 1, y] = 4;
						}
					}
				}
				if(y < gridSize-1){

					if (roomGrid[x, y] == 2)
					{
						if (roomGrid[x, y + 1] == 0)
						{
							roomGrid[x, y + 1] = 4;
						}
					}
				}
			}
		}

		for(int x= 0; x <gridSize; x++){

			for(int y= 0; y <gridSize; y++){

				if(x < gridSize-1){

					if (roomGrid[x, y] == 2 || roomGrid[x,y] == 3 || roomGrid[x,y] == 4)
					{
						if (roomGrid[x + 1, y] == 0)
						{
							roomGrid[x + 1, y] = 5;
						}
					}
				}

				if( x > 0){
					if(roomGrid[x,y] == 3 && roomGrid[x-1,y] == 0){
						roomGrid[x-1,y] = 4;

					}
				}
			}
		}

		

		for(int x= 0; x <gridSize; x++){

			for(int y= 0; y <gridSize; y++){
				
				if(roomGrid[x,y] == 5){
					roomGrid[x,y]= 4;
				}

				if(roomGrid[x,y] == 1){

					
					roomGrid[x,y] = 2;
				}
			}
		}

		///Send a Global signal with the Spawn list 

		for(int x=0; x < gridSize; x++){

			for(int y = 0; y < gridSize; y++){

				if(y < gridSize - 2){

					if(roomGrid[x,y] == 4){

						if(roomGrid[x,y + 1] == 4 ){

								roomGrid[x,y] = 5;    
						}
					}
				}

				if( x > 0 && x < gridSize - 1 && y < gridSize -1){

					if(roomGrid[x,y + 1] == 4 ){

						if(roomGrid[x - 1, y] == 4 || roomGrid[x + 1, y] == 4){

							roomGrid[x,y] = 6;

						}
					}
				}
			}
		}
	}


	private void FinishWallsAndAddDoors(){
		
		 for(int x=0; x < gridSize; x++){

			for(int y = 0; y < gridSize; y++){

				if(y <= 2){

					if(roomGrid[x,y] == 2 || roomGrid[x,y] ==3){

						if(y== 0){

							roomGrid[x,y] = 4;
						}
						if(y > 0){

							roomGrid[x,y] = 3;
						}
					}

				}

				if (x == 0){

					if(roomGrid[x,y] == 2 || roomGrid[x,y] ==3){

						roomGrid[x,y] = 5;
					}
				}

				if (x == gridSize-1){

					if(roomGrid[x,y] == 2 || roomGrid[x,y] ==3){

						roomGrid[x,y] = 5;
					}
				}

				if (y == gridSize-1){

					if(roomGrid[x,y] == 2 || roomGrid[x,y] ==3){

						roomGrid[x,y] = 6;
					}
				}

			}
		 }

		

		foreach((int,int) tile in myDoorTiles){

			roomGrid[tile.Item1, tile.Item2] = 2;

			newDoor = doorScene.Instantiate<Door_2D>();

			if(tile == exitNodeTile){
				newDoor.enterExitStatus = Door_2D.enterExit.exit;
				newDoor.myDoorState = Door_2D.doorState.closed;
				newDoor.isClosed = true;
			}else{
				newDoor.enterExitStatus = Door_2D.enterExit.enter;
				newDoor.myDoorState = Door_2D.doorState.opening;
				newDoor.isClosed = true;
				//newDoor.CallDeferred("OpenDoorWithDelay");
			}

			newDoor.GlobalPosition = new Godot.Vector2((tile.Item1 + roomOffset.X) * 16 , (tile.Item2 + roomOffset.Y) * 16);

			//// need to determine WHICH 3 tiles are underneath EACH door position, and change those tiles from wall tile type. If the door faces upward, 
			/// we need to make all wall tiles underneath changeable.


			if(tile.Item1 == 0){             				///// Room has a Left Side Door////////////////////////////////////////////////////

				newDoor.myDirection = Door_2D.direction.left;
				roomGrid[tile.Item1, tile.Item2] = 2;
				roomGrid[tile.Item1, tile.Item2+1] = 2;
				roomGrid[tile.Item1, tile.Item2+2] = 2;
			

			}else if ( tile.Item1 == gridSize - 1){			//// Room has a Right Side Door ////////////////////////////////////////////////////
				
				newDoor.myDirection = Door_2D.direction.right;
				roomGrid[tile.Item1, tile.Item2] = 2;
				roomGrid[tile.Item1, tile.Item2+1] = 2;
				roomGrid[tile.Item1, tile.Item2+2] = 2;


			}else if(tile.Item2 == 0){						////Room has a Top Side Door ////////////////////////////////////////////////////

				newDoor.myDirection = Door_2D.direction.up;
				roomGrid[tile.Item1, tile.Item2] = 2;
				roomGrid[tile.Item1, tile.Item2+1] = 2;
				roomGrid[tile.Item1, tile.Item2+2] = 2;
				roomGrid[tile.Item1 +1, tile.Item2] = 2;
				roomGrid[tile.Item1 +1, tile.Item2+1] = 2;
				roomGrid[tile.Item1 +1, tile.Item2+2] = 2;
				roomGrid[tile.Item1 +2, tile.Item2] = 2;
				roomGrid[tile.Item1 +2, tile.Item2+1] = 2;
				roomGrid[tile.Item1 +2, tile.Item2+2] = 2;

			}else if ( tile.Item2 == gridSize - 1){			//////Room has a Bottom Side Door  ////////////////////////////////////////////////////

				newDoor.myDirection = Door_2D.direction.down;
				roomGrid[tile.Item1, tile.Item2] = 2;
				roomGrid[tile.Item1 +1, tile.Item2] = 2;
				roomGrid[tile.Item1 +2, tile.Item2] = 2;
			}

			
			newDoor.DetermineDoorDirection();
			if(IsInstanceValid(doorMan)){
				doorMan.AddDoorToDoorList(newDoor);
				doorMan.CallDeferred("add_child", newDoor);
			}else{
				GD.PrintErr("Door Manager Was not found");
			}
			
			//doorMan.CallDeferred("AddDoorToDoorList", newDoor);
			
		}
	}

	private Vector2I SelectRandomTileFromBank(int xmin, int xmax, int ymin, int ymax){

		int x = GD.RandRange(xmin, xmax);
		int y = GD.RandRange(ymin, ymax);

		Vector2I myTile = new Vector2I(x,y);
		return myTile;

	}
	private void UpdateFloorMap(){

		if(exitNodeTile.Item2 == 0){ 								///Top Door	

			currentRoom = (currentRoom.Item1, currentRoom.Item2 - 1);
			
		}else if(exitNodeTile.Item2 == gridSize-1){				    ///Bottom Door	

			currentRoom = (currentRoom.Item1, currentRoom.Item2 + 1);

		}else if(exitNodeTile.Item1 ==0){							////Left Door	

			currentRoom = (currentRoom.Item1-1, currentRoom.Item2);

		}else if(exitNodeTile.Item1 == gridSize-1){					////Right Door	

			currentRoom = (currentRoom.Item1+1, currentRoom.Item2);
		}	

		floorGrid[currentRoom.Item1, currentRoom.Item2] = 1;
	}

	private void PrintGrid(){

		for(int x = 0; x< gridSize; x++){
			
			for(int y= 0; y < gridSize; y++){

				switch(roomGrid[x,y]){

					case 0:                ////Background Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, new Vector2I(0,0));
					break;
					case 1:                ////Unused                                                
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, new Vector2I(3,3));
					break;
					case 2:                ////Floor Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, SelectRandomTileFromBank(1,5,3,6));
					break;
					case 3:                 ////Wall Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, SelectRandomTileFromBank(8,14,3,4));
					break;
					case 4:                 ////Wall Top Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, SelectRandomTileFromBank(8,14,2,2));
					break;
					case 5:                 ////Wall Top Middle Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, SelectRandomTileFromBank(7,7,3,4));
					break;
					case 6:                 ////Wall Top Corner Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, SelectRandomTileFromBank(7,7,2,2));
					break;
					case 7:                 ////Hallway Floor Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, SelectRandomTileFromBank(7,7,2,2));
					break;
					case 8:                 ////Hallway Wall Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, SelectRandomTileFromBank(7,7,2,2));
					break;
					case 9:                 ////Hallway Wall Top Tiles
						tileMap.SetCell(0, new Vector2I(x + roomOffset.X, y + roomOffset.Y), 0, SelectRandomTileFromBank(7,7,2,2));
					break;

				}

				if(x == gridSize -1 && y == gridSize -1){
					//GD.Print("Printed Last Regular Brick @ " + new Vector2I(x + roomOffset.X, y + roomOffset.Y));
					}
			}
		}

		PrintHallway();	

		Vector2I roomOffsetModified = new Vector2I(0,0);   /// This part finds the new room to be generated and adds its coordinates to the current room offset

		if(exitNodeTile.Item1 == 0){						/////Left Side

			roomOffsetModified = new Vector2I(-(gridSize + 4) ,0);
			entranceNodeTile = (gridSize-1, exitNodeTile.Item2);

		}else if(exitNodeTile.Item1 == gridSize - 1){      ////Right Side

			roomOffsetModified = new Vector2I(gridSize + 4,0);
			entranceNodeTile = (0, exitNodeTile.Item2);

		}else if(exitNodeTile.Item2 == 0){					///// Top Side

			roomOffsetModified = new Vector2I(0,-(gridSize + 4));
			entranceNodeTile = (exitNodeTile.Item1, gridSize-1);

		}else if(exitNodeTile.Item2 == gridSize -1){  		/// Bottom Side

			roomOffsetModified = new Vector2I(0, gridSize + 4);
			entranceNodeTile = (exitNodeTile.Item1, 0);
			
		}

		roomOffset += roomOffsetModified;
		//entranceNodeTile = (exitNodeTile.Item1 +roomOffsetModified.X, exitNodeTile.Item2 + roomOffsetModified.Y);
		
	}

	private void PrintHallway()
	{

		switch (entranceNodeTileType)
		{
			case 1:

				GD.Print("Printing Hallway and Incoming Entrance Node Tile Type is Top");
				hallwayGrid = new int[gridSize, 5];

				for (int x = 0; x < hallwayGrid.GetLength(0); x++)
				{

					for (int y = 0; y < hallwayGrid.GetLength(1); y++)
					{

						if (x >= entranceNodeTile.Item1 && x <= entranceNodeTile.Item1 + 2 )
						{
							hallwayGrid[x, y] = 1;
						}
						if (x == entranceNodeTile.Item1 - 1 && y != 0)
						{
							hallwayGrid[x, y] = 5;
						}
						if (x == entranceNodeTile.Item1 + 3 && y != 0)
						{
							hallwayGrid[x, y] = 6;
						}
						
					}
				}


				break;
			case 2:
				GD.Print("Printing Hallway and Incoming Entrance Node Tile Type is Left");
				hallwayGrid = new int[5, gridSize];

				for (int x = 0; x < hallwayGrid.GetLength(0); x++)
				{
					for (int y = 0; y < hallwayGrid.GetLength(1); y++)
					{

						if (y >= entranceNodeTile.Item2 && y <= entranceNodeTile.Item2 + 2)
						{
							hallwayGrid[x, y] = 1;
						}
						if( y == entranceNodeTile.Item2 -1 && x != 0){

							hallwayGrid[x, y] = 2;
						}
						if( y == entranceNodeTile.Item2 - 2 && x !=0){
							hallwayGrid[x, y] = 3;
						}
						if(y == entranceNodeTile.Item2 + 3 && x != 0){
							hallwayGrid[x, y] = 4;
						}
					}
				}


				break;
			case 3:
				GD.Print("Printing Hallway and Incoming Entrance Node Tile Type is Right");

				hallwayGrid = new int[5, gridSize];

				for (int x = 0; x < hallwayGrid.GetLength(0); x++)
				{

					for (int y = 0; y < hallwayGrid.GetLength(1); y++)
					{
						if (y >= entranceNodeTile.Item2 && y <= entranceNodeTile.Item2 + 2){

							hallwayGrid[x, y] = 1;

						}if( y == entranceNodeTile.Item2 -1 && x != 0){

							hallwayGrid[x, y] = 2;

						}if( y == entranceNodeTile.Item2 - 2 && x !=0){
							hallwayGrid[x, y] = 3;
						}
						if(y == entranceNodeTile.Item2 + 3 && x != 0){
							hallwayGrid[x, y] = 4;
						}
					}
				}
				break;
			case 4:

				GD.Print("Printing Hallway and Incoming Entrance Node Tile Type is Bottom");

				hallwayGrid = new int[gridSize, 5];

				for (int x = 0; x < hallwayGrid.GetLength(0); x++)
				{
					for (int y = 0; y < hallwayGrid.GetLength(1); y++)
					{
						if (x >= entranceNodeTile.Item1 && x <= entranceNodeTile.Item1 + 2 )
						{
							hallwayGrid[x, y] = 1;
						}
						if (x == entranceNodeTile.Item1 - 1 && y!= 0)
						{
							hallwayGrid[x, y] = 5;
						}
						if (x == entranceNodeTile.Item1 + 3 && y !=0)
						{
							hallwayGrid[x, y] = 6;
						}

					}
				}
				break;


		}

		GD.Print("Incoming Entrance Node Tile that we will print from as a base is " + entranceNodeTile);

		for (int x = 0; x < hallwayGrid.GetLength(0); x++) {

			for(int y= 0 ; y < hallwayGrid.GetLength(1); y++){

				/// Based on the direction of the exit door, each VECTOR2I would be set differently
				/// If printing right, positive X value, but if printing left, negative x value.
				/// Also, if printing down, positive y value, but if printing up, negative y value. 
				
		
				switch(hallwayGrid[x,y]){

					case 1:
						tileMap.SetCell(0, OffsetHallwayPrinter(x,y), 0, SelectRandomTileFromBank(1,5,8,10));
					break;
					case 2:
						GD.Print($"PRINTING Hallway Grid at {x} , {y}, true coords: {OffsetHallwayPrinter(x,y)} ---");
						tileMap.SetCell(0, OffsetHallwayPrinter(x,y), 0, SelectRandomTileFromBank(7,11,8,8));
					break;
					case 3:
						tileMap.SetCell(0, OffsetHallwayPrinter(x,y), 0, SelectRandomTileFromBank(7,11,7,7));
					break;
					case 4:
						tileMap.SetCell(0, OffsetHallwayPrinter(x,y), 0, SelectRandomTileFromBank(1,5,8,10));
						tileMap.SetCell(1, OffsetHallwayPrinter(x,y), 0, SelectRandomTileFromBank(7,11,10,10));
					break;
					case 5:
						tileMap.SetCell(0, OffsetHallwayPrinter(x,y), 0, SelectRandomTileFromBank(16,16,8,11));
					break;
					case 6:
						tileMap.SetCell(0, OffsetHallwayPrinter(x,y), 0, SelectRandomTileFromBank(15,15,8,11));
					break;
					case 7:

						tileMap.SetCell(0, OffsetHallwayPrinter(x,y), 0, new Vector2I(18,10));
						tileMap.SetCell(0, OffsetHallwayPrinter(x+1,y), 0, new Vector2I(19,10));
						tileMap.SetCell(0, OffsetHallwayPrinter(x+2,y), 0, new Vector2I(20,10));
						tileMap.SetCell(0, OffsetHallwayPrinter(x,y + 1), 0, new Vector2I(18,11));
						tileMap.SetCell(0, OffsetHallwayPrinter(x + 1,y + 1), 0, new Vector2I(19,11));
						tileMap.SetCell(0, OffsetHallwayPrinter(x + 2,y + 1), 0, new Vector2I(20,11));

					break;
						//tileMap.SetCell(0, new Vector2I(roomOffset.X + x, roomOffset.Y + y), 0, SelectRandomTileFromBank(7,11,10,10));
				}
			}
		}
	}

	private Vector2I OffsetHallwayPrinter(int xVal, int Yval){

		int xTrue = 0;
		int yTrue = 0;

		switch(entranceNodeTileType){
			case 1:			////// TOP /////// 
				xTrue = xVal;
				yTrue = -Yval;
			break;
			case 2:			/////// LEFT ////////
				xTrue = -xVal;
				yTrue = Yval;
			break;
			case 3:			/////// RIGHT ///////
				xTrue = xVal;
				xTrue += entranceNodeTile.Item1;
				yTrue = Yval;
			break;
			case 4:			////// BOTTOM ////////
				xTrue = xVal;
				yTrue = Yval;
				yTrue += entranceNodeTile.Item2;
			break;
		}

		return new Vector2I(roomOffset.X + xTrue, roomOffset.Y 	+ yTrue);

	}
	private void OnFloorResetTimerTimeout(){

		roomLimitReached = false;
		roomOffset = new Vector2I(0,0);
		GenerateNewFloor();
		GenerateNewRoom();
		GlobalSignals.Instance.EmitAdjustCharPosition(new Godot.Vector2(200,200));
		

	}

	private void RefreshLists(){

	}



}
