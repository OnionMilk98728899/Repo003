using Godot;
using System;

public partial class WaterPlatform : AnimatableBody2D
{
        private enum motion{up,down, still}
        private motion myMotion;
       private Vector2 startPosition, peakPosition;
       private float upTime, downTime, moveSpeed;

       private bool movingUp;

    public override void _Ready()
    {
        startPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (myMotion == motion.up)
        {
            Move(peakPosition);
            
        }
        else if(myMotion == motion.down)
        {
            Move(startPosition);
        }
    }

    public void InitializeValues(float speed, Vector2 peak)
    {
        peakPosition = peak;
        moveSpeed = speed;
    }

    public void Move(Vector2 targetPosition)
    {
        GlobalPosition = GlobalPosition.MoveToward(targetPosition, moveSpeed);
        if(Mathf.Abs(GlobalPosition.Y - targetPosition.Y) <= .1)
        {
            myMotion = motion.still;
        }
    }

    public void OnJetOn()
    {
        myMotion = motion.up;
        SetCollisionLayerValue(1, true);
    }

    public void OnJetOff()
    {
        myMotion = motion.down;
        SetCollisionLayerValue(1, false);
    }



}
