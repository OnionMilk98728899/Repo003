using Godot;
using System;

public partial class PlayerManager : Node2D
{
    public Vector2 playerVelocity, playerPosition;
    private granny myGranny;
    private PlayerMovement pMovement;
    public bool carryTriggered, currentlyCarrying;
    private Label stateLabel;

    public override void _Ready()
    {
        myGranny = GetNode<granny>("../Granny");
        pMovement = GetNode<PlayerMovement>("../Granny/PlayerMovement");
        stateLabel = GetNode<Label>("StateLabel");
    }

    public override void _PhysicsProcess(double delta)
    {   
        playerVelocity = pMovement.myVelocity;
        playerPosition = myGranny.GlobalPosition;

        carryTriggered = pMovement.pressedAction1;
        currentlyCarrying = myGranny.carrying;
        stateLabel.Text = "";
    }
}
