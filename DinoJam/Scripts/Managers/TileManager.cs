using System.Collections.Generic;
using Godot;

public partial class TileManager : Node2D
{
    private bool anyActive;

    [Export] private TileMap breakableTileMap;
    [Export] private PackedScene breakableTileScene;
    private BreakableTile myBreakableTile;
    private Godot.Collections.Array<Vector2I> activeTiles;
    private List<Vector2> tilePositionList = new List<Vector2>();

    public override void _Ready()
    {
        ConvertTileMapToBreakableTiles();
    }
    public bool CheckForOtherAdjacentSideTilesDetectingPlayer(bool isLeft)
    {
        anyActive = false;
        foreach(Node node in GetChildren())
        {
            if(node is BreakableTile tile)
            {
                if (isLeft)
                {
                    if (tile.isLeftDetectorActive)
                    {
                        anyActive = true;
                    }
                }
                else
                {
                    if (tile.isRightDetectorActive)
                    {
                        anyActive = true;
                    }
                }
            }
        }
        return anyActive;
    }

    public bool CheckForOtherAdjacentTopBottomTilesDetectingPlayer(bool isTop)
    {
        anyActive = false;
        foreach(Node node in GetChildren())
        {
            if(node is BreakableTile tile)
            {
                if (isTop)
                {
                    if (tile.isTopDetectorActive)
                    {
                        anyActive = true;
                    }
                }
                else
                {
                    if (tile.isBottomDetectorActive)
                    {
                        anyActive = true;
                    }
                }  
            }
        }
        return anyActive;
    }

    public void ConvertTileMapToBreakableTiles()
    {
        activeTiles = breakableTileMap.GetUsedCells(0);
        foreach(Vector2I cell in activeTiles)
        {
            tilePositionList.Add(breakableTileMap.MapToLocal(cell));
            breakableTileMap.SetCell(0, cell, -1);
        }

        foreach(Vector2 cell in tilePositionList)
        {
            myBreakableTile = breakableTileScene.Instantiate<BreakableTile>();
            myBreakableTile.GlobalPosition = cell;
            AddChild(myBreakableTile);
        }
    }
}
