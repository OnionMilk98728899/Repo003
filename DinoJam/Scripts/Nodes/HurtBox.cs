using Godot;
using System;

public partial class HurtBox : Area2D
{
    public enum hurtBoxType { player, side, head }
    [Export] private hurtBoxType myHBoxType;
    private void OnHurtBoxEntered(Node2D body)
    {
        if (body is CharacterBody2D)
        {
            if (myHBoxType == hurtBoxType.player && body.IsInGroup("Enemy"))
            {
                EventBus.Instance.EmitSignal(EventBus.SignalName.HurtPlayer, 50);
            }

            if (myHBoxType == hurtBoxType.head && body.IsInGroup("Player"))
            {
                Player myPlayer = body.GetNode<Player>(".");

                if (myPlayer.GetSpecialState() == Player.specialState.stomp)
                {
                    EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemy, 50);
                }
                if (myPlayer.GetMoveState() == Player.moveState.fall)
                {
                    EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemy, 25);
                }
            }

            if (myHBoxType == hurtBoxType.side)
            {
                Player myPlayer = body.GetNode<Player>(".");

                if (myPlayer.GetSpecialState() == Player.specialState.charge)
                {
                    EventBus.Instance.EmitSignal(EventBus.SignalName.ChargeEnemy, 50);
                }
            }
        }

    }


}
