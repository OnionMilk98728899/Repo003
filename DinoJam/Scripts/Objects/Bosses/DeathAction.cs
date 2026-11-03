using Godot;
using System;

public partial class DeathAction : BossAction
{

    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        isDying = true;
    }
}