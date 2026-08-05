using Godot;
using System;

public partial class Edible : CharacterBody2D
{
    [Export] private Sprite2D edibleSprite;
    [Export] private CollisionShape2D edibleColl;
    [Export] private float gravity, speed, arcHeight, maxGravity, bounceVelocity;
    //[Export] private Label debugLabel;
    [Export] private GpuParticles2D dustParticles;
    [Export] private Timer destroyTimer;
    private float elapsedTime;
    public enum edibleType { rock, egg, bomb, bone }
    public enum flightDirection { left, right }
    public enum flightPath { straight, arc, lob, down }
    public flightDirection myDirection;
    public flightPath myPath;
    public edibleType myEdibleType;
    private Vector2 edibleVelocity, initialVelocity;
    private int bounceCounter = 0;
    private bool isFlying, hasBounced, isDestroyed;
    public override void _PhysicsProcess(double delta)
    {
        if (isFlying)
        {

            Velocity = edibleVelocity;
            ApplyGravity();
            MoveAndSlide();


            if (!isDestroyed && IsOnFloor() || !isDestroyed && IsOnWall())
            {
                Destroy();
                isDestroyed = true;
            }

                
            
        }

        //debugLabel.Text = Mathf.Round(edibleVelocity.X).ToString();

    }

    public void SetFlight()
    {
        switch (myPath)
        {
            case flightPath.straight:
                initialVelocity = new Vector2(200, 0);
                break;
            case flightPath.arc:
                initialVelocity = new Vector2(150, -200);

                break;
            case flightPath.lob:
                initialVelocity = new Vector2(0, -200);
                GD.Print("Lobbed one");
                break;
            case flightPath.down:
                initialVelocity = new Vector2(0, 200);
                break;
        }

        if (myDirection == flightDirection.left)
        {
            initialVelocity.X = -initialVelocity.X;
        }

        edibleVelocity = initialVelocity;
        isFlying = true;
    }

    private void ApplyGravity()
    {
        if (!IsOnFloor() && myPath == flightPath.arc || myPath == flightPath.lob)
        {
            edibleVelocity.Y += gravity;
        }
        if (IsOnFloor() || isDestroyed)
        {
            edibleVelocity.Y = 0;
            edibleVelocity.X = 0;
        }
    }

    private void Bounce()
    {
        if (bounceCounter < 3)
        {
            if (IsOnFloor())
            {
                GD.Print("BOunced");
                edibleVelocity.Y = -bounceVelocity;
                bounceCounter++;
            }
            if (IsOnWall())
            {
                if (edibleVelocity.X > 0) { edibleVelocity.X = -bounceVelocity; }
                else if (edibleVelocity.X < 0) { edibleVelocity.X = bounceVelocity; }
                bounceCounter++;
                GD.Print("Bouncing with the power of = " + edibleVelocity.X);
            }
            if (IsOnCeiling())
            {
                edibleVelocity.Y = bounceVelocity;
                bounceCounter++;
            }
        }

        if (bounceCounter >= 3)
        {
            //QueueFree();
        }
    }

    private void Destroy()
    {

        edibleSprite.Visible = false;
        dustParticles.Restart();
        destroyTimer.Start();
    }

    public void ConsumeEdible()
    {
        QueueFree();
    }

    private void OnDestroyTimerTimeout()
    {
        QueueFree();
    }
}
