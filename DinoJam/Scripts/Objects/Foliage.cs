using Godot;
using System;

public partial class Foliage : StaticBody2D
{
    [Export] private Texture2D foliage1Texture, foliage2Texture, foliage3Texture;
    [Export] private PackedScene myEdibleScene;
    private Edible myEdible;
    [Export] private AnimationPlayer plantAnim;
    [Export] private Sprite2D foliageSprite;
    [Export] private Timer destroyTimer;
    [Export] private GpuParticles2D leafParticles;
    private Player player;


    public override void _Ready()
    {
        int rand = GD.RandRange(0,2);
        switch (rand)
        {
            case 0:
            foliageSprite.Texture = foliage1Texture;
            break;
            case 1:
            foliageSprite.Texture = foliage2Texture;
            break;
            case 2:
            foliageSprite.Texture = foliage3Texture;
            break;
        }

        plantAnim.Play("idle");
    }

    private void DestroyPlant()
    {
        foliageSprite.Visible = false;
        CreateRandomEdible();
        leafParticles.Restart();
        destroyTimer.Start();
    }


    private void OnPlayerDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            player = body.GetNode<Player>(".");
            if(player.GetSpecialState() == Player.specialState.charge || 
            player.GetSpecialState() == Player.specialState.stomp)
            {
                DestroyPlant();
            }
        }
    }

    private void CreateRandomEdible()
    {
        float rand = GD.Randf();
        if (rand > .5)
        {
            myEdible = myEdibleScene.Instantiate<Edible>();
            EdibleManager.Instance.CallDeferred(Node.MethodName.AddChild, myEdible);
            myEdible.isPoppingOut = true;
            myEdible.SetRandomEdibleType();
            myEdible.SetParticlesAndSprites();
            int randX = GD.RandRange(-50, 50);
            Vector2 direction = new Vector2(randX, -100);
            myEdible.GlobalPosition = GlobalPosition;
            myEdible.SetInitialDirection(direction);
            
        }
    }

    private void OnDestroyTimerTimeout()
    {
        QueueFree();
    }
}
