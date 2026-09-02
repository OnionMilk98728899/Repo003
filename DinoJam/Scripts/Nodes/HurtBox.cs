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
    private string deathType;
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
            if (myHBoxType == hurtBoxType.player && body.IsInGroup("Enemy") && hurtTimer.IsStopped())
            {
                myEnemy = body.GetNode<Enemy>(".");
                //EventBus.Instance.EmitSignal(EventBus.SignalName.HurtPlayer, myEnemy.GlobalPosition);
                healthComp.HurtPlayer(myEnemy.GlobalPosition, "standard");
                hurtTimer.Start();
            }


            if (myHBoxType == hurtBoxType.head)
            {
                if (body.IsInGroup("Player") && hurtTimer.IsStopped())
                {
                    Player myPlayer = body.GetNode<Player>(".");

                    if (myPlayer.GetSpecialState() == Player.specialState.stomp && myPlayer.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        //EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemy, 2, enemyId, myPlayer.GetVelocity());
                        healthComp.OnEnemyHurt(2, enemyId, myPlayer.GetVelocity());
                        hurtTimer.Start();
                    }
                    if (myPlayer.GetMoveState() == Player.moveState.fall && myPlayer.GlobalPosition.Y < GlobalPosition.Y)
                    {
                        //EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemy, 1, enemyId, myPlayer.GetVelocity());
                        healthComp.OnEnemyHurt(1, enemyId, myPlayer.GetVelocity());
                        EventBus.Instance.EmitSignal(EventBus.SignalName.BouncePlayerUpwards, 180);
                        hurtTimer.Start();

                    }
                }
                if (body.IsInGroup("Edible") && hurtTimer.IsStopped())
                {
                    myEdible = body.GetNode<Edible>(".");
                    if (myEdible.isFlying)
                    {
                        healthComp.OnEnemyHurt(1, enemyId, Vector2.Zero);
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
                        healthComp.OnEnemyCharged(2, enemyId, myPlayer.GetVelocity());
                        hurtTimer.Start();
                    }
                }
                if (body.IsInGroup("Edible") && hurtTimer.IsStopped())
                {
                    myEdible = body.GetNode<Edible>(".");
                    if (myEdible.isFlying)
                    {
                        healthComp.OnEnemyHurt(1, enemyId, Vector2.Zero);
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
