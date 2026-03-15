using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FruitStand : Node2D
{
    [Export] private Sprite2D standSprite;
    [Export] private int visibility, lineLength;
    [Export] CharacterBody2D standBody;    private CharacterBody2D shadow;
    [Export] private Label debugLabel;
    private MonsterMovement2 monsterMove;
    private PlayerActionController playerAction;
    private bool isPlayerNear, isPlayerInteracting;
    public int customersInLine, customerLimit = 6;
    private Vector2 newLineSpot;
    private List<Vector2> lineList; 

    public override void _Ready()
    {
        lineList = new List<Vector2>(); 
        shadow = GetNode<CharacterBody2D>("../PlayerPackage/PlayerCharacter/ShadowBody");
        //SetUpLineList();
    }

    public override void _PhysicsProcess(double delta)
    {
        AdjustZSorting();
        DetectPlayerAction();
        //debugLabel.Text = customersInLine.ToString();
    }

    private void AdjustZSorting()
    {
        if (shadow.GlobalPosition.Y > GlobalPosition.Y - 8)  /// player Shadow is in front of stand position
        {
            ZIndex = 1;
        }
        else
        {
            ZIndex = 3;
        }
    }

    public int GetStandVisibility()
    {
        return visibility;
    }


    private void DetectPlayerAction()
    {
        if (playerAction != null && isPlayerNear)
        {
            if (playerAction.GetAction2Status())
            {
                if (!isPlayerInteracting)
                {
                    isPlayerInteracting = true;
                    GlobalSignals.Instance.EmitActivateFruitStand(isPlayerInteracting);
                }
                // else
                // {
                //     isPlayerInteracting = false;
                // }

                
            }
            if (playerAction.GetAction5Status())
            {
                if (isPlayerInteracting)
                {
                    isPlayerInteracting = false;
                    GlobalSignals.Instance.EmitActivateFruitStand(isPlayerInteracting);
                }
                
            }
        }
    }

    private void OnStandAreaBodyEntered(Node2D body)
    {
        if (body is CharacterBody2D character)
        {
            if (character.GetNode<CharacterBody2D>(".").IsInGroup("Player"))
            {
                playerAction = body.GetNode<PlayerActionController>("../PlayerActionController");
                isPlayerNear = true;
            }
            if (character.GetNode<CharacterBody2D>(".").IsInGroup("Monster"))
            {
                monsterMove = character.GetNode<MonsterMovement2>("../MonsterMovement");
                // if (monsterMove.GetSeekingStatus())
                // {
                    
                //     GD.Print("New Custy! " + customersInLine + "th customer!");
                // }
            }
        }
    }

    private void OnStandAreaBodyExited(Node2D body)
    {
        if (body is CharacterBody2D character)
        {
            if (character.GetNode<CharacterBody2D>(".").IsInGroup("Player"))
            {
                isPlayerNear = false;
            }
        }
    }

    public Vector2 GetPlaceInLine()
    {
        float xPos = standBody.GlobalPosition.X;
        float yPos = standBody.GlobalPosition.Y;

        yPos += 10 * customersInLine;
        newLineSpot.X = xPos;
        newLineSpot.Y = yPos;
        lineList.Add(newLineSpot);
        GD.Print("COordinates: " + newLineSpot + ", custies: " + customersInLine);
        return newLineSpot;
    }
    // private void SetUpLineList()
    // {
    //     float xPos = standBody.GlobalPosition.X;
    //     float yPos = standBody.GlobalPosition.Y;

    //     for (int i = 0; i < lineLength; i++)
    //     {
    //         yPos += 10;
    //         Vector2 newSpot = new Vector2(xPos, yPos);
    //         lineList.Add(newSpot);
    //     }

    //     foreach (Vector2 pos in lineList)
    //     {
    //         //GD.Print($"Spots at {pos}");
    //     }
    // }

    public void IncreaseCustomerIndex()
    {
        customersInLine++;
    }

    public Vector2 GetNextSpotInLine(int lineIndex)
    {
        return lineList[lineIndex];
    }
}
