using Godot;
using System;

public partial class Gate : StaticBody2D
{
    [Export] private Sprite2D gateSprite;
    [Export] private AnimationPlayer gateAnim;
    [Export] private int gateId;

    public override void _Ready()
    {
        EventBus.Instance.ActivateSwitchDoor += ActivateGate;
    }

    private void ActivateGate(int id, bool open)
    {
        if(id == gateId)
        {
            OpenGate(open);
        }
    }

    private void OpenGate(bool open)
    {
        if (open)
        {
            gateAnim.Play("gateopen");
        }
        else
        {
            gateAnim.Play("gateclose");
        }
    }
}
