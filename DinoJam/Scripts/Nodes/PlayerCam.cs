using Godot;
using System;

public partial class PlayerCam : Camera2D
{
    [Export]
    private CharacterBody2D player;

    [Export]
    private float followSpeed ;

    public override void _PhysicsProcess(double delta)
    {
        float weight = followSpeed * (float)delta;

        GlobalPosition = GlobalPosition.Lerp(
            player.GlobalPosition,
            weight
        );
    }
}
