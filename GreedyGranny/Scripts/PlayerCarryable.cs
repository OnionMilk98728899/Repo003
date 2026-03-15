using Godot;
using System;

public partial class PlayerCarryable : Node2D
{
    private Sprite2D mySprite;
    [Export] private Texture2D boxSprite, grubSprite, frozenGrubSprite;
    public bool carrying, frozen;
    public enum CarryableType{
        Box,
        Grub
    }
    public CarryableType currentType;
    public override void _Ready(){

        mySprite = GetNode<Sprite2D>("CarryableSprite"); 

    }

    public override void _PhysicsProcess(double delta){

    }

    public void DetermineCarryableType(CarryableType type, bool frozen){
        switch(type){
            case CarryableType.Box:
            mySprite.Texture = boxSprite;
            break;
            case CarryableType.Grub:
            if(!frozen){mySprite.Texture = grubSprite;}
            else{mySprite.Texture = frozenGrubSprite;}
            break;
        }
    }


    public void ActivateCarryable(){

        mySprite.Visible = true;
        carrying = true;
        
    }

    public void DeactivateCarryable(){
        mySprite.Visible = false;
        carrying = false;
    }

    public void ThrowCarryable(){
        
    }
}
