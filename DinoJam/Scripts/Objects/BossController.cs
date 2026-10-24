using Godot;
using System;
using System.Diagnostics;

public partial class BossController : CharacterBody2D
{
    [Export] private BossPhase[] bossPhases;
    [Export] private HurtBox[] hurtBoxes;
    [Export] private CollisionShape2D[] colliders;
    [Export] private Label debugLabel;
    [Export] private Node2D bossSpriteRoot;
  
    private bool isFacingRight;

    private BossPhase currentPhase;
    private int phaseCounter;
    public ulong enemyId { get; private set; }

    public override void _Ready()
    {
        EventBus.Instance.HitBoss += HitBoss;
        EventBus.Instance.StunBoss += StunBoss;
        currentPhase = bossPhases[0];
        bossPhases[0].StartPhase(this);
        GetHurtboxes();
        //SetActionIds();

    }

    public override void _PhysicsProcess(double delta)
    {
        if (currentPhase.currentAction != null)
        {
            Velocity = currentPhase.currentAction.bossVelocity;
        }
        debugLabel.Text = $"{currentPhase.currentAction.actionTimer.TimeLeft}";
        //FlipBossHorizontally(Velocity);
        MoveAndSlide();
    }

    private void GetHurtboxes()
    {
        foreach (HurtBox box in hurtBoxes)
        {
            box.BossInitialize(this);
        }
    }

    private void StunBoss()
    {
        currentPhase.StunBoss(this);
    }

    private void HitBoss()
    {
        currentPhase.HitBoss(this);
    }

    public void SetPhase(BossPhase thisPhase)
    {
        currentPhase = thisPhase;
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
