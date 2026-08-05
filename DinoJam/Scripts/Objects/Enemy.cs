using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    public override void _Ready()
    {
        EventBus.Instance.HurtEnemy += OnEnemyHurt;
        EventBus.Instance.ChargeEnemy += OnEnemyHurt;
        EventBus.Instance.KillEnemy += OnEnemyKilled;
    }


    private void OnEnemyHurt(int damage)
    {
        QueueFree();
    }


    private void OnEnemyKilled()
    {
        QueueFree();
    }

}
