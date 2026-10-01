using Godot;
using System;
using System.Collections.Generic;

public enum effectType
{
    dust, rocks, eggshells, bombbits, yolk, goo, water, bones, blood
}
public partial class EffectsManager : Node2D
{
    public static EffectsManager Instance { get; private set; }
    [Export] private PackedScene dustScene, rockScene, eggShellScene, yolkScene, gooScene, waterScene, boneScene, bloodScene;
    [Export] private AnimationPlayer dustAnim, smackAnim;
    [Export]private Timer smackTimer;
    [Export] private Sprite2D dustSprite, smackSprite;
    [Export] private Vector2 dustOffset;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!smackTimer.IsStopped() && Engine.TimeScale != 1)
        {
            Engine.TimeScale +=.01;
            if(Engine.TimeScale > 1)
            {
                Engine.TimeScale = 1;
            }
        }
    }

    public PackedScene GetParticleScene(effectType type)
    {
        return type switch
        {
            effectType.dust => dustScene,
            effectType.rocks => rockScene,
            effectType.eggshells => eggShellScene,
            effectType.yolk => yolkScene,
            effectType.goo => gooScene,
            effectType.water => waterScene,
            effectType.bones => boneScene,
            effectType.blood => bloodScene,
            _ => null
        };
    }

    public void PlayDustEffect(Vector2 position, float yVelocity, string effect, bool flip)
    {
        if (!dustAnim.IsPlaying() || effect == "landdust" ||  dustAnim.CurrentAnimation == "walkdust" && effect != "walkdust" )
        {
            string num = "";
            if(effect == "landdust")
            {
               
                if(yVelocity >= 200)
                {
                    num = "3";
                }else if(yVelocity < 200  &&  yVelocity >= 100)
                {
                    num = "2";
                }else if(yVelocity < 100 && yVelocity >=0)
                {
                    num = "1";
                }else if(yVelocity < 0)
                {
                    return;
                }
                EventBus.Instance.EmitSignal(EventBus.SignalName.ScreenShake, 3, 3);
            }
            dustAnim.Stop();
            dustSprite.GlobalPosition = position + dustOffset;
            dustSprite.FlipH = flip;
            dustAnim.Play(effect + num);
            
        }
        
    }

    public void PlaySmackEffect(Vector2 position, int intensity)
    {
        smackSprite.GlobalPosition = position;
        switch (intensity)
        {
            case 1:
            smackAnim.Play("smack");
            break;
            case 2:
            smackAnim.Play("deathsmack");
            smackTimer.Start();
            Engine.TimeScale = .3f;
            break;
        }
             
    }
}
