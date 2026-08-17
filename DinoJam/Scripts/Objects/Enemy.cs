using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
    public ulong enemyId { get; private set; }
    private List<HurtBox> hurtBoxes = new();
    public override void _Ready()
    {
        EventBus.Instance.HurtEnemy += OnEnemyHurt;
        EventBus.Instance.ChargeEnemy += OnEnemyHurt;
        EventBus.Instance.KillEnemy += OnEnemyKilled;
        enemyId = GetInstanceId();

        GetHurtboxes();
    }

    private void GetHurtboxes()
    {
        foreach(Node node in GetChildren())
        {
            if(node is HurtBox hurtBox)
            {
                hurtBoxes.Add(hurtBox);
                hurtBox.Initialize(this);
            }
        }
    }


    private void OnEnemyHurt(int damage, ulong ID)
    {
        if(enemyId == ID)
        {
            QueueFree();
            
        }

        
    }


    private void OnEnemyKilled(ulong ID)
    {
         if(enemyId == ID)
        {
            QueueFree();
        }
        
    }

}
