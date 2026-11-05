using Godot;
using System;
using System.Linq;

public partial class BossPhase : Node2D
{
    [Export] BossAction[] phaseActions;
    [Export] BossAction stunAction;
    [Export] private Sprite2D bossSprite;
    [Export] private Timer hitTimer;
    [Export] private bool isLooping;
    [Export] public int healthTrigger;
    public BossAction currentAction;
    private BossController myBoss;
    private ulong enemyId;
    public int actionCounter = 0;
    public bool isStunned, isNextPhaseQueued;
    private bool isDying;

    public override void _Ready()
    {
        bossSprite.UseParentMaterial = true;
    }

    public void StartPhase(BossController boss)
    {
        phaseActions[actionCounter].StartAction(boss);
        currentAction = phaseActions[actionCounter];
        myBoss = boss;
    }

    private void StartAction(BossController boss, BossAction action)
    {
        action.StartAction(boss);
        currentAction = action;
    }

    public void OnActionFinished()
    {
        if (!isStunned && !isDying)
        {
            if (actionCounter <= phaseActions.Count() - 2)
            {
                actionCounter++;
                //GD.Print("Action counter increased");
                StartAction(myBoss, phaseActions[actionCounter]);
            }
            else if (actionCounter == phaseActions.Count() - 1 && isLooping)
            {
                actionCounter = 0;
                StartAction(myBoss, phaseActions[actionCounter]);
            }
            else if (actionCounter == phaseActions.Count() - 1 && !isLooping)
            {
                MoveToNextPhase();
            }
        }
    }

    // public void SelectRandomActionFromPhase()
    // {
    //     if (!isStunned)
    //     {
    //         int rand = GD.RandRange(0, phaseActions.Count() - 1);
    //         phaseActions[rand].StartAction(myBoss);
    //         currentAction = phaseActions[rand];
    //     }
    // }

    public void StunBoss(BossController boss)
    {
        isStunned = true;
        stunAction.StartAction(boss);
        currentAction = stunAction;
        myBoss = boss;
    }

    public void HitBoss(BossController boss, int health)
    {
        if (isStunned && hitTimer.IsStopped())
        {
            bossSprite.UseParentMaterial = false;
            hitTimer.Start();
        }
    }

    public void SetDying()
    {
        // boss.KillBoss();
        isDying = true;
    }

    private void MoveToNextPhase()
    {
        myBoss.MoveToNextPhase();
    }

    public void MoveToTargetAction(BossAction action)
    {

    }

    public void OnStunActionExpired()
    {
        isStunned = false;
        //OnActionFinished();
        if (isNextPhaseQueued)
        {
            MoveToNextPhase();
        }
        //SelectRandomActionFromPhase();
    }
    private void OnHitTimerTimeout()
    {
        bossSprite.UseParentMaterial = true;
    }
}
