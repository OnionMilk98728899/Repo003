using Godot;
using System;

public partial class ElementalTile : StaticBody2D
{
    public enum tileType { vines, liteSnow, ice, poisonGas, lavaJet, lavaPool, lavaRiverLeft, lavaRiverRight, lavaFalls }
    [Export]
    private Texture2D vinesTexture, liteSnowTexture, iceTexture, poisonGasTexture, lavaPoolTexture, lavaRiverLeftTexture, lavaRiverRightTexture, lavaFallsTexture;
    [Export] private CollisionShape2D wideCollider, standardCollider;
    [Export] private Sprite2D tileSprite;
    [Export] public tileType myTileType;
    [Export] private StaticBody2D myBody;
    [Export] private AnimationPlayer tileAnim;
    [Export] private PackedScene tempTileScene;
    // private Edible myEdible;
    private TemporaryTile myTempTile;
    private string destroyAnim, idleAnim;

    public void SetTileType(int type)
    {
        myTileType = (tileType)type;
        SetTilePreferences();
    }

    private void SetTilePreferences()
    {
        tileSprite.Hframes = 10;
        switch (myTileType)
        {
            case tileType.vines:
                tileSprite.Texture = vinesTexture;
                destroyAnim = "burn";
                break;
            case tileType.liteSnow:
                tileSprite.Texture = liteSnowTexture;
                destroyAnim = "burn";
                break;
            case tileType.ice:
                tileSprite.Texture = iceTexture;
                idleAnim = "iceidle";
                standardCollider.Disabled = true;
                wideCollider.Disabled = false;
                SetCollisionLayerValue(1, true);
                break;
            case tileType.poisonGas:
                tileSprite.Texture = poisonGasTexture;
                idleAnim = "gasidle";
                SetCollisionLayerValue(15, true);
                break;
            // case tileType.gasTop:
            //     tileSprite.Texture = gasTopTexture;
            //     idleAnim = "gasidle";
            //     SetCollisionLayerValue(7, true);
            //     break;
            // case tileType.gasBottom:
            //     tileSprite.Texture = gasBottomTexture;
            //     idleAnim = "gasidle";
            //     SetCollisionLayerValue(7, true);
            //     break;
            // case tileType.gasLeft:
            //     tileSprite.Texture = gasLeftTexture;
            //     idleAnim = "gasidle";
            //     SetCollisionLayerValue(7, true);
            //     break;
            // case tileType.gasRight:
            //     tileSprite.Texture = gasRightTexture;
            //     idleAnim = "gasidle";
            //     SetCollisionLayerValue(7, true);
            //     break;
            case tileType.lavaPool:
                tileSprite.Texture = lavaPoolTexture;
                tileSprite.Hframes = 16;
                idleAnim = "lavapoolidle";
                standardCollider.Disabled = true;
                wideCollider.Disabled = false;
                SetCollisionLayerValue(7, true);
                break;
            case tileType.lavaRiverLeft:
                tileSprite.Texture = lavaRiverLeftTexture;
                tileSprite.Hframes = 8;
                idleAnim = "lavariveridle";
                standardCollider.Disabled = true;
                wideCollider.Disabled = false;
                SetCollisionLayerValue(7, true);
                break;
            case tileType.lavaRiverRight:
                tileSprite.Texture = lavaRiverRightTexture;
                tileSprite.Hframes = 8;
                idleAnim = "lavariveridle";
                standardCollider.Disabled = true;
                wideCollider.Disabled = false;
                SetCollisionLayerValue(7, true);
                break;
            case tileType.lavaFalls:
                tileSprite.Texture = lavaFallsTexture;
                tileSprite.Hframes = 12;
                idleAnim = "lavafallidle";
                standardCollider.Disabled = true;
                wideCollider.Disabled = false;
                break;
        }

        if (idleAnim != null)
        {
            tileAnim.Play(idleAnim);
        }
    }

    private void DestroyTile()
    {
        if (destroyAnim != null)
        {
            tileAnim.Play(destroyAnim);
        }
        else
        {
            QueueFree();
        }

    }

    private void ShutOffLavaJet()
    {
        if (GetParent() is JetSegment segment)
        {
            if (segment.GetParent() is JetTile tile)
            {
                if (tile.isLocked)
                {
                    tile.DisableLavaJet();
                }
            }

        }
    }

    private void OnDestroyAnimationFinished()
    {
        QueueFree();
    }
    private void OnElementDetectorBodyEntered(Node2D body)
    {
        
        if (body is Edible edible)
        {
            if (edible.myEdibleType == Edible.edibleType.water)
            {
                if (myTileType == tileType.lavaJet && edible.isFlying)
                {
                    ShutOffLavaJet();
                }
                else if (myTileType == tileType.lavaPool || myTileType == tileType.lavaRiverLeft 
                || myTileType == tileType.lavaRiverRight || myTileType == tileType.lavaFalls)
                {
                    if (!edible.hasTriggeredElement)
                    {
                       CreateTemporaryTile(GlobalPosition, tempTileType.rock); 
                       edible.hasTriggeredElement = true;
                    }
                }else if (myTileType == tileType.ice)
                {
                    
                    Vector2 newPos = Vector2.Zero;
                    float yDist = edible.GlobalPosition.Y - GlobalPosition.Y;
                    float xDist = edible.GlobalPosition.X - GlobalPosition.X;
                    GD.Print("X: " +xDist+ " Y: " +yDist);
                    if(MathF.Abs(yDist) < 12 && edible.GlobalPosition.X < GlobalPosition.X)
                    {
                        newPos = new Vector2(GlobalPosition.X - 16, GlobalPosition.Y);
                    }else if(MathF.Abs(yDist) < 12 && edible.GlobalPosition.X > GlobalPosition.X)
                    {
                        newPos = new Vector2(GlobalPosition.X + 16, GlobalPosition.Y);
                    }else if(yDist < 0)
                    {
                        newPos = new Vector2(GlobalPosition.X, GlobalPosition.Y - 16);
                    }else if(yDist > 0)
                    {
                        newPos = new Vector2(GlobalPosition.X, GlobalPosition.Y + 16);
                    }

                    if (!edible.hasTriggeredElement)
                    {
                       CreateTemporaryTile(newPos, tempTileType.ice); 
                       edible.hasTriggeredElement = true;
                    }
                    
                }
            }
            else if (edible.myEdibleType == Edible.edibleType.fire)
            {
                if (myTileType == tileType.liteSnow || myTileType == tileType.vines)
                {
                    DestroyTile();
                }
            }

            else if (edible.myEdibleType == Edible.edibleType.air)
            {
                if(myTileType == tileType.poisonGas)
                {
                    DestroyTile();
                }
                
            }
        }
    }


    private void CreateTemporaryTile(Vector2 position, tempTileType type)
    {
        GD.Print("Added temp tile at " + position);
        myTempTile = tempTileScene.Instantiate<TemporaryTile>();

        if(Mathf.Abs(GlobalStats.Instance.playerLocation.X - position.X) < 8 
        && Mathf.Abs(GlobalStats.Instance.playerLocation.Y - position.Y) < 8)
        {
            myTempTile.FormTile(type, true);
        }
        else
        {
            myTempTile.FormTile(type, false);
        }
        
        CallDeferred("AddChildTile", myTempTile, position);
    }

    private void AddChildTile(TemporaryTile tile, Vector2 position)
    {
        AddChild(tile);
        tile.GlobalPosition = position;
        GD.Print("Child Added");
    }

    // private void AddChildEdible(Edible edi)
    // {
    //     AddChild(edi);
    // }
}
