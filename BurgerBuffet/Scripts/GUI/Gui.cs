using Game.Ingredients;
using Godot;
using System;

public partial class Gui : Control
{
	[Export] private RichTextLabel _burgersLabel, _burgersInteger, _timeLabel, _timeInteger, _scoreLabel, _scoreInteger, _ordersUpLabel;
	[Export]
	private Texture2D _bottomBunTexture, _bottomBunImage, _lettuceTexture, _lettuceImage, _pattyTexture, _pattyImage, _cheeseTexture, _cheeseImage,
	_onionTexture, _onionImage, _tomatoTexture, _tomatoImage, _picklesTexture, _picklesImage, _sauceTexture, _sauceImage, _topBunTexture, _topBunImage;
	[Export] private Control _burgerImageMenu;
	[Export] private Sprite2D _burgerIngredientSprite, _burgerIngredientSprite2;
	[Export] private AnimationPlayer _burgerAnim;
	[Export] private Timer _wipeDelayTimer, _secondDelayTimer, _thirdDelayTimer, _burgerCountTimer, _ordersUpTimer, _scoreTimer, _specialTimeDisplayTimer;
	//private Sprite2D _thisSprite; 
	private Vector2 _spritePosition, _orderWindowOriginPosition, _burgerImageOriginPosition, _imagePosition;
	private IngredientType _currentType, _nextType, _nextNextType;
	private int _burgerCount, _score, _specialTime;

	public override void _Ready()
	{
		_orderWindowOriginPosition = new Vector2(480, 246);
		_burgerImageOriginPosition = new Vector2(64, 178);
		GlobalSignals.Instance.GameOver += WipeBurgerImage;
	}

	public override void _PhysicsProcess(double delta)
	{
		SetLabels();
	}

	private void SetLabels()
	{
		if (_burgerCountTimer.IsStopped())
		{
			_burgersLabel.Text = "burgers";
			_burgersInteger.Text = _burgerCount.ToString();
		}
		else
		{
			_burgersLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"burgers"}[/rainbow][/shake]";
			_burgersInteger.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{_burgerCount}[/rainbow][/shake]";
		}

		if (_scoreTimer.IsStopped())
		{
			_scoreLabel.Text = "score";
			_scoreInteger.Text = _score.ToString();
		}
		else
		{
			_scoreLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"score"}[/rainbow][/shake]";
			_scoreInteger.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{_score}[/rainbow][/shake]";
		}
		if (_specialTimeDisplayTimer.IsStopped())
		{
			_timeLabel.Text = "time";
			_timeInteger.Text = _specialTime.ToString();
		}
		else
		{
			_timeLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"time"}[/rainbow][/shake]";
			_timeInteger.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{_specialTime}[/rainbow][/shake]";
		}

		if (_ordersUpTimer.IsStopped())
		{
			_ordersUpLabel.Text = "order's up!";
		}
		else
		{
			_ordersUpLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"order's up!"}[/rainbow][/shake]";
		}
	}
	public void SetUpOrderWindow(Burger order)
	{
		WipeOrderImage();
		_spritePosition = _orderWindowOriginPosition;

		for (int i = 0; i < order.ingredients.Length; i++)
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
		foreach (Node child in GetChildren())
		{
			if (child is Sprite2D)
			{
				child.QueueFree();
			}
		}
	}

	public void AddIngredientToBurgerImage(IngredientType ingredient)
	{
		SetSpriteTextureToIngredient(ingredient, _burgerIngredientSprite);

		_burgerIngredientSprite.ZIndex = 1;
		_imagePosition = _burgerImageOriginPosition;
		_imagePosition.Y -= 2 * (int)ingredient;
		_burgerIngredientSprite.GlobalPosition = _imagePosition;

		if (!_burgerAnim.IsPlaying())
		{
			if (!_secondDelayTimer.IsStopped())
			{
				_thirdDelayTimer.Start();
				_nextNextType = ingredient;
			}
			else
			{
				_currentType = ingredient;
				_burgerAnim.Play("Drop");
				GD.Print("Dropping " + ingredient);
			}

		}
		else
		{
			_secondDelayTimer.Start();
			_nextType = ingredient;
			GD.Print("Queueing to drop " + _nextType);
		}
	}

	public void ReplaceIngredientWithNewSprite()
	{
		Sprite2D newImage = new Sprite2D();
		_burgerImageMenu.AddChild(newImage);
		GD.Print("replacing Sprite with a " + _currentType);

		SetSpriteTextureToIngredient(_currentType, newImage);

		_imagePosition = _burgerImageOriginPosition;
		_imagePosition.Y = _burgerImageOriginPosition.Y - 3 * IngredientInventory.Instance.GetCurrentIngredientIndex();
		newImage.GlobalPosition = _imagePosition;
	}

	private void SetSpriteTextureToIngredient(IngredientType ingredient, Sprite2D sprite)
	{
		switch (ingredient)
		{
			case IngredientType.bottomBun:
				sprite.Texture = _bottomBunImage;
				break;
			case IngredientType.lettuce:
				sprite.Texture = _lettuceImage;
				break;
			case IngredientType.patty:
				sprite.Texture = _pattyImage;
				break;
			case IngredientType.cheese:
				sprite.Texture = _cheeseImage;
				break;
			case IngredientType.onion:
				sprite.Texture = _onionImage;
				break;
			case IngredientType.tomato:
				sprite.Texture = _tomatoImage;
				break;
			case IngredientType.pickles:
				sprite.Texture = _picklesImage;
				break;
			case IngredientType.sauce:
				sprite.Texture = _sauceImage;
				break;
			case IngredientType.topBun:
				sprite.Texture = _topBunImage;
				break;
		}
	}

	public void IncreaseBurgerCount()
	{
		_burgerCount++;
		_burgerCountTimer.Start();
	}

	public void StartBurgerImageWipe()
	{
		_wipeDelayTimer.Start();
	}

	public void WipeBurgerImage()
	{
		foreach (Node child in _burgerImageMenu.GetChildren())
		{
			if (child is Sprite2D && !child.IsInGroup("DoNotDispose"))
			{
				child.QueueFree();
			}
		}
	}
	private void OnWipeDelayTimerTimeout()
	{
		WipeBurgerImage();
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GenerateNewOrder);
		IngredientInventory.Instance.SetCurrentIngredientIndex(0);
	}
	private void OnSecondDelayTimerTimeout()
	{
		GD.Print("(2) Dropping" + _nextType);
		SetSpriteTextureToIngredient(_nextType, _burgerIngredientSprite2);
		_currentType = _nextType;
		_burgerAnim.Play("Drop2");
	}

	private void OnThirdDelayTimerTimeout()
	{
		AddIngredientToBurgerImage(_nextNextType);
	}
}
