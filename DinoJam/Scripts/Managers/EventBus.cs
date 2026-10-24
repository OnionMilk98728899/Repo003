using Godot;
using System;

public partial class EventBus : Node2D
{
    public static EventBus Instance {get; private set;}
    [Signal] public delegate void SetCameraAnchorsEventHandler(Node2D[] points);
    [Signal] public delegate void RepositionPlayerOriginEventHandler();
    [Signal] public delegate void HurtPlayerEventHandler(Vector2 position);
    [Signal] public delegate void HurtPlayerTimeoutEventHandler();
    [Signal] public delegate void KillPlayerEventHandler(string deathType, Vector2 position);
    [Signal] public delegate void HurtEnemyEventHandler(ulong enemyID, Vector2 position);
    [Signal] public delegate void HurtEnemyTimeoutEventHandler();
    [Signal] public delegate void ChargeEnemyEventHandler( ulong enemyID, Vector2 strikeVelocity, Vector2 position);
    [Signal] public delegate void KillEnemyEventHandler(ulong enemyID, Vector2 strikeVelocity, Vector2 position);
    [Signal] public delegate void BouncePlayerUpwardsEventHandler(float bouncePower);
    [Signal] public delegate void BreakableTileBrokenEventHandler(BreakableTile tile);
    [Signal] public delegate void ShakeTreeEventHandler(Tree tree);
    [Signal] public delegate void ActivateSwitchDoorEventHandler(int gateid, bool open);
    [Signal] public delegate void ScreenShakeEventHandler(int intensity, float duration);
    [Signal] public delegate void StunBossEventHandler();
    [Signal] public delegate void HitBossEventHandler();

    //[Signal] public delegate void 

    public override void _EnterTree()
    {
        Instance = this;
    }
}
