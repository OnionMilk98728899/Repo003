using Godot;
using System;

public partial class HurtBox : Area2D
{
    public enum hurtBoxType { player, bosshead, bossside, side, head, rock, fire, water, air }
    [Export] private hurtBoxType myHBoxType;
    [Export] private Timer hurtTimer;
    [Export] private HealthComponent healthComp;
    private Enemy myEnemy;
    private BossController myBoss;
    private Edible myEdible;
    private TileMap myTilemap;
    private EnemyProjectile myProj;
    private string deathType;
    private ulong enemyId;
    //[Export] private Enemy myEnemy;

    public void Initialize(Enemy enemy)
    {
        enemyId = enemy.enemyId;
    }

    public void BossInitialize(BossController boss)
    {
        enemyId = boss.enemyId;
    }
    private void OnHurtBoxEntered(Node2D body)
    {
        if (body is CharacterBody2D)
        {
            if (myHBoxType == hurtBoxType.player && hurtTimer.IsStopped())
            {
                if (body.IsInGroup("Enemy") && body is Enemy en)
                {
                    myEnemy = en;
                    healthComp.HurtPlayer(myEnemy.GlobalPosition, "standard");
                    hurtTimer.Start();
                }
                if (body.IsInGroup("EnemyProjectile") && body is EnemyProjectile proj)
                {
                    myProj = proj;
                    healthComp.HurtPlayer(myProj.GlobalPosition, "standard");
                    hurtTimer.Start();

                }
                if (body.IsInGroup("Boss") && body is BossController boss)
                {
                    myBoss = boss;
                    healthComp.HurtPlayer(myBoss.GlobalPosition, "standard");
                    hurtTimer.Start();
                }
                if (body.IsInGroup("Edible") && body is Edible edib)
                {
                    if (edib.isPlayerDamaging)
                    {
                        healthComp.HurtPlayer(edib.GlobalPosition, "standard");
                        hurtTimer.Start();
                    }

                }

            }

            if (myHBoxType == hurtBoxType.head)
            {
                if (body.IsInGroup("Player"))
                {
                    Player myPlayer = body.GetNode<Player>(".");

                    if (myPlayer.GetSpecialState() == Player.specialState.stomp && myPlayer.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        if (hurtTimer.IsStopped())
                        {
                            healthComp.OnEnemyHurt(2, enemyId, myPlayer.GetVelocity(), myPlayer.GlobalPosition);
                            hurtTimer.Start();
                        }

                        EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayerUpwards, 175);
                    }
                    else if (myPlayer.GetMoveState() == Player.moveState.fall && myPlayer.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        if (hurtTimer.IsStopped())
                        {
                            healthComp.OnEnemyHurt(1, enemyId, myPlayer.GetVelocity(), myPlayer.GlobalPosition);
                            hurtTimer.Start();

                        }

                        EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayerUpwards, 150);
                    }
                }
                if (body.IsInGroup("Edible") && hurtTimer.IsStopped())
                {
                    myEdible = body.GetNode<Edible>(".");
                    if (myEdible.isFlying)
                    {
                        healthComp.OnEnemyHurt(1, enemyId, Vector2.Zero, myEdible.GlobalPosition);
                        hurtTimer.Start();
                    }
                }
            }

            if (myHBoxType == hurtBoxType.side)
            {
                if (body.IsInGroup("Player") && hurtTimer.IsStopped())
                {
                    if (body is Player myPlayer)
                    {
                        if (myPlayer.GetSpecialState() == Player.specialState.charge)
                        {
                            healthComp.OnEnemyCharged(2, enemyId, myPlayer.GetVelocity(), myPlayer.GlobalPosition);
                            hurtTimer.Start();
                        }
                    }
                }
                if (body.IsInGroup("Edible") && hurtTimer.IsStopped())
                {
                    myEdible = body.GetNode<Edible>(".");
                    if (myEdible.isFlying)
                    {
                        healthComp.OnEnemyHurt(1, enemyId, Vector2.Zero, myEdible.GlobalPosition);
                        hurtTimer.Start();
                    }
                }

            }

            if (myHBoxType == hurtBoxType.bosshead)
            {

                if (body.IsInGroup("Player") && body is Player myPlayer)
                {
                    if (myPlayer.GetSpecialState() == Player.specialState.stomp || myPlayer.GetSpecialState() == Player.specialState.stompland)
                    {
                        EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayerUpwards, 200);
                        healthComp.OnBossHit();
                    }
                    // else if (myPlayer.GetMoveState() == Player.moveState.fall)
                    // {
                    //     EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayerUpwards, 150);
                    // }
                }
            }
            if (myHBoxType == hurtBoxType.bossside)
            {
                if (body.IsInGroup("Player") && body is Player myPlayer)
                {
                    if (myPlayer.GetSpecialState() == Player.specialState.charge)
                    {
                        EventBus.Instance.EmitSignal(EventBus.SignalName.ChargeEnemy, enemyId, myPlayer.GetVelocity(), GlobalPosition);
                        healthComp.OnBossHit();
                    }
                }
            }
            if (myHBoxType == hurtBoxType.rock && body is Edible edible)
            {
                if (edible.myEdibleType == Edible.edibleType.rock && edible.isFlying)
                {
                    GD.Print("STUNNED BOSS");
                    edible.ConsumeEdible();
                    healthComp.OnBossStunned();
                    hurtTimer.Start();
                }
            }
            
        }
        else if (body is TileMap)
        {

            if (body.IsInGroup("Hazard") && hurtTimer.IsStopped())
            {
                myTilemap = body.GetNode<TileMap>(".");
                deathType = myTilemap.GetCellTileData(0, (Vector2I)GlobalPosition / 16).GetCustomData("hazardType").ToString();
                if (deathType != "none")
                {
                    Vector2 reflectPos = new Vector2(GlobalPosition.X, GlobalPosition.Y + 16);

                    healthComp.HurtPlayer(reflectPos, deathType);
                    hurtTimer.Start();
                }

            }

        }
        else if (body is StaticBody2D)
        {
            if (myHBoxType == hurtBoxType.player && hurtTimer.IsStopped())
            {
                if (body.IsInGroup("LavaJet") && body is JetSegment jetSeg)
                {
                    if (jetSeg.myJetType == jetType.lava)
                    {
                        deathType = "standard";
                        healthComp.HurtPlayer(jetSeg.GlobalPosition, deathType);
                        hurtTimer.Start();
                    }
                }
                if (body.IsInGroup("ElementalTile") && body is ElementalTile tile)
                {
                    if (tile.myTileType == ElementalTile.tileType.lavaPool || tile.myTileType == ElementalTile.tileType.lavaRiverLeft ||
                    tile.myTileType == ElementalTile.tileType.lavaRiverRight)
                    {
                        deathType = "lava";

                    }
                    else
                    {
                        deathType = "standard";
                    }
                    healthComp.HurtPlayer(GlobalPosition, deathType);
                    hurtTimer.Start();
                }

            }
            if (myHBoxType == hurtBoxType.player && body is AutoSpikes && hurtTimer.IsStopped())
            {
                healthComp.HurtPlayer(GlobalPosition, "standard");
                hurtTimer.Start();
            }
        }

    }

    private void OnHurtTimerTimeout()
    {
        if (myHBoxType != hurtBoxType.player)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemyTimeout);
        }
        else
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.HurtPlayerTimeout);
        }

    }


}
