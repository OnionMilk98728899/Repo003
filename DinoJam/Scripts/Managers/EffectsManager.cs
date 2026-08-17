using Godot;
using System;
using System.Collections.Generic;

public enum effectType
{
    dust, rocks, eggshells, bombbits, yolk, goo, water, bones
}
public partial class EffectsManager : Node2D
{
    public static EffectsManager Instance { get; private set; }
    [Export] private PackedScene dustScene, rockScene, eggShellScene, yolkScene, gooScene, waterScene, boneScene;

    public override void _EnterTree()
    {
        Instance = this;
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
            _ => null
        };
    }
}
