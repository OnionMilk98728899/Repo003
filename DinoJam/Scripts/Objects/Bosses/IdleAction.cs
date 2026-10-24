using Godot;
using System;

public partial class IdleAction : BossAction
{
    private bool isFinished;

    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        actionAnim.Play("idle");
    }

    public override void _PhysicsProcess(double delta)
    {

    }

    // public override bool IsFinished()
    // {
    //     return isFinished;
    // }
}