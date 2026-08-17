using Godot;
using System;

public partial class HurtBox : Area2D
{
    public enum hurtBoxType { player, side, head }
    [Export] private hurtBoxType myHBoxType;

    private ulong enemyId;
    //[Export] private Enemy myEnemy;

    public void Initialize(Enemy enemy)
    {
        enemyId = enemy.enemyId;
    }

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

                if (myPlayer.GetSpecialState() == Player.specialState.stomp && myPlayer.GlobalPosition.Y < GlobalPosition.Y)
                {
                    EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemy, 50, enemyId);
                }
                if (myPlayer.GetMoveState() == Player.moveState.fall && myPlayer.GlobalPosition.Y < GlobalPosition.Y)
                {
                    EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemy, 25, enemyId);
                    EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayer, 180);
                   
                }
            }

            if (myHBoxType == hurtBoxType.side && body.IsInGroup("Player"))
            {
                GD.Print("HBox entered");
                Player myPlayer = body.GetNode<Player>(".");

                if (myPlayer.GetSpecialState() == Player.specialState.charge)
                {
                    GD.Print("Charged Enemy!");
                    EventBus.Instance.EmitSignal(EventBus.SignalName.ChargeEnemy, 50, enemyId);
                }
            }
        }

    }


}
