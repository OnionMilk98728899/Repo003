using Godot;
using System;

public partial class EventBus : Node2D
{
    public static EventBus Instance {get; private set;}
    [Signal] public delegate void HurtPlayerEventHandler(int damage);
    [Signal] public delegate void KillPlayerEventHandler();
    [Signal] public delegate void HurtEnemyEventHandler(int damage);
    [Signal] public delegate void ChargeEnemyEventHandler(int damage);
    [Signal] public delegate void KillEnemyEventHandler();

    public override void _EnterTree()
    {
        Instance = this;
    }
}
