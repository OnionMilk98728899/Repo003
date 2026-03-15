using Godot;
using System;

public partial class PlayerActionController : Node2D
{
    [Export] private CharacterBody2D playerBody;
    private GUI myGUI;
    private bool isAction2, isAction3, isAction4, isAction5, isAction2Held, isCloseToTree;
    public bool standActive;
    public override void _Ready()
    {
        GlobalSignals.Instance.ActivateFruitStand += ActivateFruitStandGUI;
        GlobalSignals.Instance.PlayerCloseToTree += PlayerNearTree;
        myGUI = GetNode<GUI>("/root/TownScene/GUICanvas/GUI");
    }
    public override void _PhysicsProcess(double delta)
    {
        HandleActions();
    }

    private void HandleActions()
    {
        if (Input.IsActionJustPressed("Action2"))
        {
            isAction2 = true;
        }
        else
        {
            isAction2 = false;
        }
        if (Input.IsActionPressed("Action2"))
        {
            isAction2Held = true;
        }
        else
        {
            isAction2Held = false;
        }

        if (Input.IsActionJustPressed("Action3"))
        {
            isAction3 = true;
        }
        else
        {
            isAction3 = false;
        }
        if (Input.IsActionJustPressed("Action4"))
        {
            isAction4 = true;
        }
        else
        {
            isAction4 = false;
        }
        if (Input.IsActionJustPressed("Action5"))
        {
            isAction5 = true;
        }
        else
        {
            isAction5 = false;
        }
    }

    public bool GetAction2Status()
    {
        return isAction2;
    }
    public bool GetAction2HeldStatus()
    {
        return isAction2Held;
    }
    public bool GetAction3Status()
    {
        return isAction3;
    }
    public bool GetAction4Status()
    {
        return isAction4;
    }
    public bool GetAction5Status()
    {
        return isAction5;
    }
    public bool GetCloseToTree()
    {
        return isCloseToTree;
    }

    private void ActivateFruitStandGUI(bool isActive)
    {
        myGUI.ActivateFruitStandInterface(isActive);
        standActive = isActive;
    }

    private void PlayerNearTree(bool isClose)
    {
        isCloseToTree = isClose;
    }
}
