using Godot;
using System;
using Game.Ingredients;
using Godot.Collections;

public partial class Item : Node2D
{
	[Export] private Texture2D _bottomBunImage, _lettuceImage, _pattyImage, _tomatoImage, _onionImage, _picklesImage, _cheeseImage, _sauceImage, _topBunImage,
	_bottomBunShine, _lettuceShine, _pattyShine, _tomatoShine, _onionShine, _picklesShine, _cheeseShine, _sauceShine, _topBunShine;
	[Export] CharacterBody2D _itemBody;
	[Export] AnimationPlayer _itemAnim;
	[Export] private Timer _lifeTimer, _deathTimer, _shineTimer, _shineWaitTimer;
	[Export] Sprite2D _itemSprite, _shineSprite;
	public IngredientType _myIngredient;
	private (int, int) _itemCoords;
	private bool _golden;

	public void SpawnItem(Vector2 spawnPosition)
	{
		_itemBody.GlobalPosition = spawnPosition;
		_itemAnim.Play("Drop");
	}
	public void SetItemCoords((int, int) coords) { _itemCoords = coords; }
	public (int, int) GetItemCoords() { return _itemCoords; }
	public void SetGolden(bool golden)
	{
		_golden = golden;
		if (golden)
		{
			_shineWaitTimer.Start();
		}
	}
	public bool GetGolden() { return _golden; }

	public void SetItemType(IngredientType ingredient)
	{
		switch (ingredient)
		{
			case IngredientType.bottomBun:
				_itemSprite.Texture = _bottomBunImage;
				_shineSprite.Texture = _bottomBunShine;
				break;
			case IngredientType.lettuce:
				_itemSprite.Texture = _lettuceImage;
				_shineSprite.Texture = _lettuceShine;
				break;
			case IngredientType.patty:
				_itemSprite.Texture = _pattyImage;
				_shineSprite.Texture = _pattyShine;
				break;
			case IngredientType.tomato:
				_itemSprite.Texture = _tomatoImage;
				_shineSprite.Texture = _tomatoShine;
				break;
			case IngredientType.onion:
				_itemSprite.Texture = _onionImage;
				_shineSprite.Texture = _onionShine;
				break;
			case IngredientType.cheese:
				_itemSprite.Texture = _cheeseImage;
				_shineSprite.Texture = _cheeseShine;
				break;
			case IngredientType.pickles:
				_itemSprite.Texture = _picklesImage;
				_shineSprite.Texture = _picklesShine;
				break;
			case IngredientType.sauce:
				_itemSprite.Texture = _sauceImage;
				_shineSprite.Texture = _sauceShine;
				break;
			case IngredientType.topBun:
				_itemSprite.Texture = _topBunImage;
				_shineSprite.Texture = _topBunShine;
				break;
		}

		if (_golden) { _itemSprite.Frame = 2; }

		_myIngredient = ingredient;
	}

	public void ItemCollected()
	{
		if ((int)_myIngredient > 8)
		{
			GD.PrintErr($"Your ingredient, {_myIngredient}, is out of range");
		}
		BoardManager.Instance._itemCount --;
		BoardManager.Instance._occupiedSquares.Remove(_itemCoords);
		IngredientInventory.Instance.AddCollectedItemToInventory(_myIngredient, _golden);
		QueueFree();
	}

	public void ItemEaten()
	{
		BoardManager.Instance._occupiedSquares.Remove(_itemCoords);
		BoardManager.Instance._itemCount --;
		QueueFree();
	}

	private void OnLifeTimerTimeout()
	{
		_deathTimer.Start();
		if (_golden) { _itemAnim.Play("GoldenFade"); }
		else { _itemAnim.Play("Fade"); }
	}

	private void OnDeathTimerTimeout()
	{
		ItemEaten();
	}

	private void OnShineWaitTimerTimeout()
	{
		if(_lifeTimer.TimeLeft > .4)
		{
			_itemAnim.Play("Shine");
			_shineWaitTimer.Start();
		}
		
	}

}
