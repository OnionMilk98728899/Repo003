using Godot;
using System;
using Game.Ingredients;
using Godot.Collections;

public partial class Item : Node2D
{
	[Export] private Texture2D _bottomBunImage, _lettuceImage, _pattyImage, _tomatoImage, _onionImage, _picklesImage, _cheeseImage, _sauceImage, _topBunImage;
	[Export] CharacterBody2D _itemBody;
	[Export] AnimationPlayer _itemAnim;
	[Export] Sprite2D _itemSprite;
	public IngredientType _myIngredient;
	private(int,int) _itemCoords;

	public void SpawnItem(Vector2 spawnPosition)
	{
		_itemBody.GlobalPosition = spawnPosition;
		_itemAnim.Play("Drop");
	}
	public void SetItemCoords((int,int) coords)
	{
		_itemCoords = coords;
	}
	public (int,int) GetItemCoords()
	{
		return _itemCoords;
	}

	public void SetItemType(IngredientType ingredient)
	{
		switch (ingredient)
		{
			case IngredientType.bottomBun:
			_itemSprite.Texture = _bottomBunImage;
			break;
			case IngredientType.lettuce:
			_itemSprite.Texture = _lettuceImage; 
			break;
			case IngredientType.patty:
			_itemSprite.Texture = _pattyImage;
			break;
			case IngredientType.tomato:
			_itemSprite.Texture = _tomatoImage;
			break;
			case IngredientType.onion:
			_itemSprite.Texture = _onionImage;
			break;
			case IngredientType.cheese:
			_itemSprite.Texture = _cheeseImage;
			break;
			case IngredientType.pickles:
			_itemSprite.Texture = _picklesImage;
			break;
			case IngredientType.sauce:
			_itemSprite.Texture = _sauceImage;
			break;
			case IngredientType.topBun:
			_itemSprite.Texture = _topBunImage;
			break;
		}
		_myIngredient = ingredient;
	}

	public void ItemCollected()
	{
		if((int)_myIngredient > 8)
		{
			GD.PrintErr($"Your ingredient, {_myIngredient}, is out of range");
		}
		IngredientInventory.Instance.AddCollectedItemToInventory(_myIngredient);
		ItemInventory.Instance._occupiedSquares.Remove(_itemCoords);
		QueueFree();
	}

}
