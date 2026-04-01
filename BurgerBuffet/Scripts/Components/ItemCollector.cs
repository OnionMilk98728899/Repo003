using Godot;
using System;

public partial class ItemCollector : Node2D
{
	[Signal] public delegate void ItemEatenEventHandler();
	[Export] private CollisionShape2D _collectorCollider;
	private Item _myItem;
	private enum unitType
	{
		meanie, player
	}
	[Export] private unitType _myUnitType;
	private void OnItemCollectorBodyEntered(Node2D body)
	{
		if (_myUnitType == unitType.player)
		{
			if (body.IsInGroup("Items"))
			{
				_myItem = body.GetNode<Item>("..");
				_myItem.ItemCollected();
			}

			if (body.IsInGroup("Meanies"))
			{
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GameOver);
			}
		}
		if (_myUnitType == unitType.meanie)
		{
			if (body.IsInGroup("Items"))
			{
				_myItem = body.GetNode<Item>("..");
				_myItem.ItemEaten();
				EmitSignal(SignalName.ItemEaten);
				
			}
		}

	}

	public void DisableEnableCollider(bool isActive)
	{
		_collectorCollider.Disabled = !isActive;
	}
}



