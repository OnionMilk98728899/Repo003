using Godot;
using System;

public partial class WaterSource : StaticBody2D
{
    [Export] private PackedScene dropletScene;
    [Export] private AnimationPlayer waterAnim;
    private Edible myDrop;

    public override void _Ready()
    {
        waterAnim.Play("drip");
    }

    private void CreateDroplet()         /////////// Called in animation player //////////////
    {
        myDrop = dropletScene.Instantiate<Edible>();
        AddChild(myDrop);
        myDrop.GlobalPosition = GlobalPosition;
    }

}
