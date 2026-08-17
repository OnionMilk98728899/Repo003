using Godot;
using System;

public partial class BreakableTile : StaticBody2D
{
    [Export] private Sprite2D blockSprite;
    [Export] private Timer destroyTimer;
    [Export] private CollisionShape2D blockCollider;
    [Export] private Area2D topDetector, leftDetector, rightDetector, bottomDetector;
    [Export] private GpuParticles2D blockParticles, dustParticles;
    private Player player;
    private Edible edible;
    private bool isBroken;
    public bool isRightDetectorActive, isLeftDetectorActive, isBottomDetectorActive, isTopDetectorActive;

    public override void _Ready()
    {
        EventBus.Instance.BreakableTileBroken += OnBreakableTileBroken;
    }

    private void BreakTile()
    {
        if (!isBroken)
        {
            blockSprite.Visible = false;
            //blockCollider.Disabled = true;
            DisableCollisions();
            SetCollisionLayerValue(1, false);
            blockParticles.Restart();
            dustParticles.Restart();
            destroyTimer.Start();
            isBroken = true;
            if(player!=null){player.tileBreakBufferTimer.Start();}
            ///EventBus.Instance.EmitSignal(EventBus.SignalName.BreakableTileBroken);
        }

    }
    private void OnTopDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            player.SetCurrentBreakableTileTop(true, this);
            isTopDetectorActive = true;
        }

        if (body.IsInGroup("Edible"))
        {
            edible = body.GetNode<Edible>(".");
            if (edible.GetFlyingStatus())
            {
                BreakTile();
                edible.Destroy();
            }
        }
    }

    private void OnBottomDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            player.SetCurrentBreakableTileBottom(true, this);
            isBottomDetectorActive = true;
        }
        if (body.IsInGroup("Edible"))
        {
            edible = body.GetNode<Edible>(".");
            if (edible.GetFlyingStatus())
            {
                BreakTile();
                edible.Destroy();
            }
        }
    }

    private void OnLeftDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            player.SetCurrentBreakableTileLeft(true, this);
            isLeftDetectorActive = true;
        }
        if (body.IsInGroup("Edible"))
        {
            edible = body.GetNode<Edible>(".");
            if (edible.GetFlyingStatus())
            {
                BreakTile();
                edible.Destroy();
            }
        }
    }

    private void OnRightDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            player.SetCurrentBreakableTileRight(true, this);
            isRightDetectorActive = true;

        }
        if (body.IsInGroup("Edible"))
        {
            edible = body.GetNode<Edible>(".");
            if (edible.GetFlyingStatus())
            {
                BreakTile();
                edible.Destroy();
            }
        }
    }

    private void OnTopDetectorExited(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");

            isTopDetectorActive = false;

            TileManager tileMan = GetNode<TileManager>("..");
            if (!tileMan.CheckForOtherAdjacentTopBottomTilesDetectingPlayer(true))  //// If no other breakable tiles detect the player from the same direction, set Player's seeking to false; 
            {
                player.SetCurrentBreakableTileTop(false, this);
            }

            //player.SetCurrentBreakableTileBottom(false, this);

        }
    }
    private void OnBottomDetectorExited(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");

            isBottomDetectorActive = false;

            TileManager tileMan = GetNode<TileManager>("..");
            if (!tileMan.CheckForOtherAdjacentTopBottomTilesDetectingPlayer(false))  //// If no other breakable tiles detect the player from the same direction, set Player's seeking to false; 
            {
                player.SetCurrentBreakableTileBottom(false, this);
            }

            //player.SetCurrentBreakableTileBottom(false, this);

        }
    }
    private void OnLeftDetectorExited(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");

            isLeftDetectorActive = false;

            TileManager tileMan = GetNode<TileManager>("..");
            if (!tileMan.CheckForOtherAdjacentSideTilesDetectingPlayer(true))  //// If no other breakable tiles detect the player from the same direction, set Player's seeking to false; 
            {
                player.SetCurrentBreakableTileLeft(false, this);
            }
        }
    }

    private void OnRightDetectorExited(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");

            isRightDetectorActive = false;

            TileManager tileMan = GetNode<TileManager>("..");
            if (!tileMan.CheckForOtherAdjacentSideTilesDetectingPlayer(false))
            {
                player.SetCurrentBreakableTileRight(false, this);
            }
        }
    }

    private void DisableCollisions()
    {
            leftDetector.SetCollisionMaskValue(4, false);
            rightDetector.SetCollisionMaskValue(4, false);
            topDetector.SetCollisionMaskValue(4, false);
            bottomDetector.SetCollisionMaskValue(4, false);
    }



    private void OnDestroyTimerTimeout()
    {
        QueueFree();
    }

    private void OnBreakableTileBroken(BreakableTile tile)
    {
        if (tile == this)
        {
            BreakTile();
        }
    }


}
