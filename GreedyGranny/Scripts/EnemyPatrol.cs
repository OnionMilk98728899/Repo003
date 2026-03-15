using Godot;
using System;

public partial class EnemyPatrol : Node2D
{
    [Export] private int speed;
    [Export] private int gravity;
    [Export] private int MAX_SPEED;
    [Export] public int MAX_GRAVITY;
    [Export] public float MAX_BOUNCE_HEIGHT;
    private float bounceHeight, bounceApex;
    public float elapsedTime = 0;
    private int directionMod = 1;
    public Vector2 myVelocity;
    private float bounceVelocityX, bounceVelocityY;
    public RayCast2D leftGroundCheck, rightGroundCheck, leftWallCheck, rightWallCheck;
    private bool isTurning, wallDelayActive;
    public bool isBounced, hasBegunBouncing, isMotionless, isDead, isOnFloor, finishedBouncing = true;
    private Sprite2D mySprite;
    private Timer bounceTimer, wallDetectorTimer;
    

    public override void _Ready(){

        leftGroundCheck = GetNode<RayCast2D>("../FallDetectorLeft");
        rightGroundCheck = GetNode<RayCast2D>("../FallDetectorRight");
        rightWallCheck = GetNode<RayCast2D>("../WallDetectorRight");
        leftWallCheck = GetNode<RayCast2D>("../WallDetectorLeft");
        mySprite = GetNode<Sprite2D>("../EnemySprite");
        bounceTimer = GetNode<Timer>("BounceTimer");
        wallDetectorTimer = GetNode<Timer>("WallDetectorDelayTimer");

    }

    public override void _PhysicsProcess(double delta){

        ApplyMovement(delta);

        if(isOnFloor && myVelocity.Y > 0){
            myVelocity.Y = 0;
        }
    }

    public void ApplyGravity(double delta){
    
        myVelocity.Y += gravity * (float)delta;

    }
    private void ApplyMovement(double delta){

        myVelocity.X += speed * directionMod * (float)delta;

        if(myVelocity.X > MAX_SPEED){
            myVelocity.X = MAX_SPEED;
        }
        if(myVelocity.X < -MAX_SPEED){
            myVelocity.X = -MAX_SPEED;  
        }

        if(myVelocity.X < -10 || myVelocity.X > 10){
            isTurning = false;
        }

        if(myVelocity.X > 1){
            mySprite.FlipH = true;
        }
        if(myVelocity.X < -1){
            mySprite.FlipH = false;
        }

        if(!leftGroundCheck.IsColliding() || leftWallCheck.IsColliding()
        //|| !rightGroundCheck.IsColliding() 
        //|| rightWallCheck.IsColliding() || leftWallCheck.IsColliding()
        ){



            if(!isTurning){
                myVelocity.X = 0;
                isTurning = true;
            }
            if(!wallDelayActive){
                
                rightWallCheck.Enabled = false;
                leftWallCheck.Enabled = false;
                wallDetectorTimer.Start();
                wallDelayActive = true;
            }
   
            directionMod = 1;
        }

        if(!rightGroundCheck.IsColliding() || rightWallCheck.IsColliding()
        ){

            if(!isTurning){
                myVelocity.X = 0;
                isTurning = true;
            }

            if(!wallDelayActive){
                
                rightWallCheck.Enabled = false;
                leftWallCheck.Enabled = false;
                wallDetectorTimer.Start();
                wallDelayActive = true;
            }

            directionMod = -1;
        }

        if(isMotionless && !isBounced){
            myVelocity.X =0;
        }

        if(isBounced){

            elapsedTime += (float)delta;
            if(!isOnFloor &&  myVelocity.Y < 50){
                myVelocity.Y +=  elapsedTime*(1000)/speed;
            }

            myVelocity.X +=  (1.5f) * bounceVelocityX * speed * (float)delta;
            
        }else{
            elapsedTime = 0;
        }

    }

    public void BounceOffOfPlayer(float playerVel){

        if(!hasBegunBouncing){

            bounceTimer.Start();
            bounceVelocityX = playerVel;
            hasBegunBouncing = true;
            finishedBouncing = false;
            myVelocity.Y =  -60;
        }
        
    }

    public void DetermineFloorPosition(Vector2 position){

        bounceHeight = position.Y;
        bounceApex = bounceHeight + MAX_BOUNCE_HEIGHT;
    }

    private void OnBounceTimerTimeout(){
        isBounced = false;
        hasBegunBouncing = false;
    }
    
    private void OnWallDetectorDelayTimeout(){
        wallDelayActive = false;
        leftWallCheck.Enabled = true;
        rightWallCheck.Enabled = true;
    }
}
