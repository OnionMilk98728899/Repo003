using Godot;
using System;

public enum timedParticleType{dust}
public partial class TimedParticles : GpuParticles2D
{
    public timedParticleType myParticleType;
    [Export] private Timer lifeTimer;
    [Export] private Texture2D dustTexture, waterTexture, fireTexture; 
    public void SetLifetime(float time)
    {
        myParticleType = timedParticleType.dust;
        lifeTimer.WaitTime = time;
        Restart();
        lifeTimer.Start();
    }

    

    private void OnLifeTimerTimeout()
    {
        if(myParticleType == timedParticleType.dust)
        {
            ProjectileManager.Instance.GenerateFallingRock(GlobalPosition);
        }
        QueueFree();
    }


}
