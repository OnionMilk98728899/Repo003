using System.Collections.Generic;
using Godot;

public partial class TileManager : Node2D
{
    private bool anyActive;

    [Export] private TileMap breakableTileMap;
    [Export] private PackedScene breakableTileScene, elementalTileScene;
    [Export] private Sprite2D blockSprite;
    public enum tileType{breakable, crate, eggcrate}
    public tileType myTileType;
    private BreakableTile myBreakableTile;
    private Godot.Collections.Array<Vector2I> activeTiles;
    private List<Vector2> tilePositionList = new List<Vector2>();

    public override void _Ready()
    {
        ConvertTileMapToObject(breakableTileMap, tileType.breakable, 0);
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

    public void ConvertTileMapToObject(TileMap map, tileType type, int layer)
    {
        activeTiles = map.GetUsedCells(layer);

        foreach(Vector2I cell in activeTiles)
        {
            GD.Print(cell);
        }

        foreach(Vector2I cell in activeTiles)
        {

            tilePositionList.Add(breakableTileMap.MapToLocal(cell));
            CreateTileObjectOfType(breakableTileMap.MapToLocal(cell), type);
            if(map == breakableTileMap)
            {
                ConvertBreakableTileDirectionalData(map);
            }
            
        }

        foreach(Vector2I cell in activeTiles)
        {
            map.SetCell(layer, cell, -1);
        }
    }

    private void CreateTileObjectOfType(Vector2 position, tileType type)
    {
        switch (type)
        {
            case tileType.breakable:
            myBreakableTile = breakableTileScene.Instantiate<BreakableTile>();
            myBreakableTile.GlobalPosition = position;
            AddChild(myBreakableTile);
            break;
            case tileType.crate:
            break;
            case tileType.eggcrate:
            break;

        }
    }

    private void ConvertBreakableTileDirectionalData(TileMap tMap)
    {
         foreach(Node node in GetChildren())
        {  
            if(node is BreakableTile tile)
            {
                // Vector2 newPos = (tile.GlobalPosition/16) - new Vector2(.5f, .5f);
                // GD.Print("Tile position is "+ newPos);
                int direction = (int)tMap.GetCellTileData(0, (Vector2I)tile.GlobalPosition/16).GetCustomData("directionInt");
                int color = (int)tMap.GetCellTileData(0, (Vector2I)tile.GlobalPosition/16).GetCustomData("colorInt");

                tile.blockSprite.Frame = (color * 5) + direction;
                
                switch (direction)
                {
                    case 1:
                    tile.myDirectionType = BreakableTile.directionalType.top;
                    break;
                    case 2:
                    tile.myDirectionType = BreakableTile.directionalType.left;
                    break;
                    case 3:
                    tile.myDirectionType = BreakableTile.directionalType.bottom;
                    break;
                    case 4:
                    tile.myDirectionType = BreakableTile.directionalType.right;
                    break;
                }
                tile.InitializeCollisions();
            }
        }
    }
}
