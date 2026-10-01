using Godot;
using System;

public partial class HurtBox : Area2D
{
    public enum hurtBoxType { player, side, head }
    [Export] private hurtBoxType myHBoxType;
    [Export] private Timer hurtTimer;
    [Export] private HealthComponent healthComp;
    private Enemy myEnemy;
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

    // public override void _PhysicsProcess(double delta)
    // {
    //     if(myHBoxType== hurtBoxType.side)
    //     {
    //         GD.Print(enemyId);
    //     }

    // }


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
                    // Vector2 smackPosition = GlobalPosition + ((myEnemy.GlobalPosition - GlobalPosition) / 2);
                    // EffectsManager.Instance.PlaySmackEffect(smackPosition);
                    GD.Print("Player damaged by enemy");
                }
                if (body.IsInGroup("EnemyProjectile") && body is EnemyProjectile proj)
                {
                    myProj = proj;
                    healthComp.HurtPlayer(myProj.GlobalPosition, "standard");
                    hurtTimer.Start();
                    // Vector2 smackPosition = GlobalPosition + ((myProj.GlobalPosition - GlobalPosition) / 2);
                    // EffectsManager.Instance.PlaySmackEffect(smackPosition);

                }

            }

            if (myHBoxType == hurtBoxType.head)
            {
                if (body.IsInGroup("Player") && hurtTimer.IsStopped())
                {
                    Player myPlayer = body.GetNode<Player>(".");

                    if (myPlayer.GetSpecialState() == Player.specialState.stomp && myPlayer.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        GD.Print("Player Stomped enemy");
                        healthComp.OnEnemyHurt(2, enemyId, myPlayer.GetVelocity(), myPlayer.GlobalPosition);
                        hurtTimer.Start();
                        // Vector2 smackPosition = GlobalPosition + ((myPlayer.GlobalPosition - GlobalPosition) / 2);
                        // EffectsManager.Instance.PlaySmackEffect(smackPosition);
                        EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayerUpwards, 220);
                        

                    }else if (myPlayer.GetMoveState() == Player.moveState.fall && myPlayer.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        GD.Print("Player Jumped on enemy");
                        healthComp.OnEnemyHurt(1, enemyId, myPlayer.GetVelocity(), myPlayer.GlobalPosition);

                        EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayerUpwards, 180);
                        hurtTimer.Start();
                        // Vector2 smackPosition = GlobalPosition + ((myPlayer.GlobalPosition - GlobalPosition) / 2);
                        // EffectsManager.Instance.PlaySmackEffect(smackPosition);

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
                    Player myPlayer = body.GetNode<Player>(".");

                    if (myPlayer.GetSpecialState() == Player.specialState.charge)
                    {
                        bool isLeft = myPlayer.GetIsPlayerLeft();
                        healthComp.OnEnemyCharged(2, enemyId, myPlayer.GetVelocity(), myPlayer.GlobalPosition);

                        hurtTimer.Start();
                        // Vector2 smackPosition = GlobalPosition + ((myPlayer.GlobalPosition - GlobalPosition) / 2);
                        // EffectsManager.Instance.PlaySmackEffect(smackPosition);
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
            if (myHBoxType == hurtBoxType.player && body.IsInGroup("LavaJet") && hurtTimer.IsStopped())
            {
                if (body is JetSegment jetSeg)
                {
                    if (jetSeg.myJetType == jetType.lava)
                    {
                        deathType = "lava";
                        healthComp.HurtPlayer(GlobalPosition, deathType);
                        hurtTimer.Start();
                    }
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
