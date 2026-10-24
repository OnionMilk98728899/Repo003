using Godot;
using System;
public enum tempTileType { ice, rock }
public partial class TemporaryTile : StaticBody2D
{
    [Export] public tempTileType myTileType;
    [Export] private Texture2D iceTexture, rockTexture;
    [Export] private Sprite2D tileSprite;
    [Export] private AnimationPlayer tileAnim;
    [Export] private CollisionShape2D tileCollider;
    [Export] private Timer breakTimer;

    private bool isBroken;

    public override void _Ready()
    {
        breakTimer.Start();
    }


    private void OnBreakTimerTimeout()
    {
        tileAnim.Play("break");
    }

    private void OnBreakAnimationFinished()
    {
        QueueFree();
    }


    public void FormTile(tempTileType type, bool broken)
    {
        if (!broken)
        {
            myTileType = type;
            DetermineTileType(myTileType);
            tileAnim.Play("form");
        }
        else
        {
            QueueFree();
        }

    }

    private void DetermineTileType(tempTileType type)
    {
        if(type == tempTileType.ice)
        {
            tileSprite.Texture = iceTexture;
        }
        else
        {
            tileSprite.Texture = rockTexture;
        }
    }

}
