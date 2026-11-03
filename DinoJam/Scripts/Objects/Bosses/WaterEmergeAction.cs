using Godot;
using System;

public partial class WaterEmergeAction : BossAction
{
    [Export] HurtBox[] hurtBoxes;
    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        actionAnim.Play("emerge");
    }

    private void OnDiveAnimationInitiated()
    {
        foreach(HurtBox box in hurtBoxes)
        {
            box.SetCollisionMaskValue(4, true);
        }
    }
}