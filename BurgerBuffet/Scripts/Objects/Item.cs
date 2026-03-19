using Godot;
using System;
using Game.Ingredients;

public partial class Item : Node2D
{
	[Export] private Texture2D _bottomBunImage, _lettuceImage, _pattyImage, _tomatoImage, _onionImage, _picklesImage, _cheeseImage, _sauceImage, _topBunImage;
	[Export] CharacterBody2D _itemBody;
	[Export] AnimationPlayer _itemAnim;
	[Export] Sprite2D _itemSprite;
	public IngredientType _myIngredient;

	public void SpawnItem(Vector2 spawnPosition)
	{
		_itemBody.GlobalPosition = spawnPosition;
		_itemAnim.Play("Drop");
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
		IngredientInventory.Instance.AddCollectedItemToInventory(_myIngredient);
		QueueFree();
	}

}
