using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class BossController : CharacterBody2D
{
    [Export] private BossPhase[] bossPhases;
    [Export] private HurtBox[] hurtBoxes;
    [Export] private CollisionShape2D[] colliders;
    [Export] private HealthComponent healthComp;
    [Export] private Label debugLabel;
    [Export] private Node2D bossSpriteRoot;
    private bool isFacingRight;
    private BossPhase currentPhase;
    private int phaseCounter = 0;
    public ulong enemyId { get; private set; }

    public override void _Ready()
    {
        // EventBus.Instance.HitBoss += HitBoss;
        // EventBus.Instance.StunBoss += StunBoss;
        currentPhase = bossPhases[phaseCounter];
        bossPhases[phaseCounter].StartPhase(this);
        GetHurtboxes();
        healthComp.InitializeBoss(this);
        //SetActionIds();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (currentPhase.currentAction != null)
        {
            Velocity = currentPhase.currentAction.bossVelocity;
        }
        //debugLabel.Text = $"{currentPhase.currentAction.actionTimer.TimeLeft}";
        debugLabel.Text = $"{healthComp.enemyHealth}--{phaseCounter}/{currentPhase.actionCounter}: {currentPhase.currentAction.Name}";

        MoveAndSlide();
    }

    private void GetHurtboxes()
    {
        foreach (HurtBox box in hurtBoxes)
        {
            box.BossInitialize(this);
        }
    }

    public BossPhase GetCurrentPhase()
    {
        return currentPhase;
    }

    public void StunBoss()
    {
        currentPhase.StunBoss(this);
    }

    public void HitBoss(int health)
    {
        if (currentPhase.isStunned)
        {
            healthComp.SubtractEnemyHealth(1);
            currentPhase.HitBoss(this, health);
            GD.Print("Hit boss and health = " +health);
            if(phaseCounter >= bossPhases.Count()-1)
            {
                return;
            }
            if(health <= 0)
            {
                GD.Print("Death Initiated");
                
                MoveToDeathPhase();
                currentPhase.SetDying();
            }
            else if(health <= currentPhase.healthTrigger)
            {
                QueueNextPhase();
            }
        }
    }

    public void KillBoss()
    {
        QueueFree();
    }

    // public void SetPhase(BossPhase thisPhase)
    // {
    //     currentPhase = thisPhase;
    // }

    public void MoveToNextPhase()
    {
        phaseCounter++;

        if(phaseCounter >= bossPhases.Count() - 1)
        {
            return;
        }
        bossPhases[phaseCounter].StartPhase(this);
        currentPhase = bossPhases[phaseCounter];
    }

    private void MoveToDeathPhase()
    {
        phaseCounter = bossPhases.Count()-1;
        bossPhases[phaseCounter].StartPhase(this);
        currentPhase = bossPhases[phaseCounter];
    }

    private void QueueNextPhase()
    {
        currentPhase.isNextPhaseQueued = true;
    }


    public BossPhase GetPhase()
    {
        return currentPhase;
    }

    public void FlipBossHorizontally(Vector2 velocity)
    {
        if (velocity.X < 0 && isFacingRight || velocity.X > 0 && !isFacingRight)
        {
            bossSpriteRoot.Scale = new Vector2(isFacingRight ? 1 : -1, 1);
            isFacingRight  = bossSpriteRoot.Scale.X < 0;
        }
    }
}
