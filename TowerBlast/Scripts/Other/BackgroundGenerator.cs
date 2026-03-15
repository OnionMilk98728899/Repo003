using Godot;
using System;

public partial class BackgroundGenerator : Node2D
{
	[Export] private TileMap _myBackground;
	private Vector2 _mapCoords;
	private Vector2I _myAtlasCoords;
	private Vector2I _myGrid;
	private int HORIZONTAL_GRID_SIZE = 31, VERTICAL_GRID_SIZE = 30, HORIZONTAL_OFFSET = -3;
   
	public override void _Ready()
	{
		_myGrid = new Vector2I(HORIZONTAL_GRID_SIZE, VERTICAL_GRID_SIZE);
		//DrawBackground();
	}

	// private void DrawBackground()
	// {
	//     for(int x =  0; x < _myGrid.X; x++)
	//     {
	//         for(int y =  0; y < _myGrid.Y; y++)
	//         {
	//              _myBackground.SetCell(0, new Vector2I(x + HORIZONTAL_OFFSET,-y),2, new Vector2I(8,10));
	//             if(y> 0 && y < VERTICAL_GRID_SIZE / 2)
	//             {
	//                  _myBackground.SetCell(0, new Vector2I(x + HORIZONTAL_OFFSET,-y),2, new Vector2I(2,8));
	//             }
	//             if(y == VERTICAL_GRID_SIZE / 2)
	//             {
	//                  _myBackground.SetCell(1, new Vector2I(x + HORIZONTAL_OFFSET,-y),2, new Vector2I(1,8));
	//             }
	//             if(y == VERTICAL_GRID_SIZE/2 + 1)
	//             {
	//                 _myBackground.SetCell(1, new Vector2I(x + HORIZONTAL_OFFSET,-y),2, new Vector2I(1,7));
	//             }
	//             if(y > 0 && y < VERTICAL_GRID_SIZE/ 4)
	//             {
	//                 _myBackground.SetCell(1, new Vector2I(x + HORIZONTAL_OFFSET,-y),2, new Vector2I(3,8));
	//             }
	//             if(y >= VERTICAL_GRID_SIZE/4 && y < VERTICAL_GRID_SIZE/4 +3)
	//             {
	//                 _myBackground.SetCell(1, new Vector2I(x + HORIZONTAL_OFFSET,-y),2, new Vector2I(3,7));
	//             }
	//             if(y == VERTICAL_GRID_SIZE / 4 + 3)
	//             {
	//                 _myBackground.SetCell(2, new Vector2I(x + HORIZONTAL_OFFSET,-y),0, new Vector2I(0,0));
	//             }
	//             if(y == VERTICAL_GRID_SIZE / 4)
	//             {
	//                 _myBackground.SetCell(2, new Vector2I(x + HORIZONTAL_OFFSET,-y),0, new Vector2I(0,4));
	//             }
	//         }
	//     }
	   
	// }

	public void AddAdiitionalBackgroundTiles(int yCoordBase)
	{
		GD.Print("Y coord base is  = " + yCoordBase);
		for(int x = 0; x < _myGrid.X; x++)
		{
			for(int y= 0; y < _myGrid.Y; y++)
			{
				_myBackground.SetCell(0, new Vector2I(x + HORIZONTAL_OFFSET,-(y + yCoordBase)),2, new Vector2I(8,10));
			}
		}
	}


}
