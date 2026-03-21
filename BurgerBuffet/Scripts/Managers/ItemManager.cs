using Game.Ingredients;
using Godot;
using Godot.Collections;
using Array = System.Array;
using System;
using System.Linq;

public partial class ItemManager : Node2D
{
	[Export] private Timer _delayTimer;
	[Export] private PackedScene _itemScene;
	private Item _myItem;
	private Burger _myBurger;
	private int FLOORSIZE_X = 16, FLOORSIZE_Y = 14;
	private int _burgerSizeLimit;
	private bool _hasPatty, _validSquare;
	private Vector2 _boardOriginPosition = new Vector2(152, 54), _spawnPosition;
	private (int, int) _boardSquare;

	public override void _Ready()
	{
		GlobalSignals.Instance.GameOver += StartRound;
		GlobalSignals.Instance.GenerateNewOrder += GenerateRandomOrder;
	}

	private void StartRound()
	{
		WipeAllBoardItems();
		_delayTimer.Start();
		_burgerSizeLimit = 5;
		GenerateRandomOrder();
	}

	private void OnDelayTimerTimeout()
	{
		GenerateRandomBoardItem();
		_delayTimer.Start();
	}

	public void WipeAllBoardItems()
	{
		if (GetChildren() != null)
		{
			foreach (Node child in GetChildren())
			{
				if (child is Item)
				{
					child.QueueFree();
				}
			}
		}

	}

	private void FindRandomAvailableBoardSquare()
	{
		int randX = GD.RandRange(0, FLOORSIZE_X - 1);
		int randY = GD.RandRange(0, FLOORSIZE_Y - 1);

		_validSquare = true;
		for (int i = 0; i < ItemInventory.Instance._occupiedSquares.Count; i++)
		{
			if (ItemInventory.Instance._occupiedSquares[i].Item1 == randX && ItemInventory.Instance._occupiedSquares[i].Item2 == randY)
			{
				_validSquare = false;
			}

		}

		if (!_validSquare)
		{
			FindRandomAvailableBoardSquare();
		}
		else
		{
			_boardSquare = (randX, randY);
			ItemInventory.Instance._occupiedSquares.Add(_boardSquare);
		}
	}

	private void GenerateRandomBoardItem()
	{
		if (ItemInventory.Instance._occupiedSquares.Count < 40)
		{
			FindRandomAvailableBoardSquare();

			float bias = 0.85f; // 70% chance to pick from ingredients

			int randItem;
			float biasCheck = GD.Randf();

			if (biasCheck < bias && _myBurger.ingredients != null)
			{
				int index = GD.RandRange(0, _myBurger.ingredients.Length - 1);
				randItem = (int)_myBurger.ingredients[index];
				//GD.Print($"Picked {_myBurger.ingredients[index]} from the Burger order");
			}
			else
			{
				randItem = GD.RandRange(0, 8);
				//GD.Print("Made a purely random ingredient because Burger is null");
			}

			_spawnPosition.X = _boardOriginPosition.X + (_boardSquare.Item1 * 16);
			_spawnPosition.Y = _boardOriginPosition.Y + (_boardSquare.Item2 * 16);

			_myItem = _itemScene.Instantiate<Item>();
			AddChild(_myItem);
			_myItem.SetItemType((IngredientType)randItem);
			_myItem.SetItemCoords(_boardSquare);
			_myItem.SpawnItem(_spawnPosition);
		}
	}

	private void GenerateRandomOrder()
	{
		int burgerSize = GD.RandRange(3, _burgerSizeLimit);

		IngredientType[] newBurger = new IngredientType[burgerSize];

		newBurger[0] = IngredientType.bottomBun;
		newBurger[burgerSize - 1] = IngredientType.topBun;

		if (burgerSize == 3) { newBurger[1] = IngredientType.patty; }

		if (burgerSize > 3)
		{
			while (!_hasPatty)
			{
				for (int i = 1; i < burgerSize - 1; i++)
				{
					int rand = GD.RandRange(1, 7);
					newBurger[i] = (IngredientType)rand;
					if (newBurger[i] == IngredientType.patty) { _hasPatty = true; }
				}
			}
			_hasPatty = false;

			Dictionary<IngredientType, int> compList = new Dictionary<IngredientType, int>();

			for (int i = 1; i < burgerSize - 1; i++)
			{
				IngredientType ingredient = newBurger[i];

				if (!compList.ContainsKey(ingredient)) { compList[ingredient] = 1; }
				else { compList[ingredient]++; }

				if (compList[ingredient] >= 3)
				{
					do
					{
						int rand = GD.RandRange(1, 7);
						newBurger[i] = (IngredientType)rand;
					}
					while (newBurger[i] == ingredient);

					//GD.Print($"Detected a {compList[ingredient]}X of {ingredient} and replaced it with {newBurger[i]}");   
				}
			}
			Array.Sort(newBurger);

		}

		// for (int i = 0; i < burgerSize; i++)
		// {
		// 	GD.Print(newBurger[i]);
		// }

		_myBurger.ingredients = newBurger;
		OrderManager.Instance.SetCurrentOrder(_myBurger);
	}
}
