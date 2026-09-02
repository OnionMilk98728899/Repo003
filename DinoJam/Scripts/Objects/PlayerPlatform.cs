using Godot;
using System;

public partial class PlayerPlatform : CharacterBody2D
{

    private Player myPlayer;
    private TileMap myTileMap;
    [Export] private float moveSpeed, gravity, maxGravity;

    private Vector2I tilePosition;
    private Vector2 platformVelocity, waterVelocity;
    private bool isTouchingWater, isTouchingWaterfall, isTouchingPlayer;
    [Export] private Label debugLabel;
    private bool isTriggered;
    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(double delta)
    {
        Float(delta);
        debugLabel.Text = platformVelocity.ToString();
    }

    private void Float(double delta)
    {
        if (isTouchingWater && isTouchingPlayer)
        {
            isTriggered = true;
        }

        if (isTriggered)
        {
            Vector2 samplePosition = GlobalPosition + new Vector2(0, 1);

            tilePosition = myTileMap.LocalToMap(myTileMap.ToLocal(samplePosition));
            TileData tileData = myTileMap.GetCellTileData(0, tilePosition);

            if (tileData != null)
            {
                waterVelocity = (Vector2)tileData.GetCustomData("movement");
                platformVelocity = platformVelocity.MoveToward(waterVelocity, moveSpeed * (float)delta);
            }

            if (!isTouchingWater && !isTouchingWaterfall && platformVelocity.Y < maxGravity)
            {
                platformVelocity.Y += gravity;
            }
        }
        Velocity = platformVelocity;
        MoveAndSlide();
    }
    private void OnPlayerDetectorBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player") && body is Player player)
        {
            myPlayer = player;
            isTouchingPlayer = true;
        }
    }

    private void OnWaterDetectorBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Hazard") && body is TileMap tileMap)
        {
            myTileMap = tileMap;
            isTouchingWater = true;
        }
    }

    private void OnWaterDetectorBodyExited(Node2D body)
    {
        if (body.IsInGroup("Hazard") && body is TileMap tileMap)
        {
            isTouchingWater = false;
        }
    }

    private void OnWaterfallDetectorBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Hazard") && body is TileMap tileMap)
        {
            myTileMap = tileMap;
            isTouchingWaterfall = true;
        }
    }

    private void OnWaterfallDetectorBodyExited(Node2D body)
    {
        isTouchingWaterfall = false;
    }

}
