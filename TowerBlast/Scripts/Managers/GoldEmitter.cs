using Godot;
using System;
using System.Collections.Generic;

public partial class GoldEmitter : Node2D
{
	[Export] private Hurtbox _myHurtbox;
	[Export] private PackedScene _goldScene;
	[Export] private int _goldQuantity;
	private Gold _myGold;
	private List<Gold> _myGoldList = new List<Gold>();
	private Vector2 _throwPosition;
	public override void _Ready()
	{
		_myHurtbox.DestroyUnit += OnDestroyUnit;
	}

	private void OnDestroyUnit()
	{
		EmitGold();
	}

	private void AddGoldToParentManager(Gold coin)
	{
		ItemManager.Instance.AddChild(coin);
	}
	
	private void EmitGold()
	{
		for(int i = 0; i < _goldQuantity; i++)
		{
			_throwPosition = GlobalPosition;
			_throwPosition.Y -= 5;
			_myGold = _goldScene.Instantiate<Gold>();
			_myGold.SetFlightDirection(_throwPosition);
			_myGold.GlobalPosition = _throwPosition;
			_myGold.SetState(Gold._state.solo);
			_myGoldList.Add(_myGold);
			ItemManager.Instance.CallDeferred(Node.MethodName.AddChild, _myGold);
		}

		// foreach(Gold gold in _myGoldList)
		// {
		// 	CallDeferred("AddGoldToParentManager", gold);
		// }
	}
}
