using Godot;
using System;

public partial class MainCamera : Camera2D
{
    private CharacterBody2D playerCharacter;
    private Vector2 originalPosition, followPosition, targetPosition, standPosition = new Vector2(0, 45);
    [Export] private float followSpeed, verticalTransitionSpeed;
    private bool standActive, focusedOnStand;

    public override void _Ready()
    {
        playerCharacter = GetNode<CharacterBody2D>("../PlayerCharacter/PlayerBody");
        GlobalSignals.Instance.ActivateFruitStand += SetStandActive;
        originalPosition = GlobalPosition;
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!standActive)
        {
            FollowPlayer(delta);
            if (Mathf.Abs(GlobalPosition.Y - originalPosition.Y) > 1)
            {
                RefocusFromStandToPlayer(standPosition, delta);
            }
        }
        else
        {
            FocusOnStand(standPosition, delta);
        }

    }

    private void FollowPlayer(double delta)
    {
        followPosition = GlobalPosition.Lerp(playerCharacter.GlobalPosition, (float)delta * followSpeed);
        followPosition.Y = GlobalPosition.Y;
        GlobalPosition = followPosition;
    }

    private void FocusOnStand(Vector2 offset, double delta)
    {
        targetPosition = playerCharacter.GlobalPosition + offset;
        followPosition = GlobalPosition.Lerp(targetPosition, (float)delta * verticalTransitionSpeed);
        GlobalPosition = followPosition;
        //focusedOnStand = true;
    }

    private void RefocusFromStandToPlayer(Vector2 offset, double delta)
    {
        targetPosition = playerCharacter.GlobalPosition - offset;
        followPosition = GlobalPosition.Lerp(targetPosition, (float)delta * verticalTransitionSpeed);
        GlobalPosition = followPosition;
    }

    private void SetStandActive(bool isActive)
    {
        standActive = isActive;
    }

    public Vector2 GetCameraPosition()
    {
        return GlobalPosition;
    }

}
