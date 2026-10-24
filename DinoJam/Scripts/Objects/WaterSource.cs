using Godot;
using System;

public partial class WaterSource : StaticBody2D
{
    private enum sourceType { water, air, fire }
    [Export] private sourceType mySourceType;
    [Export] private Texture2D waterSourceTexture, airSourceTexture;
    [Export] private PackedScene dropletScene;
    [Export] private AnimationPlayer waterAnim;
    [Export] private Timer flameRespawnTimer;
    private string emitAnim;
    private Vector2 spawnOffset;
    [Export] private Sprite2D sourceSprite;
    //[Export] bool isAirVent;
    private Edible myDrop;

    public override void _Ready()
    {
        if (mySourceType == sourceType.water)
        {
            sourceSprite.Texture = waterSourceTexture;
            sourceSprite.Hframes = 16;
            emitAnim = "drip";
            waterAnim.Play(emitAnim);
        }
        else if (mySourceType == sourceType.air)
        {
            sourceSprite.Texture = airSourceTexture;
            sourceSprite.Hframes = 26;
            emitAnim = "bubble";
            SetCollisionLayerValue(1, true);
            waterAnim.Play(emitAnim);
        }
        else if (mySourceType == sourceType.fire)
        {
            sourceSprite.Texture = airSourceTexture;
            sourceSprite.Hframes = 26;
            SetCollisionLayerValue(1, true);
            CreateFlame();
        }

    }

    private void CreateDroplet()         /////////// Called in animation player //////////////
    {
        myDrop = dropletScene.Instantiate<Edible>();
        AddChild(myDrop);
        if (mySourceType == sourceType.water)
        {
            myDrop.myEdibleType = Edible.edibleType.water;
            spawnOffset = Vector2.Zero;
        }
        else if (mySourceType == sourceType.air)
        {
            myDrop.myEdibleType = Edible.edibleType.air;
            spawnOffset = new Vector2(0, -16);

        }

        myDrop.SetParticlesAndSprites();
        myDrop.GlobalPosition = GlobalPosition + spawnOffset;
    }

    private void CreateFlame()
    {
        myDrop = dropletScene.Instantiate<Edible>();
        AddChild(myDrop);
        myDrop.myEdibleType = Edible.edibleType.fire;
        spawnOffset = new Vector2(0, -16);
        myDrop.SetParticlesAndSprites();
        myDrop.GlobalPosition = GlobalPosition + spawnOffset;
    }

    public void StartFlameRespawn()
    {
        flameRespawnTimer.Start();
    }

    private void OnFlameRespawnTimerTimeout()
    {
        CreateFlame();
    }
}
