using Godot;
using System;

public partial class brown_box : CharacterBody2D
{
    [Export] private int gravity, speed, acceleration;
    private Label StateLabel;
    private Vector2 myVelocity, noMovement;
    private PlayerManager playerMan;
    private CollisionShape2D myCollider;
    private Sprite2D mySprite;
    private PlayerCarryable playerCarryable;
    private granny myGranny;
    private State currentState, nextState;
    private bool carryable, carried, thrown, thrownInAir;
    private Timer breakTimer;

    public enum State{
        Falling,
        Idle, 
        Carryable,
        Carried,
        Thrown,
        Breaking,
        Broken
    } 
    public override void _Ready(){

        currentState = State.Idle;
        StateLabel = GetNode<Label>("StateLabel");
        playerMan = GetNode<PlayerManager>("../PlayerManager");
        mySprite = GetNode<Sprite2D>("BoxSprite");
        myCollider = GetNode<CollisionShape2D>("BoxBorder");
        breakTimer = GetNode<Timer>("BreakTimer");

        noMovement = new Vector2(0, 0);
        
    }

    public override void _PhysicsProcess(double delta)
    {   
        DetermineState(delta);
        ApplyGravity(delta);
        ActivatePlayerCarry();

        if(carryable){
            CheckToDropCarryable();
        }
        
        Velocity = myVelocity;
        MoveAndSlide();
        currentState = nextState;

        
        StateLabel.Text = currentState.ToString() +  "  " + Mathf.Round(myVelocity.X) + "    " + Mathf.Round(myVelocity.Y) + "  " + IsOnFloor() + "\n" + 
        breakTimer.TimeLeft;
    }

    private void DetermineState(double delta){

        if(!IsOnFloor()){

            if(currentState == State.Thrown && thrownInAir){
                if(myGranny.currentDirection == granny.Direction.Left){myVelocity.X = -150;}
                if(myGranny.currentDirection == granny.Direction.Right){myVelocity.X = 150;}
                thrownInAir = false;
            }

            if(myVelocity.X <= 0 && currentState != State.Thrown){
                nextState = State.Falling;
            }
            
        }
        if(IsOnFloor()){

            if(carryable){
                nextState = State.Carryable;
            }else{
                nextState = State.Idle;
            }

            if(thrown){
                nextState = State.Breaking;
                breakTimer.Start();
                thrown = false;
                
            }

        }
    }

    private void ActivatePlayerCarry(){

        if(carryable && playerMan.carryTriggered){
            nextState = State.Carried;
            playerCarryable.ActivateCarryable();
            myGranny.carrying = true;
            mySprite.Visible = false;
            myCollider.Disabled = true;
            carried = true;
            
        }
    }

    private void CheckToDropCarryable(){

        if(playerCarryable.carrying){
            if(!myGranny.carrying){
                ThrowCarriedBox();
            }
        }
    }

    private void DropCarriedBox(){

        GlobalPosition = new Vector2(myGranny.Position.X +15, myGranny.Position.Y);
        playerCarryable.DeactivateCarryable();
        myGranny.carrying = false;
        mySprite.Visible = true;
        myCollider.Disabled = false;
    }

    private void ThrowCarriedBox(){

        GlobalPosition = new Vector2(myGranny.Position.X , myGranny.Position.Y - 32);
        nextState = State.Thrown;
        playerCarryable.DeactivateCarryable();
        myGranny.carrying = false;
        mySprite.Visible = true;
        myCollider.Disabled = false;
        carried = false;
        thrown = true;
    }

    private void ApplyGravity(double delta){

        if(!IsOnFloor() && currentState != State.Carried){
            myVelocity.Y += gravity * (float)delta;
            
        }
        if(IsOnFloor()){

            myVelocity.Y = 0;
            myVelocity.X = 0;
        }
        
    }

    private void OnArea2DBodyEntered(PhysicsBody2D body){
        if (body == null){
            carryable = false;
            return;
        }

       
        if(body.IsInGroup("Player")){
            myGranny = body.GetNode<granny>(".");
            playerCarryable =  body.GetNode<PlayerCarryable>("PlayerCarryable");
            carryable = true;
        }
    }

    private void OnArea2DBodyExited(PhysicsBody2D body){

        if(body.IsInGroup("Player")){

            if(!carried && !thrown){
                carryable = false;
            }
 
        }
    }

    private void OnBreakTimerTimeout() {
        QueueFree();
    }

}
