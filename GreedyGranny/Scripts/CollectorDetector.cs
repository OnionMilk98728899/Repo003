using Godot;
using System;

public partial class CollectorDetector : Area2D
{
    [Signal] public delegate void CollectibleEnteredEventHandler();
    [Signal] public delegate void CoinCollectedEventHandler(int coinValue);
    [Signal] public delegate void HatCollectedEventHandler(int hatIndex);

    private collectible_item.ItemType itemType;
    private Headgear.HeadgearType hatType;
    private collectible_item collItem;
    public void OnBodyEntered(PhysicsBody2D body){

        if(body.IsInGroup("Collectable")){

            collItem = body.GetNode<collectible_item>(".");
            if(!collItem.hasBeenCollected){
                collItem.Collect();
                itemType = body.GetNode<collectible_item>(".").thisType;
                hatType = body.GetNode<collectible_item>(".").hatType;
                DetermineItemAction();

            }

        }

    }

    private void DetermineItemAction(){
 
        switch(itemType){
            case collectible_item.ItemType.Coin:
            EmitSignal("CoinCollected", 1);
            break;
            case collectible_item.ItemType.CoinBag:
            break;
            case collectible_item.ItemType.Soda:
            break;
            case collectible_item.ItemType.Key:
            break;
            case collectible_item.ItemType.Hat:
                switch(hatType){
                    case Headgear.HeadgearType.BikeHelm:
                        EmitSignal("HatCollected", 1);
                    break;
                    case Headgear.HeadgearType.Firehat:
                        EmitSignal("HatCollected", 2);
                    break;
                    case Headgear.HeadgearType.Hardhat:
                        EmitSignal("HatCollected", 3);
                    break;
                    case Headgear.HeadgearType.Snorkel:
                        EmitSignal("HatCollected", 4);
                    break;
                    case Headgear.HeadgearType.Sunhat:
                        EmitSignal("HatCollected", 5);
                    break;
                    case Headgear.HeadgearType.Viking:
                        EmitSignal("HatCollected", 6);
                    break;
                    case Headgear.HeadgearType.Visor:
                        EmitSignal("HatCollected", 7);
                    break;
                    case Headgear.HeadgearType.Winterhat:
                        EmitSignal("HatCollected", 8);
                    break;
                }
            break;

        }
    }
}
