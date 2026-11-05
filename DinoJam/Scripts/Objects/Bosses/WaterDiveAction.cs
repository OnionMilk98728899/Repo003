using Godot;
using System;

public partial class WaterDiveAction : BossAction
{
    [Export] HurtBox[] hurtBoxes;
    [Export] CollisionShape2D[] colliders;
    [Export] private bool isEntering;
    public override void StartAction(BossController boss)
    {
        if (isEntering)
        {
            actionAnim.Play("dive");
        }
        else
        {
            actionAnim.Play("emerge");
        }
        base.StartAction(boss);
        DisableEnableColliders();
    }

    // public void SetEnteringStatus(bool entering)
    // {
    //     isEntering = entering;
    // }

    private void DisableEnableColliders()
    {
        foreach (HurtBox box in hurtBoxes)
        {
            box.SetCollisionMaskValue(4, !isEntering);
        }

        foreach (CollisionShape2D coll in colliders)
        {
            coll.Disabled = isEntering;
        }
    }
}