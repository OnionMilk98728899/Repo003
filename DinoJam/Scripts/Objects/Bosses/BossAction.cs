using Godot;
using System;

public partial class BossAction : Node2D
{
    //public virtual bool IsFinished();
    [Export] public AnimationPlayer actionAnim;
    [Export] public Timer actionTimer;
    [Export] private float gravity;
    public BossController myBoss;
    public Vector2 bossVelocity;
    public bool isActive;
    // private ulong enemyId;
    public virtual void StartAction(BossController boss)
    {
        myBoss = boss;
        isActive = true;
        actionTimer.Start();
    }
    public virtual void ApplyGravity(BossController boss)
    {
        if (!boss.IsOnFloor())
        {
            bossVelocity.Y += gravity;
        }
        else
        {
            bossVelocity.Y = 0;
        }
    }

    public virtual void OnActionTimerTimeout()
    {
        isActive = false;
        myBoss.GetPhase().SelectRandomActionFromPhase();
    }
}