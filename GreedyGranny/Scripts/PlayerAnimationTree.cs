using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerAnimationTree : AnimationTree
{
    private granny myGranny;

    // private bool Idle, Walk, Run, Jump, Fall, Land, Climb, CarryIdle, CarryWalk, CarryRun, CarryJump, CarryFall, CarryLand,
    //             Crouch, CrouchWalk, Swim, SwimIdle, Charge, Hurt, Interact, Stomp, BladeCharge, LaserIdle, LaserWalk, 
    //             ThrowIdle, ThrowWalk, Float, Dead;


    private int selectedActionIndex;
    private List<string> actionList;
    private granny.State playerState;

    //public float idleCounter;

    public override void _Ready(){

        myGranny = GetNode<granny>("..");

        actionList = new List<string>() {"Idle", "Walk", "Run", "Jump", "Fall", "Land", "Climb", "ClimbIdle", "CarryIdle", "CarryRun", "CarryJump", "CarryFall", "CarryLand",
                "Crouch", "CrouchWalk", "Swim", "SwimIdle", "Charge", "Hurt", "Interact", "Stomp", "RollerCharge", "LaserIdle", "LaserWalk", "Throw", "Float", "Dead", "Sitting", 
                "Knitting", "Hang", "SwimCharge", "StompBounce"};
    }

    public override void _PhysicsProcess(double delta)
    {
        playerState = myGranny.currentState;
        EnableDisableIdleStates();
    }

    public void AnimateGranny(){
        switch(playerState){

            case granny.State.Idle:
            selectedActionIndex = 0;
            break;
            case granny.State.Walk:
            selectedActionIndex = 1;
            break;
            case granny.State.Run:
            selectedActionIndex = 2;
            break;
            case granny.State.Jump:
            selectedActionIndex = 3;
            break;
            case granny.State.Fall:
            selectedActionIndex = 4;
            break;
            case granny.State.Land:
            selectedActionIndex = 5;
            break;
            case granny.State.Climb:
            selectedActionIndex = 6;
            break;
            case granny.State.ClimbIdle:
            selectedActionIndex = 7;
            break;
            case granny.State.CarryIdle:
            selectedActionIndex = 8;
            break;
            case granny.State.CarryRun:
            selectedActionIndex = 9;
            break;
            case granny.State.CarryJump:
            selectedActionIndex = 10;
            break;
            case granny.State.CarryFall:
            selectedActionIndex = 11;
            break;
            case granny.State.CarryLand:
            selectedActionIndex = 12;
            break;
            case granny.State.Crouch:
            selectedActionIndex = 13;
            break;
            case granny.State.CrouchWalk:
            selectedActionIndex = 14;
            break;
            case granny.State.Swim:
            selectedActionIndex = 15;
            break;
            case granny.State.SwimIdle:
            selectedActionIndex = 16;
            break;
            case granny.State.Charge:
            selectedActionIndex = 17;
            break;
            case granny.State.Hurt:
            selectedActionIndex = 18;
            break;
            case granny.State.Interact:
            selectedActionIndex = 19;
            break;
            case granny.State.Stomp:
            selectedActionIndex = 20;
            break;
            case granny.State.RollerCharge:
            selectedActionIndex = 21;
            break;
            case granny.State.LaserIdle:
            selectedActionIndex = 22;
            break;
            case granny.State.LaserWalk:
            selectedActionIndex = 23;
            break;
            case granny.State.Throw:
            selectedActionIndex = 24;
            break;
            case granny.State.Float:
            selectedActionIndex = 25;
            break;
            case granny.State.Dead:
            selectedActionIndex = 26;
            break;
            case granny.State.Sitting:
            selectedActionIndex = 27;
            break;
            case granny.State.Knitting:
            selectedActionIndex = 28;
            break;
            case granny.State.Hang:
            selectedActionIndex = 29;
            break;
            case granny.State.SwimCharge:
            selectedActionIndex = 30;
            break;
            case granny.State.StompBounce:
            selectedActionIndex = 31;
            break;

        }

        SelectCurrentAction();
    }

    private void SelectCurrentAction(){

        for(int i = 0; i < actionList.Count; i++){
            if(i == selectedActionIndex){
                Set($"parameters/conditions/{actionList[i]}", true);

            }else{
                Set($"parameters/conditions/{actionList[i]}", false);
            }

        }
    }

    private void EnableDisableIdleStates(){

        if(playerState == granny.State.Idle || playerState == granny.State.Sitting || playerState == granny.State.Knitting){
            myGranny.isIdle = true;
        }else{
            myGranny.isIdle = false;
        }
    }
}
