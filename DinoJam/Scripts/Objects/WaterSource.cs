using Godot;
using System;

public partial class WaterSource : StaticBody2D
{
    [Export] private Texture2D waterSourceTexture, airSourceTexture;
    [Export] private PackedScene dropletScene;
    [Export] private AnimationPlayer waterAnim;
    private string emitAnim;
    private Vector2 spawnOffset;
    [Export]private Sprite2D sourceSprite;
    [Export] bool isAirVent;
    private Edible myDrop;

    public override void _Ready()
    {
        if (!isAirVent)
        {
            sourceSprite.Texture = waterSourceTexture;
            sourceSprite.Hframes = 16;
            emitAnim ="drip";
        }
        else
        {
            sourceSprite.Texture = airSourceTexture;
            sourceSprite.Hframes = 26;
            emitAnim = "bubble";
            SetCollisionLayerValue(1, true);
        }
        waterAnim.Play(emitAnim);
    }

    private void CreateDroplet()         /////////// Called in animation player //////////////
    {
        myDrop = dropletScene.Instantiate<Edible>();
        AddChild(myDrop);
        if (!isAirVent)
        {
            myDrop.myEdibleType = Edible.edibleType.water;
            spawnOffset = Vector2.Zero;
        }
        else
        {
            myDrop.myEdibleType = Edible.edibleType.air;
            spawnOffset = new Vector2(0, -16);
        }
        
        myDrop.SetParticlesAndSprites();
        myDrop.GlobalPosition = GlobalPosition +spawnOffset;
    }

}
