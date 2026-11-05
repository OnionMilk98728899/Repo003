using Godot;
using System;

public partial class DeathAction : BossAction
{

    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        actionAnim.Play("death");
        isDying = true;
    }

    public override void OnActionTimerTimeout()
    {
        base.OnActionTimerTimeout();
        myBoss.KillBoss();
    }
}