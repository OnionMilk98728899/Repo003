using Godot;
using System;

public partial class HealthComponent : Node2D
{
    public enum unitType{player, enemy}
    [Export] public unitType myUnitType;
    [Export] public int enemyHealth;
    private BossController myBoss;
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

    public void InitializeBoss(BossController boss)
    {
        myBoss = boss;
    }

    public void OnEnemyHurt(int damage, ulong ID, Vector2 strikeVelocity, Vector2 position)
    {
        enemyHealth -= damage;
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
            EventBus.Instance.EmitSignal(EventBus.SignalName.KillPlayer, deathType, hurterPosition);
        }

    }

    public int GetEnemyHealth()
    {
        return enemyHealth;
    }

    public void SubtractEnemyHealth(int dmg)
    {
        enemyHealth -= dmg;
    }

    public void OnBossHit()
    {
        //EventBus.Instance.EmitSignal(EventBus.SignalName.HitBoss);
        myBoss.HitBoss(enemyHealth);
    }

    public void OnBossStunned()
    {
        //EventBus.Instance.EmitSignal(EventBus.SignalName.StunBoss);
        myBoss.StunBoss();
    }

}
