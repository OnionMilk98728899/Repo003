using Godot;
using System;

public partial class SplatDeath : EnemyDeath
{
    
    private bool hasSplattered;
    public override Vector2 Death(Vector2 velocity)
    {
        if (!hasSplattered)
        {
            GpuParticles2D myParticles1 = EffectsManager.Instance.GetParticleScene(effectType.blood).Instantiate<GpuParticles2D>();
            AddChild(myParticles1);
            myParticles1.Restart();
            enemySprite.Visible = false;
            hasSplattered = true;
        }

        return Vector2.Zero;
    }
}