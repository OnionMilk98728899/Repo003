using Godot;
using System;

public partial class Magic : Node2D
{
    [Export] private CharacterBody2D magicBody;
    [Export] private Sprite2D magicSprite;
    [Export] private AnimationPlayer magicAnim;

    public enum unitType{ player, enemy}
    public unitType currentUnitType;

    public enum magicType
    {
        red, orange, green, blue, purple
    }
    public magicType currentMagicType;
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
    
    }



}
