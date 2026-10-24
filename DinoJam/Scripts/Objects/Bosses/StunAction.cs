using Godot;
using System;

public partial class StunAction : BossAction
{
    [Signal] public delegate void StunExpiredEventHandler();
    [Export] private string stunAnim, preStunAnim;
    [Export] private bool hasPreStunPhase;
    //[Export] private Sprite2D
    public override void StartAction(BossController boss)
    {
        base.StartAction(boss);
        if (hasPreStunPhase)
        {
            actionAnim.Play(preStunAnim);
        }
        else
        {
            actionAnim.Play(stunAnim);
        }
       
    }

    private void OnPreStunAnimationFinished()
    {
        actionAnim.Play(stunAnim);
    }

    public override void OnActionTimerTimeout()
    {
        base.OnActionTimerTimeout();
        EmitSignal(SignalName.StunExpired);

    }
}