using Godot;
using System;
using System.Collections.Generic;

public enum enemyMoveState { move, prepare, attack, jump, hurt, dying }
public enum enemyDeathType{splat, spin}
public partial class Enemy : CharacterBody2D
{
    [Export] public EnemyAttacker attacker;
    [Export] public EnemyDeath death;
    [Export] public float attackDelay;
    [Export] private float bounceBack;
    [Export] private HealthComponent healthComp;
    [Export] private Timer hurtTimer, attackDelayTimer, deathTimer, prepareTimer;
    public ulong enemyId { get; private set; }
    public enemyMoveState currentMoveState;
    public enemyDeathType currentDeathType;
    public Player myPlayer;
    public Vector2 enemyVelocity, deathVelocity;
    public bool isBounced;
    private List<HurtBox> hurtBoxes = new();
    public override void _Ready()
    {
        EventBus.Instance.HurtEnemy += OnEnemyHurt;
        EventBus.Instance.ChargeEnemy += OnEnemyCharged;
        EventBus.Instance.KillEnemy += OnEnemyKilled;
        EventBus.Instance.HurtEnemyTimeout += OnHurtEnemyTimeout;

        enemyId = GetInstanceId();

        GetHurtboxes();
        GetHealthComponent();
        attackDelayTimer.WaitTime = attackDelay;
    }

    private void GetHurtboxes()
    {
        foreach (Node node in GetChildren())
        {
            if (node is HurtBox hurtBox)
            {
                hurtBoxes.Add(hurtBox);
                hurtBox.Initialize(this);
            }
        }
    }

    private void GetHealthComponent()
    {
        healthComp.Initialize(this);
    }

    private void OnPlayerDetectorEntered(Node2D body)
    {
        if (body.IsInGroup("Player") && currentMoveState != enemyMoveState.hurt && currentMoveState != enemyMoveState.dying
        && attackDelayTimer.IsStopped())
        {
            myPlayer = body.GetNode<Player>(".");
            PlayerDetected();
        }
    }

    public virtual void PlayerDetected()
    {
        currentMoveState = enemyMoveState.prepare;
        attackDelayTimer.Start();
        prepareTimer.Start();
    }


    private void OnEnemyHurt(ulong ID)
    {
        if (enemyId == ID)
        {
            currentMoveState = enemyMoveState.hurt;
        }
    }

    private void OnHurtEnemyTimeout()
    {
        currentMoveState = enemyMoveState.move;
    }

    private void OnEnemyKilled(ulong ID, Vector2 strikeVelocity)
    {
        if (enemyId == ID)
        {
            Vector2 newVel = Vector2.Zero;
            if(strikeVelocity.Y == 0)
            {
                
                newVel = new Vector2(strikeVelocity.X * 3, -300);
            }
            else
            {
                SetCollisionMaskValue(1, false);
                newVel = new Vector2(0,strikeVelocity.Y * 2);
            }
            
            deathVelocity = newVel;
            currentMoveState = enemyMoveState.dying;
            isBounced = true;
            enemyVelocity = deathVelocity;
            deathTimer.Start();
        }
    }

    private void OnEnemyCharged(ulong ID, Vector2 strikeVelocity)
    {
        if (enemyId == ID)
        {
            currentMoveState = enemyMoveState.hurt;
            if (IsOnFloor())
            {
                if (strikeVelocity.X < 0)
                {
                    enemyVelocity.X = -bounceBack;
                }
                else
                {
                    enemyVelocity.X = bounceBack;
                }
                //enemyVelocity.X
            }
        }
    }
    private void OnPrepareTimerTimeout()
    {
        if(currentMoveState != enemyMoveState.hurt && currentMoveState != enemyMoveState.dying)
        {
            currentMoveState = enemyMoveState.attack;
        }
    }
    private void OnDeathTimerTimeout()
    {
        QueueFree();
    }

}
