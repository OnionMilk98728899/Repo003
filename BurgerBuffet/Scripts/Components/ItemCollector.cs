using Godot;
using System;

public partial class ItemCollector : Node2D
{
	private Item _myItem;
	private void OnItemCollectorBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Items"))
		{
			_myItem = body.GetNode<Item>("..");
			_myItem.ItemCollected();
		}
	}
}



