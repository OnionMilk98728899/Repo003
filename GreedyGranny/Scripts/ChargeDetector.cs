using Godot;
using System;

public partial class ChargeDetector : Area2D
{

    [Signal] public delegate void ChargedIntoEnemyEventHandler();
    [Signal] public delegate void ChargedIntoBreakableEventHandler();

    private void OnBodyEntered(PhysicsBody2D body){

        if(body.IsInGroup("Enemy")){

        }else if(body.IsInGroup("Breakable")){

        }else{
            return;
        }

    }
}
