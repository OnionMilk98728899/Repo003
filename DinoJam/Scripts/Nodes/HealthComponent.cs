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

    public void OnEnemyHurt(int damage, ulong ID, Vector2 strikeVelocity, Vector2 position)
    {
        enemyHealth -= damage;
        GD.Print("Enemy hurt with strikeVelocity of " +strikeVelocity);
        if(enemyHealth <= 0)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.KillEnemy, ID, strikeVelocity, position);
        }
        else
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.HurtEnemy, ID, position);
        }
    }

    public void OnEnemyCharged(int damage, ulong ID, Vector2 strikeVelocity, Vector2 position)
    {   
        enemyHealth-= damage;
        
        if(enemyHealth <= 0)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.KillEnemy, enemyId, strikeVelocity, position);
        }
        else
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.ChargeEnemy, ID, strikeVelocity, position);
           
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
