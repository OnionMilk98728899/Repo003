using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class MapGenerator : Node2D
{
	[Export] private TileMap tileMap;
	[Export] private TileSet tileSet;

	[Export] private int gridSize;
	private int tileQuantity, nodeTileQuantity;
	private (int,int) minimumDistance, medianDistance, distance;
	private int[,] grid;
	private List<(int,int)> myNodeTiles, nodeTilesToAdd;
	//private (int,int)[] odeTiles;

	public override void _Ready()
	{
		
		grid = new int[gridSize, gridSize];

		PlaceNodeTiles();
		ExpandNodeTiles();	
		
		foreach((int,int) tile in myNodeTiles){
			tileMap.SetCell(0,new Vector2I(tile.Item1, tile.Item2), 0, new Vector2I(2,1));
		}

		foreach((int,int) tile in nodeTilesToAdd){
			tileMap.SetCell(0,new Vector2I(tile.Item1, tile.Item2), 0, new Vector2I(1,3));
		}
		
	}

	public override void _PhysicsProcess(double delta)
	{

	}

	private void PlaceNodeTiles(){

		nodeTileQuantity =  GD.RandRange(3, gridSize/2);

		GenerateNodeTiles(nodeTileQuantity);
		CheckAndAddBorderTiles();
		//CheckAndFillIslands();


		for (int z = 0; z < myNodeTiles.Count; z++)  //// Looping through the node tiles array for each grid space
		{
			GD.Print($"NodeTiles {z} = {myNodeTiles[z]} ");
		}

		bool nodeMatch = false;

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)		//// Looping through the grid
			{
				nodeMatch = false;
				for (int z = 0; z < myNodeTiles.Count; z++)	//// Looping through the node tiles array for each grid space
				{
					
					if(myNodeTiles[z].Item1 == x && myNodeTiles[z].Item2 == y){
						nodeMatch = true;
					}
				}

				if(nodeMatch){
					grid[x,y] = 1;
					tileMap.SetCell(0,new Vector2I(x, y), 0, new Vector2I(2,1));
				}else{
					grid[x,y] = 0;
					tileMap.SetCell(0,new Vector2I(x, y), 0, new Vector2I(1,1));
				}
			}
		}
	}

	private void GenerateNodeTiles(int quantity){

		//nodeTiles = new (int,int)[quantity];
		myNodeTiles = new List<(int,int)>(quantity);
		GD.Print($"Quantity is {quantity} NodeTile List Count is {myNodeTiles.Count}");

		for (int x = 0; x < quantity; x++)
		{
			myNodeTiles.Add((GD.RandRange(0, gridSize-1), GD.RandRange(0, gridSize-1))) ;
			GD.Print($"NodeTile {myNodeTiles[x]} was properly Generated");
		}
	}

	private void CheckAndAddBorderTiles(){

		bool borderTileChecker = false;
		int borderTileCount = 0;
		foreach((int,int) tile in myNodeTiles){

			if(tile.Item1 == 0){
				borderTileChecker = true;
			}
			if(borderTileChecker){
				borderTileCount++;
				borderTileChecker = false;
			}
		}
		foreach((int,int) tile in myNodeTiles){
			
			if(tile.Item1 == gridSize - 1){
				borderTileChecker = true;
			}
			if(borderTileChecker){
				borderTileCount++;
				borderTileChecker = false;
			}
		}
		foreach((int,int) tile in myNodeTiles){

			if(tile.Item2 == 0){
				borderTileChecker = true;
			}
			if(borderTileChecker){
				borderTileCount++;
				borderTileChecker = false;
			}
		}
		foreach((int,int) tile in myNodeTiles){
			
			if(tile.Item2 == gridSize-1){
				borderTileChecker = true;
			}
			if(borderTileChecker){
				borderTileCount++;
				borderTileChecker = false;
			}
		}


		GD.Print($"{borderTileCount} tiles bordering the grid wall");

		if(borderTileCount <=1){

			int borderPlacer = GD.RandRange(0,5);

			switch(borderPlacer){
				case 0:
					myNodeTiles.Add((0, GD.RandRange(0, gridSize-1)));  // Left Side Border Wall
					myNodeTiles.Add((gridSize-1, GD.RandRange(0, gridSize-1)));	// Right Side Border Wall
				break;
				case 1:
					myNodeTiles.Add((0, GD.RandRange(0, gridSize-1)));  // Left Side Border Wall
					myNodeTiles.Add((GD.RandRange(0, gridSize-1), 0 )) ; // Top Side Border Wall
				break;
				case 2:
					myNodeTiles.Add((GD.RandRange(0, gridSize-1), 0 )) ; // Top Side Border Wall
					myNodeTiles.Add((GD.RandRange(0, gridSize-1), gridSize-1)) ; // Bottom Side Border Wall
				break;
				case 3:
					myNodeTiles.Add((0, GD.RandRange(0, gridSize-1)));  // Left Side Border Wall
					myNodeTiles.Add((GD.RandRange(0, gridSize-1), gridSize-1 )) ; // Bottom Side Border Wall
				break;
				case 4:
					myNodeTiles.Add((gridSize-1, GD.RandRange(0, gridSize-1)));	// Right Side Border Wall
					myNodeTiles.Add((GD.RandRange(0, gridSize-1), 0 )) ; // Top Side Border Wall
				break;
				case 5:
					myNodeTiles.Add((gridSize-1, GD.RandRange(0, gridSize-1)));	// Right Side Border Wall
					myNodeTiles.Add((GD.RandRange(0, gridSize-1), gridSize-1 )) ; // Bottom Side Border Wall
				break;
			}
			GD.Print($"Added Border Tiles! --  tiles set according to scheme {borderPlacer}");
		}
	}

	private void CheckAndFillIslands(){

		nodeTilesToAdd = new List<(int, int)>();

		foreach ((int, int) tile in myNodeTiles){
			GD.Print($"Tile ({tile.Item1},{tile.Item2})");
			minimumDistance = (100,100);

			foreach ((int, int) tileCheck in myNodeTiles){

				distance = ((int)MathF.Abs(tile.Item1 - tileCheck.Item1), (int)MathF.Abs(tile.Item2 - tileCheck.Item2));

				GD.Print($"\t..distance to tile: ({tileCheck.Item1},{tileCheck.Item2}) is ({distance.Item1},{distance.Item2})");

				if(distance.Item1 < minimumDistance.Item1 || distance.Item2 < minimumDistance.Item2){

					if(distance.Item1 != 0 && distance.Item2 != 0){
						minimumDistance = distance;

						if(tile.Item1 > tileCheck.Item1){
							medianDistance.Item1 = tile.Item1 - (minimumDistance.Item1/2);
						}else{
							medianDistance.Item1 = tile.Item1 + (minimumDistance.Item1/2);
						}

						if(tile.Item2 > tileCheck.Item2){
							medianDistance.Item2 = tile.Item2 - (minimumDistance.Item2/2);
						}else{
							medianDistance.Item2 = tile.Item2 + (minimumDistance.Item2/2);
						}
						
					}
					
				}

			}

			//GD.Print($"Minimum distance to Node ({tile.Item1},{tile.Item2}) is ({minimumDistance.Item1},{minimumDistance.Item2})");

			if(minimumDistance.Item1 > 8 || minimumDistance.Item2 > 8){

				nodeTilesToAdd.Add(medianDistance);
				GD.Print($"Added Median Node at ({medianDistance.Item2}, {medianDistance.Item2})");
			}
			
		}

		foreach((int,int) tile in nodeTilesToAdd){
			myNodeTiles.Add(tile);
		}
	}

	private void ExpandNodeTiles(){

		foreach ((int, int) tile in myNodeTiles)
		{
			int xpandDegreeX = GD.RandRange(2, 5);
			int xpandDegreeY = GD.RandRange(2, 5);			/// Set the expansion degree for each node tile randomly

			//GD.Print($"Expand Degree for nodeTile {tile} is ({xpandDegreeX}, {xpandDegreeY})");

			for (int x = tile.Item1 - xpandDegreeX; x <= tile.Item1 + xpandDegreeX; x++)   /// for each tile within the X-axis' expansion degree...
			{	
				for (int y = tile.Item2 - xpandDegreeY; y <= tile.Item2 + xpandDegreeY; y++) /// and for each tile within the Y-axis' expansion degree
				{
					if(x >= 0 && x < gridSize && y >=0 && y < gridSize ){

						//GD.Print("Attempting to flip Grid at " +x+ ", " +y);
						grid[x,y] = 2;
						tileMap.SetCell(0,new Vector2I(x, y), 0, new Vector2I(1,2));
					}
					if(x == tile.Item1 && y== tile.Item2){

						tileMap.SetCell(0,new Vector2I(x, y), 0, new Vector2I(2,1));
					}
					
				}
			}
		}
	}


	
	
}
