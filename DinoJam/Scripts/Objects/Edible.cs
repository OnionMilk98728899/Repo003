using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Edible : CharacterBody2D
{
    [Signal] public delegate void EdibleEnemyConsumedEventHandler();
    [Export] private Sprite2D edibleSprite;
    [Export] private AnimationPlayer edibleAnim;
    [Export] private float gravity, speed, arcHeight, maxGravity, bounceVelocity;
    private GpuParticles2D myParticles1, myParticles2;
    private effectType fXType1, fXType2;
    [Export] private Timer destroyTimer;
    [Export] private Texture2D rockTexture, eggTexture, bombTexture, boneTexture, waterTexture, bugTexture, keyTexture;
    private float elapsedTime;
    public enum edibleType { rock, egg, bomb, bone, water, bug, key }
    public enum flightDirection { left, right }
    public enum flightPath { straight, arc, lob, down }
    public flightDirection myDirection;
    public flightPath myPath;
    [Export] public edibleType myEdibleType;
    private Vector2 edibleVelocity, initialVelocity, initialPosition;
    private int fXCounter, bounceCounter = 0;
    private bool hasBounced, isDestroyed, hasBeenEaten, isEnemyMaskSet;
    public bool isFlying, isPoppingOut;
    private string flightAnimation, staticAnimation;

    public override void _Ready()
    {
        initialPosition = GlobalPosition;
        
        if (myEdibleType == edibleType.bug && !hasBeenEaten)
        {
            return;
        }
        else
        {
            SetParticlesAndSprites();
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        if (myEdibleType == edibleType.bug && !hasBeenEaten)
        {
            return;
        }
        else
        {
            if (isFlying)
            {

                if (myEdibleType == edibleType.bone && myPath == flightPath.straight) { ApplyBoomerangMotion(); }

                if (!isDestroyed && IsOnFloor() || !isDestroyed && IsOnWall() || !isDestroyed && IsOnCeiling())
                {
                    Destroy();
                }


            }
            ApplyGravity();
            Velocity = edibleVelocity;
            MoveAndSlide();
            AnimateEdible();
        }
        if(isPoppingOut && !IsOnFloor())
        {
            edibleSprite.Rotation = edibleVelocity.Angle()-  Mathf.Pi / 2.0f;
        }else if(isPoppingOut && IsOnFloor())
        {
            edibleSprite.Rotation = 0;
            isPoppingOut = false;
        }


    }
    public void SetInitialDirection(Vector2 direction)
    {
        edibleVelocity = direction;
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
        if (isFlying)
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
            SetEnemyCollisionMask();
        }
        else
        {

            if (!IsOnFloor())
            {
                edibleVelocity.Y += gravity;
            }
            else
            {
                edibleVelocity.Y = 0;
                if(edibleVelocity.X > 0)
                {
                    edibleVelocity.X -= 1;
                }else if(edibleVelocity.X < 0)
                {
                    edibleVelocity.X +=1;
                }
            }


        }

    }

    private void SetEnemyCollisionMask()
    {
        if (!isEnemyMaskSet)
        {
            SetCollisionMaskValue(5, true);
            isEnemyMaskSet = true;
        }
    }
    private void ApplyBoomerangMotion()
    {
        float range = 200;
        if (myDirection == flightDirection.left) { range = -200; }
        Vector2 target = new Vector2(initialPosition.X + range, initialPosition.Y);
        float distance = target.Length();

        edibleVelocity.X -= 1 / distance;
    }

    private void Bounce()
    {
        if (bounceCounter < 3)
        {
            if (IsOnFloor())
            {
                edibleVelocity.Y = -bounceVelocity;
                bounceCounter++;
            }
            if (IsOnWall())
            {
                if (edibleVelocity.X > 0) { edibleVelocity.X = -bounceVelocity; }
                else if (edibleVelocity.X < 0) { edibleVelocity.X = bounceVelocity; }
                bounceCounter++;
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

    public void SetRandomEdibleType()
    {
        float rand = GD.Randf();
        if(rand > .75)
        {
            myEdibleType = edibleType.egg;
        }
        else
        {
            myEdibleType = edibleType.rock;
        }
    }

    public void SetParticlesAndSprites()
    {
        switch (myEdibleType)
        {
            case edibleType.rock:
                fXType1 = effectType.dust;
                fXType2 = effectType.rocks;
                edibleSprite.Texture = rockTexture;
                fXCounter = 2;
                break;
            case edibleType.egg:
                fXType1 = effectType.eggshells;
                fXType2 = effectType.yolk;
                fXCounter = 2;
                edibleSprite.Texture = eggTexture;
                edibleSprite.Hframes = 6;
                flightAnimation = "eggglow";
                staticAnimation = "eggglow";
                break;
            case edibleType.bomb:
                fXType1 = effectType.bombbits;
                fXCounter = 1;
                edibleSprite.Texture = bombTexture;
                edibleSprite.Hframes = 17;
                flightAnimation = "bombfly";
                staticAnimation = "bombstatic";
                break;
            case edibleType.bone:
                fXType1 = effectType.bones;
                fXCounter = 1;
                edibleSprite.Texture = boneTexture;
                edibleSprite.Hframes = 4;
                flightAnimation = "bonefly";
                break;
            case edibleType.water:
                fXType1 = effectType.water;
                fXCounter = 1;
                edibleSprite.Texture = waterTexture;
                edibleSprite.Hframes = 6;
                flightAnimation = "waterfly";
                break;
            case edibleType.bug:
                fXType1 = effectType.goo;
                fXCounter = 1;
                edibleSprite.Texture = bugTexture;
                edibleSprite.Hframes = 4;
                flightAnimation = "bugfly";
                break;
            case edibleType.key:
                fXCounter = 0;
                break;
        }
    }

    private void AnimateEdible()
    {
        if (isFlying)
        {
            if (edibleSprite.Hframes > 1)
            {
                edibleAnim.Play(flightAnimation);
            }
        }
        else
        {
            if (myEdibleType == edibleType.bomb || myEdibleType == edibleType.egg)
            {
                edibleAnim.Play(staticAnimation);
            }
            if (myEdibleType == edibleType.water)
            {
                if (IsOnFloor())
                {
                    edibleSprite.Frame = 1;
                }
                else
                {
                    edibleSprite.Frame = 0;
                }
            }
        }

    }

    public void Destroy()
    {
        edibleSprite.Visible = false;

        if (fXCounter >= 1)
        {
            myParticles1 = EffectsManager.Instance.GetParticleScene(fXType1).Instantiate<GpuParticles2D>();
            AddChild(myParticles1);
            myParticles1.Restart();
        }
        if (fXCounter == 2)
        {
            myParticles2 = EffectsManager.Instance.GetParticleScene(fXType2).Instantiate<GpuParticles2D>();
            AddChild(myParticles2);
            myParticles2.Restart();
        }
        destroyTimer.Start();
        isDestroyed = true;
    }

    public void ConsumeEdible()
    {
        if (myEdibleType == edibleType.bug)
        {
            EmitSignal(SignalName.EdibleEnemyConsumed);

        }
        else
        {
            QueueFree();
        }

    }

    private void OnDestroyTimerTimeout()
    {

        if (myEdibleType == edibleType.bug)
        {
            EmitSignal(SignalName.EdibleEnemyConsumed);
        }
        else
        {
            QueueFree();
        }

    }

    public void SetEatenProperty(bool isEaten)
    {
        hasBeenEaten = isEaten;
    }

    public bool GetEatenProperty()
    {
        return hasBeenEaten;
    }

    public bool GetFlyingStatus()
    {
        return isFlying;
    }
}
