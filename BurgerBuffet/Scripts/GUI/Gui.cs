using Game.Ingredients;
using Godot;
using System;

public partial class Gui : Control
{
	[Export] private RichTextLabel _burgersLabel, _burgersInteger, _timeLabe, _timeInteger, _scoreLabel, _scoreInteger, _ordersUpLabel;
	[Export] private Texture2D _bottomBunTexture, _bottomBunImage, _lettuceTexture, _lettuceImage, _pattyTexture, _pattyImage, _cheeseTexture, _cheeseImage, 
	_onionTexture,_onionImage, _tomatoTexture, _tomatoImage, _picklesTexture, _picklesImage,  _sauceTexture,_sauceImage,  _topBunTexture, _topBunImage;
	[Export] private Control _burgerImageMenu;
	[Export] private Sprite2D _burgerIngredientSprite;
	[Export] private AnimationPlayer _burgerAnim;
	[Export]private Timer _wipeDelayTimer;
	//private Sprite2D _thisSprite; 
	private Vector2 _spritePosition, _orderWindowOriginPosition, _burgerImageOriginPosition, _imagePosition;
	private IngredientType _currentType;

	public override void _Ready()
	{
		_orderWindowOriginPosition = new Vector2(480,246);
		_burgerImageOriginPosition = new Vector2(64, 178);
	}

	public void SetUpOrderWindow(Burger order)
	{
		WipeOrderImage();
		_spritePosition = _orderWindowOriginPosition;

		for(int i = 0; i < order.ingredients.Length; i++)
		{
			Sprite2D thisSprite = new Sprite2D();
			AddChild(thisSprite);

			switch (order.ingredients[i])
			{
				case IngredientType.bottomBun:
				thisSprite.Texture = _bottomBunTexture;
				break;
				case IngredientType.lettuce:
				thisSprite.Texture = _lettuceTexture;
				break;
				case IngredientType.patty:
				thisSprite.Texture = _pattyTexture;
				break;
				case IngredientType.cheese:
				thisSprite.Texture = _cheeseTexture;
				break;
				case IngredientType.onion:
				thisSprite.Texture = _onionTexture;
				break;
				case IngredientType.tomato:
				thisSprite.Texture = _tomatoTexture;
				break;
				case IngredientType.pickles:
				thisSprite.Texture = _picklesTexture;
				break;
				case IngredientType.sauce:
				thisSprite.Texture = _sauceTexture;
				break;
				case IngredientType.topBun:
				thisSprite.Texture = _topBunTexture;
				break;
			}
			_currentType = order.ingredients[i];
			thisSprite.ZIndex = 1;
			thisSprite.GlobalPosition = _spritePosition;
			float vertSpace = 100 / order.ingredients.Length;
			_spritePosition.Y -= vertSpace;

		}
	}

	private void WipeOrderImage()
	{
		foreach(Node child in GetChildren())
		{
			if(child is Sprite2D)
			{
				child.QueueFree();
			}
		}
	}

	public void AddIngredientToBurgerImage(IngredientType ingredient)
	{
		//Sprite2D newImage = new Sprite2D();
		//_imagePosition = _burgerImageOriginPosition;
		//_burgerImageMenu.AddChild(newImage);
		GD.Print("Dropping " + ingredient);
		switch (ingredient)
			{
				case IngredientType.bottomBun:
				_burgerIngredientSprite.Texture = _bottomBunImage;
				break;
				case IngredientType.lettuce:
				_burgerIngredientSprite.Texture = _lettuceImage;
				break;
				case IngredientType.patty:
				_burgerIngredientSprite.Texture = _pattyImage;
				break;
				case IngredientType.cheese:
				_burgerIngredientSprite.Texture = _cheeseImage;
				break;
				case IngredientType.onion:
				_burgerIngredientSprite.Texture = _onionImage;
				break;
				case IngredientType.tomato:
				_burgerIngredientSprite.Texture = _tomatoImage;
				break;
				case IngredientType.pickles:
				_burgerIngredientSprite.Texture = _picklesImage;
				break;
				case IngredientType.sauce:
				_burgerIngredientSprite.Texture = _sauceImage;
				break;
				case IngredientType.topBun:
				_burgerIngredientSprite.Texture = _topBunImage;
				break;
			}

			_burgerIngredientSprite.ZIndex = 1;
			_imagePosition = _burgerImageOriginPosition;
			_imagePosition.Y -= 2*(int)ingredient;
			_burgerIngredientSprite.GlobalPosition = _imagePosition;
			_currentType = ingredient;
			_burgerAnim.Play("Drop");
			
			// _imagePosition.Y -= 4;
	}

	public void ReplaceIngredientWithNewSprite()
	{
		Sprite2D newImage = new Sprite2D();
		_burgerImageMenu.AddChild(newImage);

		switch (_currentType)
			{
				case IngredientType.bottomBun:
				newImage.Texture = _bottomBunImage;
				break;
				case IngredientType.lettuce:
				newImage.Texture = _lettuceImage;
				break;
				case IngredientType.patty:
				newImage.Texture = _pattyImage;
				break;
				case IngredientType.cheese:
				newImage.Texture = _cheeseImage;
				break;
				case IngredientType.onion:
				newImage.Texture = _onionImage;
				break;
				case IngredientType.tomato:
				newImage.Texture = _tomatoImage;
				break;
				case IngredientType.pickles:
				newImage.Texture = _picklesImage;
				break;
				case IngredientType.sauce:
				newImage.Texture = _sauceImage;
				break;
				case IngredientType.topBun:
				newImage.Texture = _topBunImage;
				break;
			}

		_imagePosition = _burgerImageOriginPosition;
		_imagePosition.Y = _burgerImageOriginPosition.Y - 2*(int)_currentType;
		newImage.GlobalPosition = _imagePosition;
		GD.Print("Replaced Image Successfully!");
	}

	public void StartBurgerImageWipe()
	{
		_wipeDelayTimer.Start();
	}

	public void WipeBurgerImage()
	{
		foreach(Node child in _burgerImageMenu.GetChildren())
		{
			if(child is Sprite2D && !child.IsInGroup("DoNotDispose"))
			{
				child.QueueFree();
			}
		}
	}

	private void OnWipeDelayTimerTimeout()
	{
		WipeBurgerImage();
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GenerateNewOrder);
	}

}
