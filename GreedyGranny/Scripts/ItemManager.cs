using Godot;
using System;
using System.Collections.Generic;

public partial class ItemManager : Node2D
{
    private Timer delayTimer;
    private List<collectible_item> myItemList, myItemRemovalList;
    private int containerIndex = 0;

    public override void _Ready(){

        delayTimer = GetNode<Timer>("DelayTimer");
        delayTimer.Start();

        myItemList = new List<collectible_item>();  
        myItemRemovalList = new List<collectible_item>(); 
    }
    private void AssembleLists(){

        foreach (Node item in GetChildren()){

         if(item is collectible_item){
            myItemList.Add(item.GetNode<collectible_item>("."));
            

         }else if(item is carryable_item){

         }

        }
    
    }

    public void ReleaseContentsOfContainer(int itemQuantity, collectible_item.ItemType itemType, Vector2 position){

        int counter = 0;

        foreach(collectible_item item in myItemList){

            switch(itemType){

                case collectible_item.ItemType.Coin:

                    if(counter < itemQuantity){

                        item.GlobalPosition = position;
                        item.RevealItem();
                        item.ThrowInRandomDirection();
                        counter++;
                        myItemRemovalList.Add(item);
                    }

                break;
                case collectible_item.ItemType.CoinBag:


                break;
                case collectible_item.ItemType.Key:


                break;
                case collectible_item.ItemType.Soda:


                break;
                case collectible_item.ItemType.Hat:


                break;
            }
            
        }

        foreach(collectible_item item in myItemRemovalList){
            myItemList.Remove(item);
        }
        counter = 0;
    }

    public void AddCollisionForInstantiatedCoins(){


        foreach(collectible_item coin in GetChildren()){

        }
    }

    private void OnDelayTimerTimeout(){
        AssembleLists();
    }

}
