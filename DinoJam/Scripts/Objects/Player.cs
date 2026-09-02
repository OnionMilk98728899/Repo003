using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public partial class Player : CharacterBody2D
{
    [Export] Sprite2D playerSprite;
    [Export] AnimationPlayer playerAnim;
    [Export] private float moveSpeed, maxMoveSpeed, jumpPower, hiJumpPower, gravity, maxGravity, climbSpeed, maxClimbSpeed, chargeSpeed, stompSpeed, bounceBack;
    [Export] private Label debugLabel;
    [Export] private Timer recoverTimer, specialBufferTimer, deathTimer;
    [Export] public Timer tileBreakBufferTimer;
    [Export] private CollisionShape2D playerColl;
    public enum moveState { idle, walk, jump, fall, land, eat, digest, climb, idleclimb }
    public enum lifeState { alive, hurt, dying }
    public enum eatingState { full, empty }
    public enum specialState
    {
        none, charge, chargeland, stomp, stompland, spit, spitup,
        spitdown, doorenter, doorexit, tubeenter, tubeexit
    }
    private moveState currentMoveState;
    private lifeState currentLifeState;
    private eatingState currentEatingState;
    private specialState currentspecialState;
    private List<Edible> edibleList = new List<Edible>();
    [Export] private PackedScene edibleScene, waterDropScene;
    private Edible myEdible, myWaterDrop;
    private Edible.edibleType edibleInMouth;
    private WarpTile currentWarpTile, destinationWarpTile;
    private BreakableTile currentBreakableSideTile, currentBreakableTopTile, currentBreakableBottomTile;
    private Tree currentTree;
    private Vector2 inputDirection, playerVelocity, levelOriginPosition;
    private Vector2 MOUTH_POSITION_OFFSET = new Vector2(0, -8);
    private bool isJumping, isJumpReset, isTouchingLadder, isClimbing, isAboveLadder, isSpitting, isCharging, isStomping, isFalling, isLanding,
    isMoving, isHurt, canEat, isEating, isDigesting, isStompLanding, isChargeLanding, isEnteringDoor, isExitingDoor, isEnteringTube, isExitingTube, isCancellingCharge,
    isBouncing, isTouchingBreakableTileLeft, isTouchingBreakableTileRight, isTouchingBreakableTileBottom, isTouchingBreakableTileTop, isTouchingTreeCharge,
    isTouchingTreeStomp, isTreeDetectorOn, isShakingTree;
    public bool isTouchingDoor;
    private string deathType;
    public Vector2 doorEntryInput;

    public override void _Ready()
    {
        EventBus.Instance.HurtPlayer += OnPlayerHurt;
        EventBus.Instance.HurtPlayerTimeout += OnPlayerHurtTimeout;
        EventBus.Instance.KillPlayer += OnPlayerKilled;
        EventBus.Instance.ChargeEnemy += BounceBackFromCharge;
        EventBus.Instance.BouncePlayerUpwards += OnPlayerBouncedUpward;
        EventBus.Instance.RepositionPlayerOrigin += OnPlayerOriginRepositioned;
        inputDirection.X = 1;
        currentEatingState = eatingState.empty;
        levelOriginPosition = GlobalPosition;
        //Engine.TimeScale = .5f;
    }

    public override void _PhysicsProcess(double delta)
    {

        if (currentLifeState != lifeState.alive)
        {
            HandleHurtPhysics();
            //HandleDirectionalInput();
        }
        else if (currentspecialState != specialState.none)
        {
            HandleSpecialInput();
            DetectTrees();
            if (isSpitting)
            {
                HandleDirectionalInput();
            }
        }
        else if (currentspecialState == specialState.none)
        {
            HandleDirectionalInput();
            HandleJumpInput();
            HandleClimbInput();
            HandleDoorInput();
            HandleSpecialInput();
            HandleTreeInput();
        }

        DetectHazardsAsFloorIfHurt();
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
                isEating = false;
            }
        }
        if (IsOnCeiling() && inputDirection.Y == -1 && currentBreakableTopTile != null)
        {
            DetectBreakableTiles();
        }
    }

    private void ApplyGravity()
    {
        if (!IsOnFloor() && !isClimbing)
        {
            if (playerVelocity.Y < maxGravity)
            {
                playerVelocity.Y += gravity;
                isJumpReset = false;
            }
        }
        if (IsOnFloor() && !isJumping && !isClimbing && !isBouncing)
        {
            isJumpReset = true;
            playerVelocity.Y = 0;
        }
    }

    private void HandleClimbInput()
    {
       // if (!isEating && !isDigesting && !canEat)
       // {
            if (inputDirection.Y != 0 && isTouchingLadder || inputDirection.Y == 1 && isAboveLadder)
            {
                isClimbing = true;
                isJumping = false;
                playerVelocity.Y += inputDirection.Y * climbSpeed;
                playerVelocity.Y = Mathf.Clamp(playerVelocity.Y, -maxClimbSpeed, maxClimbSpeed);
            }
        //}

        if (!isTouchingLadder)
        {
            isClimbing = false;
        }
        if (inputDirection.Y == 1 && isAboveLadder 
       // && currentMoveState != moveState.eat && currentMoveState != moveState.digest && currentEatingState == eatingState.empty
        ){
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

    private void DetectHazardsAsFloorIfHurt()
    {
        if (isHurt) { SetCollisionMaskValue(7, true); }
        else { SetCollisionMaskValue(7, false); }
    }

    private void HandleTreeInput()
    {
        if (currentspecialState == specialState.none && isShakingTree)
        {
            isShakingTree = false;
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
                currentWarpTile.AnimateTube();
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
                    if (IsOnFloor() && !isStompLanding && !isChargeLanding)
                    {

                        if (playerSprite.FlipH)
                        {
                            playerVelocity.X = -chargeSpeed;

                        }
                        else
                        {
                            playerVelocity.X = chargeSpeed;
                            // if (isTouchingBreakableTileRight)
                            // {
                            //     EventBus.Instance.EmitSignal(EventBus.SignalName.BreakableTileBroken, currentBreakableTile);
                            // }
                        }

                        isCharging = true;
                    }
                    else if (!IsOnFloor() && !isStompLanding && !isBouncing)
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
                if (TryToGetWallPosition(out Vector2 wallPosition, out bool wallIsLeft))
                {
                    if (wallIsLeft && playerSprite.FlipH || !wallIsLeft && !playerSprite.FlipH)
                    {
                        BounceBackFromCharge(0, Vector2.Zero);
                        DetectBreakableTiles();
                    }

                }

            }
            if (Input.IsActionJustPressed("ActionZ"))
            {
                isCancellingCharge = true;
                specialBufferTimer.Stop();
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
                isJumping = false;
                isStompLanding = true;
                isStomping = false;
                recoverTimer.Start();
                DetectBreakableTiles();
            }
            else
            {
                playerVelocity.X = 0;
            }
        }
        if (isSpitting)
        {
            if (IsOnFloor())
            {
                isJumpReset = true;
                isJumping = false;
                playerVelocity.Y = 0;
            }
        }
    }

    private void HandleHurtPhysics()
    {
        if (isHurt && IsOnFloor())
        {
            if (playerVelocity.X > 0)
            {
                playerVelocity.X -= .1f * moveSpeed;
            }
            else
            {
                playerVelocity.X += .1f * moveSpeed;
            }


            if (Input.IsActionJustPressed("ActionZ") && isJumpReset)
            {
                if (inputDirection.Y < 0) { playerVelocity.Y -= hiJumpPower; }
                else { playerVelocity.Y -= jumpPower; }
                isJumping = true;
                isClimbing = false;
                isEating = false;
            }

        }
        else if (isHurt && !IsOnFloor())
        {
            HandleDirectionalInput();
        }
        if (IsOnFloor()) { isJumpReset = true; }
        if (currentLifeState == lifeState.dying)
        {
            playerVelocity = Vector2.Zero;
        }


    }

    private void DetectBreakableTiles()
    {
        if (isCharging || isChargeLanding)
        {
            if (isTouchingBreakableTileLeft && !playerSprite.FlipH || isTouchingBreakableTileRight && playerSprite.FlipH)
            {
                EventBus.Instance.EmitSignal(EventBus.SignalName.BreakableTileBroken, currentBreakableSideTile);
                if (isTouchingBreakableTileLeft) { isTouchingBreakableTileLeft = false; }
                if (isTouchingBreakableTileRight) { isTouchingBreakableTileRight = false; }
            }
        }

        if (isTouchingBreakableTileBottom && inputDirection.Y == -1 && !isCharging && !isChargeLanding && !isStomping && tileBreakBufferTimer.IsStopped())
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.BreakableTileBroken, currentBreakableTopTile);
            isTouchingBreakableTileBottom = false;
        }

        if (isStomping || isStompLanding)
        {
            if (isTouchingBreakableTileTop)
            {
                EventBus.Instance.EmitSignal(EventBus.SignalName.BreakableTileBroken, currentBreakableBottomTile);
                isTouchingBreakableTileTop = false;
                playerVelocity.Y *= .5f;
            }
        }


    }

    private void DetectTrees()
    {
        if (isCharging && !isTreeDetectorOn)
        {
            SetCollisionMaskValue(11, true);
            isTreeDetectorOn = true;
        }
        else
        {
            SetCollisionMaskValue(11, false);
            isTreeDetectorOn = false;
        }

        if (isChargeLanding && IsOnWall() && isTouchingTreeCharge && currentTree != null && !isShakingTree ||
        isStompLanding && IsOnFloor() && isTouchingTreeStomp && currentTree != null && !isShakingTree)
        {
            EventBus.Instance.EmitSignal(EventBus.SignalName.ShakeTree, currentTree);
            isShakingTree = true;
        }
    }

    private string DetermineState()
    {
        string state = "";
        if (currentLifeState != lifeState.alive)
        {
            if (currentLifeState == lifeState.hurt)
            {
                state = currentLifeState.ToString();
            }
            else if (currentLifeState == lifeState.dying)
            {
                state = currentLifeState + deathType;
            }

        }
        else
        {
            if (isCharging && !isCancellingCharge) { currentspecialState = specialState.charge; }
            if (isCharging && isCancellingCharge) { currentspecialState = specialState.none; isCharging = false; isCancellingCharge = false; }
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
                    if (isFalling && !isBouncing)
                    {
                        currentMoveState = moveState.land;
                        isFalling = false;
                        isLanding = true;
                        if (!canEat) { isEating = false; }
                        recoverTimer.Start();
                    }
                    if (isBouncing && isFalling)
                    {
                        isFalling = false;
                        currentMoveState = moveState.jump;
                        isJumping = true;
                    }
                    if (!isLanding && !isEating && !isBouncing)
                    {
                        if (playerVelocity.X == 0) { currentMoveState = moveState.idle; }
                        else { currentMoveState = moveState.walk; }
                        if (isDigesting && currentEatingState == eatingState.empty) { currentMoveState = moveState.digest; }
                    }
                    if (inputDirection.Y == 1 && canEat && currentEatingState == eatingState.empty && !isAboveLadder)
                    {

                        isEating = true;
                        currentMoveState = moveState.eat;
                        EatNearestEdible();
                    }
                    if (inputDirection.Y == 1 && currentEatingState == eatingState.full && !isAboveLadder)
                    {
                        if (currentMoveState == moveState.idle || currentMoveState == moveState.walk)
                        {
                            currentEatingState = eatingState.empty;
                            currentMoveState = moveState.digest;
                            isDigesting = true;
                            DigestEdible();
                        }

                    }

                }
                else
                {
                    if (!isClimbing)
                    {
                        if (isJumping) { currentMoveState = moveState.jump; }
                        if (playerVelocity.Y > 0 && !isEating) { isJumping = false; isFalling = true; isBouncing = false; currentMoveState = moveState.fall; }

                        if (inputDirection.Y == 1 && canEat && currentEatingState == eatingState.empty)
                        {
                            currentMoveState = moveState.eat;
                            isEating = true;
                            isJumping = false;
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

        debugLabel.Text = $"{currentMoveState}";

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
                Vector2 toTarget = GlobalPosition - edible.GlobalPosition;
                float distance = toTarget.Length();

                if (distance < closestX)
                {
                    closestEdible = edible;
                }
            }


            edibleList.Remove(closestEdible);
            edibleInMouth = closestEdible.myEdibleType;
            closestEdible.ConsumeEdible();
            currentEatingState = eatingState.full;

            canEat = false;
        }
        else
        {
            return;
        }
    }

    private void SpitOutEdible()
    {
        myEdible = edibleScene.Instantiate<Edible>();
        myEdible.SetEatenProperty(true);
        myEdible.myEdibleType = edibleInMouth;
        //myEdible.SetParticlesAndSprites();
        //myEdible.SetEatenProperty(true);
        myEdible.GlobalPosition = GlobalPosition + MOUTH_POSITION_OFFSET;
        EdibleManager.Instance.AddChild(myEdible);

        if (playerSprite.FlipH) { myEdible.myDirection = Edible.flightDirection.left; }
        else { myEdible.myDirection = Edible.flightDirection.right; }


        if (IsOnFloor() && inputDirection.Y == 0)
        {
            myEdible.myPath = Edible.flightPath.straight;
        }
        else if (!IsOnFloor() && inputDirection.Y == 0 && inputDirection.X == 0)
        {
            myEdible.myPath = Edible.flightPath.arc;
        }
        else if (!IsOnFloor() && inputDirection.Y == 0 && inputDirection.X != 0)
        {
            myEdible.myPath = Edible.flightPath.straight;
        }
        else if (inputDirection.Y > 0)
        {
            myEdible.myPath = Edible.flightPath.down;
        }
        else if (inputDirection.Y < 0)
        {
            myEdible.myPath = Edible.flightPath.lob;
        }

        if (edibleList.Count > 0)
        {
            canEat = true;
        }

        myEdible.SetFlight();
    }
    private void DigestEdible()
    {
        if (edibleInMouth == Edible.edibleType.water)
        {
            GlobalStats.Instance.SetPlayerHealth(1);
        }
    }


    private void EmitWater()
    {
        for (int i = 0; i < GlobalStats.Instance.playerHealth; i++)
        {
            myWaterDrop = waterDropScene.Instantiate<Edible>();
            EdibleManager.Instance.CallDeferred(Node.MethodName.AddChild, myWaterDrop);
            myWaterDrop.isPoppingOut = true;
            myWaterDrop.myEdibleType = Edible.edibleType.water;
            myWaterDrop.SetParticlesAndSprites();
            myWaterDrop.GlobalPosition = GlobalPosition + MOUTH_POSITION_OFFSET;
            int randX = GD.RandRange(-50, 50);
            Vector2 direction = new Vector2(randX, -200);
            myWaterDrop.SetInitialDirection(direction);

        }
    }

    // private void AddNodeToEdibleManager()
    // {
    //      EdibleManager.Instance.AddChild(myWaterDrop);
    // }

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

    private void AnimatePlayer(string state)
    {
        playerAnim.Play(state);
    }

    private bool TryToGetWallPosition(out Vector2 wallPosition, out bool wallIsLeft)
    {
        wallPosition = Vector2.Zero;
        wallIsLeft = false;

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            Vector2 normal = collision.GetNormal();

            // Ignore floor/ceiling collisions
            if (Mathf.Abs(normal.X) < 0.5f)
                continue;

            wallPosition = collision.GetPosition();
            wallIsLeft = wallPosition.X < GlobalPosition.X;

            return true;
        }

        return false;
    }



    //////////////////////////////////////////////////// EVENT BUS SIGNAL CALLS  //////////////////////////////////////////////////////////

    private void BounceBackFromCharge(ulong enemyID, Vector2 strikeVelocity)   //// arguments are a hold-over from Event Signal
    {
        playerVelocity.X = -playerVelocity.X;
        isCharging = false;
        isChargeLanding = true;
    }
    private void OnPlayerHurt(Vector2 attackerPos)
    {
        currentLifeState = lifeState.hurt;
        if (currentMoveState == moveState.fall || attackerPos.Y > GlobalPosition.Y)
        {
            playerVelocity.Y = -bounceBack;
            isBouncing = true;
        }
        else
        {
            if (GlobalPosition.X > attackerPos.X)
            {
                playerVelocity.X = bounceBack;
            }
            else
            {
                playerVelocity.X = -bounceBack;
            }
        }

        isHurt = true;
        EmitWater();
    }

    private void OnPlayerHurtTimeout()
    {
        isHurt = false;
    }
    private void OnPlayerKilled(string death)
    {
        isHurt = true;
        currentLifeState = lifeState.dying;
        deathType = death;
        deathTimer.Start();
    }

    private void OnPlayerBouncedUpward(float bouncePower)
    {
        if (Input.IsActionPressed("ActionZ"))
        {
            playerVelocity.Y = -bouncePower * 1.2f;
        }
        else
        {
            playerVelocity.Y = -bouncePower;
        }
        if (isStomping) { currentspecialState = specialState.none; isStomping = false; }
        isJumping = true;
        isBouncing = true;
    }

    private void OnPlayerOriginRepositioned()
    {
        levelOriginPosition = GlobalPosition;
    }

    // private void OnPlayerBouncedHorizontally(Vector2 bouncerPos, float bouncePower)
    // {
    //     GD.Print("Player bounced horizontally");
    //     if (GlobalPosition.X > bouncerPos.X)
    //     {
    //         playerVelocity.X = bouncePower;
    //     }
    //     else
    //     {
    //         playerVelocity.X = -bouncePower;
    //     }

    // }

    ///////////////////////////////////////////////////// CALL METHOD TRACKS ///////////////////////////////////////////////////////////////////

    private void OnSpecialAnimationFinished()
    {
        currentspecialState = specialState.none;
        specialBufferTimer.Start();
        //if (isCharging || isChargeLanding) { DetectBreakableTiles(); }
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
            isJumping = false;
            //isLanding = false;
            //isFalling = false;
            isJumpReset = true;
            //if(isLanding){recoverTimer.Start();}
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
    private void OnDigestAnimationFinished()
    {
        isDigesting = false;
    }

    //////////////////////////////////////////////////////////   DETECTORS    ////////////////////////////////////////////////////
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
        if (body.IsInGroup("Edible") && !body.GetNode<Edible>(".").GetEatenProperty() && currentEatingState != eatingState.full)
        {
            Edible myEdible = body.GetNode<Edible>(".");
            edibleList.Add(myEdible);
            canEat = true;


        }
        if (body.IsInGroup("EdibleEnemy") && !body.GetNode<Edible>("Edible").GetEatenProperty() && currentEatingState != eatingState.full)
        {
            Edible myEdible = body.GetNode<Edible>("Edible");
            edibleList.Add(myEdible);
            canEat = true;

        }


    }

    private void OnEdibleDetectorBodyExited(Node2D body)
    {
        if (body.IsInGroup("Edible") && !body.GetNode<Edible>(".").GetEatenProperty())
        {
            Edible myEdible = body.GetNode<Edible>(".");
            edibleList.Remove(myEdible);

        }
        if (body.IsInGroup("EdibleEnemy") && !body.GetNode<Edible>(".").GetEatenProperty())
        {
            Edible myEdible = body.GetNode<Edible>("Edible");
            edibleList.Remove(myEdible);
        }


        if (edibleList.Count == 0)
        {
            canEat = false;
        }
    }

    public void SetCurrentBreakableTileLeft(bool isTouchingLeft, BreakableTile tile)
    {
        isTouchingBreakableTileLeft = isTouchingLeft;
        if (isTouchingLeft) { currentBreakableSideTile = tile; }
    }

    public void SetCurrentBreakableTileRight(bool isTouchingRight, BreakableTile tile)
    {
        isTouchingBreakableTileRight = isTouchingRight;
        if (isTouchingRight) { currentBreakableSideTile = tile; }
    }

    public void SetCurrentBreakableTileBottom(bool isTouchingBottom, BreakableTile tile)
    {
        isTouchingBreakableTileBottom = isTouchingBottom;
        if (isTouchingBottom) { currentBreakableTopTile = tile; }
    }

    public void SetCurrentBreakableTileTop(bool isTouchingTop, BreakableTile tile)
    {
        isTouchingBreakableTileTop = isTouchingTop;
        if (isTouchingTop) { currentBreakableBottomTile = tile; }
    }

    public void SetTouchingTreeChargeDetector(bool isTouching, Tree tree)
    {
        isTouchingTreeCharge = isTouching;
        if (isTouching) { currentTree = tree; }
    }

    public void SetTouchingTreeStompDetector(bool isTouching, Tree tree)
    {
        isTouchingTreeStomp = isTouching;
        if (isTouching) { currentTree = tree; }
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

    private void OnDeathTimerTimeout()
    {
        GlobalPosition = levelOriginPosition;
        currentLifeState = lifeState.alive;
        currentspecialState = specialState.none;
        currentMoveState = moveState.idle;
        if(isStomping){isStomping= false;}
    }

    private void OnSpecialBufferTimerTimeout()
    {

    }


    ///////////////////////////////////////////// EXPOSERS ////////////////////////////////////////////

    public specialState GetSpecialState()
    {
        return currentspecialState;
    }
    public bool GetIsPlayerLeft()
    {
        return playerSprite.FlipH;
    }

    public moveState GetMoveState()
    {
        return currentMoveState;
    }

    public lifeState GetLifeState()
    {
        return currentLifeState;
    }

    public bool GetCeilingStatus()
    {
        return IsOnCeiling();
    }

    public Vector2 GetInputDirection()
    {
        return inputDirection;
    }
    public Vector2 GetVelocity()
    {
        return playerVelocity;
    }

    public void SetCurrentWarpTiles(WarpTile currentDoor, WarpTile targetDoor)
    {
        currentWarpTile = currentDoor;
        destinationWarpTile = targetDoor;
    }
}
