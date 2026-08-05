using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public partial class Player : CharacterBody2D
{
    [Export] Sprite2D playerSprite;
    [Export] AnimationPlayer playerAnim;
    [Export] private float moveSpeed, maxMoveSpeed, jumpPower, hiJumpPower, gravity, climbSpeed, maxClimbSpeed, chargeSpeed, stompSpeed;
    [Export] private Label debugLabel;
    [Export] private Timer recoverTimer, specialBufferTimer;
    [Export] private CollisionShape2D playerColl;
    public enum moveState { idle, walk, jump, fall, land, eat, climb, idleclimb }
    public enum lifeState { alive, hurt, dying }
    public enum eatingState { full, empty }
    public enum specialState { none, charge, chargeland, stomp, stompland, spit, spitup, spitdown, digest, doorenter, doorexit, tubeenter, tubeexit }
    private moveState currentMoveState;
    private lifeState currentLifeState;
    private eatingState currentEatingState;
    private specialState currentspecialState;
    private List<Edible> edibleList = new List<Edible>();
    [Export] private PackedScene edibleScene;
    private Edible myEdible;
    private Edible.edibleType edibleInMouth;
    private WarpTile currentWarpTile, destinationWarpTile;
    private Vector2 inputDirection, playerVelocity;
    private Vector2 MOUTH_POSITION_OFFSET = new Vector2(0, -8);
    private bool isJumping, isJumpReset, isTouchingLadder, isClimbing, isAboveLadder, isSpitting, isCharging, isStomping, isFalling, isLanding,
    isMoving, isHurt, canEat, isEating, isStompLanding, isChargeLanding, isEnteringDoor, isExitingDoor, isEnteringTube, isExitingTube;
    public bool isTouchingDoor;
    public Vector2 doorEntryInput;

    public override void _Ready()
    {
        EventBus.Instance.HurtPlayer += OnPlayerHurt;
        EventBus.Instance.ChargeEnemy += BounceBackFromCharge;
        inputDirection.X = 1;
        currentEatingState = eatingState.empty;

    }

    public override void _PhysicsProcess(double delta)
    {

        if (currentLifeState != lifeState.alive)
        {
            HandleHurtPhysics();
        }
        else if (currentspecialState != specialState.none)
        {
            HandleSpecialInput();

        }
        else if (currentspecialState == specialState.none)
        {
            HandleDirectionalInput();
            HandleJumpInput();
            HandleClimbInput();
            HandleDoorInput();
            HandleSpecialInput();

        }

        ApplyGravity();
        AnimatePlayer(DetermineState());
        Velocity = playerVelocity;
        MoveAndSlide();

    }


    private void HandleDirectionalInput()
    {
        if (Input.IsActionPressed("ui_left")) inputDirection.X = -1;
        else if (Input.IsActionPressed("ui_right")) inputDirection.X = 1;
        else inputDirection.X = 0;

        if (Input.IsActionPressed("ui_up")) inputDirection.Y = -1;
        else if (Input.IsActionPressed("ui_down")) inputDirection.Y = 1;
        else inputDirection.Y = 0;

        if (inputDirection.X != 0)
        {
            playerVelocity.X += inputDirection.X * moveSpeed;
            playerVelocity.X = Mathf.Clamp(playerVelocity.X, -maxMoveSpeed, maxMoveSpeed);
            if (inputDirection.X > 0)
            {
                playerSprite.FlipH = false;
            }
            else
            {
                playerSprite.FlipH = true;
            }

        }
        else
        {
            playerVelocity.X = 0;
        }
    }
    private void HandleJumpInput()
    {
        if (IsOnFloor())
        {
            if (Input.IsActionJustPressed("ActionZ") && isJumpReset)
            {
                if (inputDirection.Y < 0) { playerVelocity.Y -= hiJumpPower; }
                else { playerVelocity.Y -= jumpPower; }
                isJumping = true;
                isClimbing = false;
            }
        }
    }

    private void ApplyGravity()
    {
        if (!IsOnFloor() && !isClimbing)
        {
            playerVelocity.Y += gravity;
            isJumpReset = false;
        }
        if (IsOnFloor() && !isJumping && !isClimbing)
        {
            isJumpReset = true;
            playerVelocity.Y = 0;
        }
    }

    private void HandleClimbInput()
    {
        if (inputDirection.Y != 0 && isTouchingLadder || inputDirection.Y == 1 && isAboveLadder)
        {
            isClimbing = true;
            isJumping = false;
            playerVelocity.Y += inputDirection.Y * climbSpeed;
            playerVelocity.Y = Mathf.Clamp(playerVelocity.Y, -maxClimbSpeed, maxClimbSpeed);
        }
        if (inputDirection.Y == 1 && isAboveLadder)
        {
            SetCollisionMaskValue(3, false);
        }
        if (isClimbing)
        {

            if (inputDirection.Y == 0)
            {
                playerVelocity.Y = 0;
            }
        }

    }

    private void HandleDoorInput()
    {
        if (currentMoveState == moveState.idle || currentMoveState == moveState.walk)
        {
            if (isTouchingDoor && doorEntryInput == inputDirection)
            {
                isEnteringDoor = true;
                playerVelocity = Vector2.Zero;
            }
        }
    }

    private void HandleSpecialInput()
    {
        if (Input.IsActionJustPressed("ActionX"))
        {
            if (currentEatingState != eatingState.full)
            {
                if (!isCharging && !isStomping && specialBufferTimer.IsStopped())
                {
                    if (IsOnFloor() && !isStompLanding)
                    {

                        if (playerSprite.FlipH)
                        {
                            playerVelocity.X = -chargeSpeed;
                        }
                        else
                        {
                            playerVelocity.X = chargeSpeed;
                        }

                        isCharging = true;
                    }
                    else if (!IsOnFloor() && !isStompLanding)
                    {
                        playerVelocity.Y = stompSpeed;
                        isStomping = true;
                        //currentspecialState = specialState.stomp;
                    }
                }
            }
            else     ////////// Spit logic
            {
                SpitOutEdible();
                isSpitting = true;
                if (inputDirection.X == 0) { playerVelocity.X = 0; }
            }

        }
        if (isCharging)
        {
            if (IsOnWall())
            {
                BounceBackFromCharge(0);

            }
        }
        if (IsOnFloor())
        {
            isJumpReset = true;
        }
        if (isChargeLanding)
        {
            if (playerSprite.FlipH)
            {
                playerVelocity.X -= .05f * moveSpeed;
            }
            else
            {
                playerVelocity.X += .05f * moveSpeed;
            }
        }

        if (isStomping)
        {
            if (IsOnFloor())
            {
                playerVelocity.X = 0;
                playerVelocity.Y = 0;
                isJumpReset = true;
                isStompLanding = true;
                isStomping = false;
                recoverTimer.Start();
            }
            else
            {
                playerVelocity.X = 0;
            }
        }
    }

    private void HandleHurtPhysics()
    {
        if (isHurt)
        {
            if (playerSprite.FlipH)
            {
                playerVelocity.X -= .05f * moveSpeed;
            }
            else
            {
                playerVelocity.X += .05f * moveSpeed;
            }

        }
    }

    private string DetermineState()
    {
        string state = "";
        if (currentLifeState != lifeState.alive)
        {
            state = currentLifeState.ToString();
        }
        else
        {
            if (isCharging) { currentspecialState = specialState.charge; }
            if (isStomping) { currentspecialState = specialState.stomp; }
            if (isStompLanding) { currentspecialState = specialState.stompland; }
            if (isChargeLanding) { currentspecialState = specialState.chargeland; }
            if (isSpitting)
            {
                if (inputDirection.Y == 0)
                {
                    currentspecialState = specialState.spit;
                }
                else if (inputDirection.Y > 0)
                {
                    currentspecialState = specialState.spitdown;
                }
                else
                {
                    currentspecialState = specialState.spitup;
                }
            }
            if (isEnteringDoor)
            {
                if (currentWarpTile.myDoorType != WarpTile.doorType.tube)
                {
                    currentspecialState = specialState.doorenter;
                }
                else { currentspecialState = specialState.tubeenter; }
            }
            if (isExitingDoor)
            {
                if (destinationWarpTile.myDoorType != WarpTile.doorType.tube)
                {
                    currentspecialState = specialState.doorexit;
                }
                else
                {
                    currentspecialState = specialState.tubeexit;
                }

            }

            if (currentspecialState != specialState.none)
            {
                state = currentspecialState.ToString();
            }
            else if (currentspecialState == specialState.none)
            {
                if (IsOnFloor())
                {
                    if (isFalling)
                    {
                        currentMoveState = moveState.land;
                        isFalling = false;
                        isLanding = true;
                        recoverTimer.Start();
                    }
                    if (!isLanding && !isEating)
                    {
                        if (playerVelocity.X == 0) { currentMoveState = moveState.idle; }
                        else { currentMoveState = moveState.walk; }
                    }
                    if (inputDirection.Y == 1 && canEat)
                    {
                        isEating = true;
                        currentMoveState = moveState.eat;
                        EatNearestEdible();
                    }

                }
                else
                {
                    if (!isClimbing)
                    {
                        if (isJumping) { currentMoveState = moveState.jump; }
                        if (playerVelocity.Y > 0) { isJumping = false; isFalling = true; currentMoveState = moveState.fall; }

                        if (inputDirection.Y == 1 && canEat)
                        {
                            currentMoveState = moveState.eat;
                            EatNearestEdible();
                        }
                    }
                    else
                    {
                        if (playerVelocity.Y != 0) { currentMoveState = moveState.climb; }
                        else { currentMoveState = moveState.idleclimb; }
                    }

                }

                if (currentEatingState == eatingState.full && currentMoveState != moveState.eat &&
                 currentMoveState != moveState.climb && currentMoveState != moveState.idleclimb)
                {
                    state = currentMoveState + currentEatingState.ToString();
                }
                else
                {
                    state = currentMoveState.ToString();
                }
            }
        }

        debugLabel.Text = state;
        return state;
    }

    private void EatNearestEdible()
    {
        if (edibleList.Count > 0)
        {
            float closestX = 100;
            Edible closestEdible = edibleList[0];
            foreach (Edible edible in edibleList)
            {
                if (Mathf.Abs(GlobalPosition.X - edible.GlobalPosition.X) < closestX)
                {
                    closestEdible = edible;
                }
            }

            edibleList.Remove(closestEdible);
            edibleInMouth = closestEdible.myEdibleType;
            closestEdible.ConsumeEdible();
            currentEatingState = eatingState.full;

            if (edibleList.Count <= 0)
            {
                canEat = false;
            }
        }
    }

    private void SpitOutEdible()
    {
        myEdible = edibleScene.Instantiate<Edible>();
        myEdible.GlobalPosition = GlobalPosition + MOUTH_POSITION_OFFSET;
        EdibleManager.Instance.AddChild(myEdible);

        if (playerSprite.FlipH) { myEdible.myDirection = Edible.flightDirection.left; }
        else { myEdible.myDirection = Edible.flightDirection.right; }


        if (IsOnFloor() && inputDirection.Y == 0)
        {
            myEdible.myPath = Edible.flightPath.straight;
        }
        else if (!IsOnFloor() && inputDirection.Y == 0)
        {
            myEdible.myPath = Edible.flightPath.arc;
        }
        else if (inputDirection.Y > 0)
        {
            myEdible.myPath = Edible.flightPath.down;
        }
        else if (inputDirection.Y < 0)
        {
            myEdible.myPath = Edible.flightPath.lob;
        }

        myEdible.SetFlight();

        GD.Print("Spitting!");
    }

    private void InteractWithDoors()
    {
        if (currentMoveState == moveState.idle || currentMoveState == moveState.walk)
        {
            isTouchingDoor = true;
        }
        else
        {
            isTouchingDoor = false;
        }
    }

    private void BounceBackFromCharge(int damage)   //// damage argument is hold-over from Event Signal
    {
        playerVelocity.X = -playerVelocity.X;
        isCharging = false;
        isChargeLanding = true;
    }

    private void AnimatePlayer(string state)
    {
        playerAnim.Play(state);
    }

    private void OnPlayerHurt(int damage)
    {
        currentLifeState = lifeState.hurt;
        if (playerSprite.FlipH)
        {
            playerVelocity.X = 2 * moveSpeed;
        }
        else
        {
            playerVelocity.X = -2 * moveSpeed;
        }

        isHurt = true;
    }

    ///////////////////////////////////////////////////// CALL METHOD TRACKS ///////////////////////////////////////////////////////////////////

    private void OnSpecialAnimationFinished()
    {
        currentspecialState = specialState.none;
        specialBufferTimer.Start();

        isCharging = false;
        isChargeLanding = false;
        isStomping = false;
        if (isSpitting)
        {
            isSpitting = false;
            currentEatingState = eatingState.empty;
        }
        if (IsOnFloor())
        {
            isLanding = false;
            isFalling = false;
            isJumpReset = true;
        }
    }

    private void OnDoorEnteredAnimationFinished()
    {
        isEnteringDoor = false;
        isExitingDoor = true;
        currentWarpTile.WarpPlayer();
    }

    private void OnDoorExitAnimationFinished()
    {
        isExitingDoor = false;
        currentspecialState = specialState.none;
    }
    // private void OnChargeAnimationFinished()
    // {
    //     currentspecialState = specialState.none;
    //     specialBufferTimer.Start();
    //     isCharging = false;
    //     isChargeLanding = false;
    // }
    // private void OnStompAnimationFinished()
    // {
    //     currentspecialState = specialState.none;
    //     specialBufferTimer.Start();
    //     isStomping = false;
    // }
    private void OnHurtAnimationFinished()
    {
        currentLifeState = lifeState.alive;
        currentspecialState = specialState.none;
        isFalling = false;
        isStomping = false;
    }

    private void OnEatAnimationFinished()
    {
        isEating = false;
    }

    //////////////////////////////////////////////////////////   DETECTORS ////////////////////////////////////////////////////
    private void OnLadderDetectorBodyEntered(Node2D body)
    {
        isTouchingLadder = true;
    }

    private void OnLadderDetectorBodyExited(Node2D body)
    {
        isTouchingLadder = false;
        isClimbing = false;

    }

    private void OnLadderTopDetectorBodyEntered(Node2D body)
    {
        isAboveLadder = true;
    }

    private void OnLadderTopDetectorBodyExited(Node2D body)
    {
        isAboveLadder = false;
        SetCollisionMaskValue(3, true);
    }

    private void OnEdibleDetectorBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Edible"))
        {
            Edible myEdible = body.GetNode<Edible>(".");
            edibleList.Add(myEdible);
            canEat = true;
        }
    }

    private void OnEdibleDetectorBodyExited(Node2D body)
    {
        if (body.IsInGroup("Edible"))
        {
            Edible myEdible = body.GetNode<Edible>(".");
            edibleList.Remove(myEdible);
            if (edibleList.Count == 0)
            {
                canEat = false;
            }
        }
    }

    ////////////////////////////////////////////////////////////  TIMERS  ///////////////////////////////////////////
    private void OnRecoverTimerTimeout()
    {
        if (isLanding) { isLanding = false; }
        if (isStompLanding)
        {
            isStompLanding = false;
            isFalling = false;
            currentspecialState = specialState.none;
        }
    }

    private void OnSpecialBufferTimerTimeout()
    {

    }


    ///////////////////////////////////////////// EXPOSERS ////////////////////////////////////////////

    public specialState GetSpecialState()
    {
        return currentspecialState;
    }

    public moveState GetMoveState()
    {
        return currentMoveState;
    }

    public void SetCurrentWarpTiles(WarpTile currentDoor, WarpTile targetDoor)
    {
        currentWarpTile = currentDoor;
        destinationWarpTile = targetDoor;
    }
}
