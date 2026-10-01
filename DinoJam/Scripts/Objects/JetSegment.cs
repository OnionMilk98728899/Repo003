using Godot;
using System;

public partial class JetSegment : StaticBody2D
{    
    [Export] private CollisionShape2D collision;
    [Export] private AnimatedSprite2D jetSprite;
    public jetType myJetType;
    public void DisableEnableCollider(bool isEnabled)
    {
        collision.Disabled = isEnabled;
    }

    public void AnimateJet(string anim)
    {
        jetSprite.Play(anim);
    }

    public void ExtinguishJet()
    {
        collision.Disabled = true;
        jetSprite.Visible = false;
    }

}
