using Godot;
using System;
using Game.MagicTypes;

public partial class PlayerAttacks : Node2D
{
    [Signal] public delegate void CastMagicEventHandler();
    [Export] private PackedScene myMagicScene;
    private Magic myMagic;

    public override void _PhysicsProcess(double delta)
    {
        //HandleInstrumentInput();
    }



    public void FireProjectile(ProjectileColor color, Vector2 inputDirection)
    {
        myMagic = myMagicScene.Instantiate<Magic>();
        RhythmManager.Instance.AddChild(myMagic);
        myMagic.SetMagicStats(color, inputDirection, Magic.unitType.player, 180);
        myMagic.GlobalPosition = GlobalPosition;
        GD.Print("MagicMade");
    }
}
