using Godot;
using System;

public partial class EventBus : Node2D
{
    public static EventBus Instance {get; private set;}
    [Signal] public delegate void SetCameraAnchorsEventHandler(Node2D[] points);
    [Signal] public delegate void HurtPlayerEventHandler(int damage);
    [Signal] public delegate void KillPlayerEventHandler();
    [Signal] public delegate void HurtEnemyEventHandler(int damage, ulong enemyID);
    [Signal] public delegate void ChargeEnemyEventHandler(int damage, ulong enemyID);
    [Signal] public delegate void KillEnemyEventHandler(ulong enemyID);
    [Signal] public delegate void BouncePlayerEventHandler(float bouncePower);
    [Signal] public delegate void BreakableTileBrokenEventHandler(BreakableTile tile);
    [Signal] public delegate void ShakeTreeEventHandler(Tree tree);

    public override void _EnterTree()
    {
        Instance = this;
    }
}
