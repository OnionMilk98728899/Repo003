using Godot;
using System;

public partial class HealthComponent : Node2D
{
    public enum unitType{player, enemy}
    [Export] public unitType myUnitType;
    [Export] private int enemyHealth;
    private ulong enemyId;
    public override void _Ready()
    {
        //EventBus.Instance.HurtEnemy += OnEnemyHurt;
        //EventBus.Instance.HurtPlayer += OnHurtPlayer;
        //EventBus.Instance.ChargeEnemy += OnEnemyCharged;
        // if(myUnitType == unitType.player)
        // {
        //     health = GlobalStats.Instance.playerHealth;
        // }
    }

    public void Initialize(Enemy enemy)
    {
        enemyId = enemy.enemyId;
    }

    public void OnEnemyHurt(int damage, ulong ID, Vector2 strikeVelocity)
    {
        enemyHealth -= damage;
        if(enemyHealth <= 0)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.KillEnemy, ID, strikeVelocity);
        }
        else
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemy, ID);
        }
    }

    public void OnEnemyCharged(int damage, ulong ID, Vector2 strikeVelocity)
    {   
        enemyHealth-= damage;

        if(enemyHealth <= 0)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.KillEnemy, enemyId, strikeVelocity);
        }
        else
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.ChargeEnemy, ID, strikeVelocity);
        }
    }

    public void HurtPlayer(Vector2 hurterPosition, string deathType)
    {
       if(GlobalStats.Instance.playerHealth > 0)
        {
            
            EventBus.Instance.EmitSignal(EventBus.SignalName.HurtPlayer, hurterPosition);
            GlobalStats.Instance.ZeroPlayerHealth();
        }
        else
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.KillPlayer, deathType);
        }

    }

}
