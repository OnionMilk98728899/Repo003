using Godot;
using System;
using System.Linq;

public partial class BossPhase : Node2D
{
    [Export] BossAction[] phaseActions;
    [Export] BossAction stunAction;
    [Export] private Sprite2D bossSprite;
    [Export] private Timer hitTimer;
    public BossAction currentAction;
    private BossController myBoss;
    private ulong enemyId;
    private int actionCounter;
    private bool isStunned;

    public override void _Ready()
    {
        bossSprite.UseParentMaterial = true;
    }
    // public void InitializeID(BossController boss)
    // {
    //     enemyId = boss.enemyId;
    //     foreach(BossAction action in phaseActions)
    //     {
    //         action.InitializeID(enemyId);
    //     }
    // }
    public void StartPhase(BossController boss)
    {
        phaseActions[actionCounter].StartAction(boss);
        currentAction = phaseActions[actionCounter];
        myBoss = boss;
    }

    public void SelectRandomActionFromPhase()
    {
        if (!isStunned)
        {
            int rand = GD.RandRange(0, phaseActions.Count() - 1);
            phaseActions[rand].StartAction(myBoss);
            currentAction = phaseActions[rand];
        }
    }

    public void StunBoss(BossController boss)
    {
        isStunned = true;
        stunAction.StartAction(boss);
        currentAction = stunAction;
        myBoss = boss;
    }

    public void HitBoss(BossController boss)
    {
        if (isStunned && hitTimer.IsStopped())
        {
            
            bossSprite.UseParentMaterial = false;
            hitTimer.Start();
        }
    }

    public void KillBoss(BossController boss)
    {
        boss.KillBoss();
    }

    private void MoveToNextPhase()
    {
        
    }

    private void OnStunActionExpired()
    {
        isStunned = false;
        SelectRandomActionFromPhase();
    }
    private void OnHitTimerTimeout()
    {
        GD.Print("Hit timer timed out!");
        bossSprite.UseParentMaterial = true;
    }
}
