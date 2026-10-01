using Godot;
using System;

public partial class AutoSpikes : StaticBody2D
{
    [Export] private Timer onTimer, offTimer;
    [Export] private AnimationPlayer spikeAnim;
    [Export] private float onTime, offTime;
    private bool isOn;

    public override void _Ready()
    {
        onTimer.WaitTime = onTime;
        offTimer.WaitTime = offTime;
        offTimer.Start();
    }

    private void OnOnTimerTimeout()
    {
        offTimer.Start();
        spikeAnim.Play("spikesoff");
        //isOn = false;
    }

    private void OnOffTimerTimeout()
    {
        onTimer.Start();
        spikeAnim.Play("spikeson");
        //isOn = true;
    }

}
