using Godot;
using System;

public partial class Bounceable : StaticBody2D
{
    [Export] private float bouncePower;
    [Export] private AnimationPlayer bounceAnim;
    private void OnPlayerDetectorBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            if(body.GetNode<Player>(".").GetMoveState() == Player.moveState.fall || 
            body.GetNode<Player>(".").GetSpecialState() == Player.specialState.stomp)
            {
                EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayer, bouncePower);
                bounceAnim.Play("bounce");
            }
        }
    }
}
