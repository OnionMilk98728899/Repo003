using Godot;
using System;

public partial class ItemCollector : Node2D
{
	[Signal] public delegate void ItemEatenEventHandler();
	[Export] private CollisionShape2D _collectorCollider;
	private Item _myItem;
	private bool _isJumping;
	private enum unitType
	{
		meanie, player
	}
	[Export] private unitType _myUnitType;
	private void OnItemCollectorBodyEntered(Node2D body)
	{
		if (_myUnitType == unitType.player)
		{
			if (body.IsInGroup("Items") && !_isJumping)
			{
				_myItem = body.GetNode<Item>("..");
				_myItem.ItemCollected();
			}

			if (body.IsInGroup("Meanies"))
			{
				if(body.GetNode<EnemyMovement>("..")._currentState == EnemyMovement.state.mean)
				{
					if (_isJumping)
					{
						body.GetNode<EnemyMovement>("..").TurnNice(0);
						AudioManager.Instance.PlaySFX(AudioManager.Instance._audioLibrary.enemyConverted);
					}
					else
					{
						GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GameOver);
					}
					
				}
				else
				{
					body.GetNode<EnemyMovement>("..").KillMeanie();
				}
				
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
		_isJumping = !isActive;
	}
}



