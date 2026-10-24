using Godot;
using System;

public partial class ProjectileManager : Node2D
{
    public static ProjectileManager Instance {get; private set;}
    [Export] private PackedScene particleScene, edibleScene;
    private TimedParticles myParticles;
    private Edible myEdibleProjectile;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public void GenerateCeilingCrumble(Vector2 position, float lifetime)
    {
        myParticles = particleScene.Instantiate<TimedParticles>();
        myParticles.GlobalPosition = position;
        
        AddChild(myParticles);
        myParticles.SetLifetime(lifetime);
    }

    public void GenerateFallingRock(Vector2 position)
    {
        myEdibleProjectile = edibleScene.Instantiate<Edible>();
        myEdibleProjectile.GlobalPosition = position;
        AddChild(myEdibleProjectile);
        myEdibleProjectile.myEdibleType = Edible.edibleType.rock;
        myEdibleProjectile.SetPlayerDamagingComponent();
        //GD.Print("Rock Position is " + position);
    }


}
