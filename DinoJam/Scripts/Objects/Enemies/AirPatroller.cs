using Godot;
using System;

public partial class AirPatroller : Enemy
{
    
    public override void _Ready()
    {
        base._Ready();
        
    }

        public override void _PhysicsProcess(double delta)
    {
        Velocity = enemyVelocity;
        MoveAndSlide();
    }

    private void DetermineBehavior()
    {
        switch (currentMoveState)
        {
            case enemyMoveState.move:

                break;
            case enemyMoveState.prepare:

                break;
            case enemyMoveState.attack:

                break;
            case enemyMoveState.hurt:

                break;
            case enemyMoveState.dying:

                break;
        }
    }

    private void HandleMovement()
    {
        
    }
}

