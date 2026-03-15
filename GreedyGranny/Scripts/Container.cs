using Godot;
using System;
using System.Collections.Generic;

public partial class Container : Node2D
{   
	private ItemManager itemMan;
	public collectible_item.ItemType containedItem;
	public int itemQuantity;
	[Export] private PackedScene itemToGenerate;
	private collectible_item collectItem;
	private List<collectible_item> itemsList;
	private carryable_item carryableItem;

	public override void _Ready()
	{

		//itemMan = GetNode<ItemManager>(itemManNodePath);
		carryableItem = GetNode<carryable_item>("..");
		itemMan = GetNode<ItemManager>($"../{carryableItem.itemManNodePath}");
		
		// containedItem = carryableItem.containedItem;
		// itemQuantity = carryableItem.containedItemQuan;
		GetContainerContentsFromParent();

		//itemsList = new List<collectible_item>();

		GenerateContents();
	}

	public override void _PhysicsProcess(double delta)
	{

	}
	private void AddContentsToManager(collectible_item thisItem){

		itemMan.AddChild(thisItem);
		thisItem.HideItem();
	}

	private void DetermineItemType(){

	}

	public void ReleaseContents(){

	}

	private void GetContainerContentsFromParent(){

		if(carryableItem.IsInGroup("EnemyBody")){
			
			basic_enemy basicEn =  GetNode<basic_enemy>("../..");
			containedItem = collectible_item.ItemType.Coin;
			itemQuantity = basicEn.coinQuantity;

		}else{

			containedItem = carryableItem.containedItem;
			itemQuantity = carryableItem.containedItemQuan;

		}
	}

	private void GenerateContents(){

		switch(containedItem){

			case collectible_item.ItemType.Coin:

				for(int i = 0; i < itemQuantity; i++){
					
					collectible_item item = itemToGenerate.Instantiate<collectible_item>();

					//item.GlobalPosition = new Vector2 (GlobalPosition.X + 15, GlobalPosition.Y - 15); 
					
					CallDeferred("AddContentsToManager", item);

				}

			break;
		}

	}
}
