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
	private int _burgerSizeLimit;
	private bool _hasPatty;
	private Vector2 _BOARD_ORIGIN_POSITION = new Vector2(152, 54), _spawnPosition;
	private (int,int) _boardSquare;

	public override void _Ready()
	{
		GlobalSignals.Instance.GameOver += OnGameOver;
		GlobalSignals.Instance.RestartGame += StartRound;
		GlobalSignals.Instance.GenerateNewOrder += GenerateRandomOrder;
		GlobalSignals.Instance.SceneReady += OnSceneReady;
		//StartRound();
	}

	
	private void OnSceneReady(Node scene)
	{
		StartRound();
	}
	private void OnGameOver()
	{
		_delayTimer.Stop();
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

	private void GenerateRandomBoardItem()
	{
		if (BoardManager.Instance._itemCount < 25)
		{
			_boardSquare = BoardManager.Instance.OccupyRandomAvailableBoardSquare(false);

			float bias = .95f;

			int randItem;
			float biasCheck = GD.Randf();

			if (biasCheck < bias && _myBurger.ingredients != null)
			{
				int index = GD.RandRange(0, _myBurger.ingredients.Length - 1);
				randItem = (int)_myBurger.ingredients[index];
			}
			else
			{
				randItem = GD.RandRange(0, 8);
			}

			_spawnPosition.X = _BOARD_ORIGIN_POSITION.X + (_boardSquare.Item1 * 16);
			_spawnPosition.Y = _BOARD_ORIGIN_POSITION.Y + (_boardSquare.Item2 * 16);
			BoardManager.Instance._itemCount++;

			_myItem = _itemScene.Instantiate<Item>();
			AddChild(_myItem);
			CalculateGoldenChance(_myItem);
			_myItem.SetItemType((IngredientType)randItem);
			_myItem.SetItemCoords(_boardSquare);
			_myItem.SpawnItem(_spawnPosition);
			
		}
	}

	private void CalculateGoldenChance(Item item)
	{
		float probability = 1- (.01f * GlobalResources.Instance.GetGamePhase());
		float luck = GD.Randf();

		if(luck > probability)
		{
			item.SetGolden(true);
		}
	}

	private void GenerateRandomOrder()
	{
		int burgerSize = GD.RandRange(3, Mathf.Clamp(_burgerSizeLimit + GlobalResources.Instance.GetGamePhase(), 5, 8));

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

				}
			}
			Array.Sort(newBurger);

		}

		_myBurger.ingredients = newBurger;
		OrderManager.Instance.SetCurrentOrder(_myBurger);
	}
}
