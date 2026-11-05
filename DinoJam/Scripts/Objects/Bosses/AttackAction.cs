using Godot;
using System;

public partial class AttackAction : BossAction
{
    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        actionAnim.Play("attack");
    }
}