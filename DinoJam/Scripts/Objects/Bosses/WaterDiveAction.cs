using Godot;
using System;

public partial class WaterDiveAction : BossAction
{
    [Export] HurtBox[] hurtBoxes;
    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        actionAnim.Play("dive");
    }

    private void OnDiveAnimationInitiated()
    {
        foreach(HurtBox box in hurtBoxes)
        {
            box.SetCollisionMaskValue(4, false);
        }
    }
}