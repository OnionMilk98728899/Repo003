using Game.Ingredients;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GraphicInterface : Control
{
	[Export] private RichTextLabel _burgersLabel, _burgersInteger, _timeLabel, _timeInteger, _scoreLabel, _scoreInteger, _ordersUpLabel, _gameoverLabel, _gameOverLabelBack;
	[Export]
	private Texture2D _bottomBunTexture, _bottomBunSheet, _lettuceTexture, _lettuceSheet, _pattyTexture, _pattySheet, _cheeseTexture, _cheeseSheet,
	_onionTexture, _onionSheet, _tomatoTexture, _tomatoSheet, _picklesTexture, _picklesSheet, _sauceTexture, _sauceSheet, _topBunTexture, _topBunSheet;
	[Export] private TextureButton _quitButton, _retryButton;
	[Export] private Control _burgerImageMenu, _gameOverMenu;
	[Export] private Sprite2D[] _burgerIngredientSprites;
	[Export] private AnimationPlayer _imageAnim, _burgerAnim1, _burgerAnim2, _burgerAnim3, _burgerAnim4;
	[Export] private Timer _wipeDelayTimer, _burgerCountTimer, _ordersUpTimer, _scoreTimer, _specialTimeDisplayTimer;
	[Export] private Label debugLabel;
	private Sprite2D _currentIngredientSprite;
	//public List<IngredientType> _queuedIngredients = new List<IngredientType>();
	private Vector2 _spritePosition, _orderWindowOriginPosition, _burgerImageOriginPosition, _imagePosition;
	private IngredientType _currentType, _nextType, _nextNextType;
	private int  _currentIngredientIndex;
	private bool _isDumpingBurger, _isFocusGrabbed;

	public override void _Ready()
	{
		_burgerIngredientSprites = _burgerImageMenu.GetChildren().OfType<Sprite2D>().ToArray();
		_orderWindowOriginPosition = new Vector2(480, 246);
		_burgerImageOriginPosition = new Vector2(64, 178);
		GlobalSignals.Instance.GameOver += OnGameOver;
		GlobalSignals.Instance.RestartGame += OnRestartGame;
		WipeBurgerImage();
		HideRevealButtons(false);
	}

	public override void _PhysicsProcess(double delta)
	{
		if(GlobalResources.Instance._currentGameState == GlobalResources.gameState.gameOver)
		{
			HandleButtonInput();
			if (!_retryButton.Visible && !_quitButton.Visible)
			{
				HideRevealButtons(true);
			}
		}
		else
		{
			SetLabels();
			if (_retryButton.Visible && _quitButton.Visible)
			{
				HideRevealButtons(false);
			}
		}
		
	}
	private void SetLabels()
	{
		if (_burgerCountTimer.IsStopped())
		{
			_burgersLabel.Text = "burgers";
			_burgersInteger.Text = GlobalResources.Instance.GetBurgerCount().ToString();
		}
		else
		{
			_burgersLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"burgers"}[/rainbow][/shake]";
			_burgersInteger.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{GlobalResources.Instance.GetBurgerCount()}[/rainbow][/shake]";
		}

		if (_scoreTimer.IsStopped())
		{
			_scoreLabel.Text = "score";
			_scoreInteger.Text = GlobalResources.Instance.GetScore().ToString();
		}
		else
		{
			_scoreLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"score"}[/rainbow][/shake]";
			_scoreInteger.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{GlobalResources.Instance.GetScore()}[/rainbow][/shake]";
		}
		if (_specialTimeDisplayTimer.IsStopped())
		{
			_timeLabel.Text = "time";
			_timeInteger.Text = GlobalResources.Instance.GetSpecialTime().ToString();
		}
		else
		{
			_timeLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"time"}[/rainbow][/shake]";
			_timeInteger.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{GlobalResources.Instance.GetSpecialTime()}[/rainbow][/shake]";
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

	private void HandleButtonInput()
	{
		if (!_isFocusGrabbed)
		{
			_retryButton.GrabFocus();
			_isFocusGrabbed = true;
		}
	}

	private void HideRevealButtons(bool hidden)
	{
		_retryButton.Visible = hidden;
		_quitButton.Visible = hidden;
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
			if (child is Sprite2D && !child.IsInGroup("DoNotDispose"))
			{
				child.QueueFree();
			}
		}
	}

	public void AddIngredientToBurgerImage(IngredientType ingredient)
	{
		if (_isDumpingBurger)
		{
			return;
		}
		_currentIngredientIndex++;
		GD.Print($"Sprite is {_burgerIngredientSprites[_currentIngredientIndex - 1].Name}");
		SetSpriteTextureToIngredient(ingredient, _burgerIngredientSprites[_currentIngredientIndex - 1]);

		switch (_currentIngredientIndex)
		{
			case 1:
				_burgerAnim1.Play($"Drop{_currentIngredientIndex}");
				break;
			case 2:
				_burgerAnim2.Play($"Drop{_currentIngredientIndex}");
				break;
			case 3:
				_burgerAnim3.Play($"Drop{_currentIngredientIndex}");
				break;
			case 4:
				_burgerAnim4.Play($"Drop{_currentIngredientIndex}");
				break;
			case 5:
				_burgerAnim1.Play($"Drop{_currentIngredientIndex}");
				break;
			case 6:
				_burgerAnim2.Play($"Drop{_currentIngredientIndex}");
				break;
			case 7:
				_burgerAnim3.Play($"Drop{_currentIngredientIndex}");
				break;
			case 8:
				_burgerAnim4.Play($"Drop{_currentIngredientIndex}");
				break;
		}

		if (ingredient == OrderManager.Instance.GetCurrentOrder().ingredients[_currentIngredientIndex - 1])
		{
			GD.Print("CORRECT INGREDIENT!");
			AudioManager.Instance.PlaySFX(AudioManager.Instance._audioLibrary.collect);
			GlobalResources.Instance.SetScore(10);

			if (_currentIngredientIndex == OrderManager.Instance.GetCurrentOrder().ingredients.Length)
			{
				GD.Print("BURGER COMPLETE!");
				_imageAnim.Play("Clear");
				_currentIngredientIndex = 0;
				_isDumpingBurger = true;
				AudioManager.Instance.PlaySFX(AudioManager.Instance._audioLibrary.collect2);
				GlobalResources.Instance.SetScore(50);
				SetBurgerCount(1);
			}
		}
		else
		{
			_imageAnim.Play("Cancel");
			_currentIngredientIndex = 0;
			_isDumpingBurger = true;
			AudioManager.Instance.PlaySFX(AudioManager.Instance._audioLibrary.badCollect);
			GlobalResources.Instance.SetScore(-10);
		}
	}

	private void SetSpriteTextureToIngredient(IngredientType ingredient, Sprite2D sprite)
	{
		switch (ingredient)
		{
			case IngredientType.bottomBun:
				sprite.Texture = _bottomBunSheet;
				break;
			case IngredientType.lettuce:
				sprite.Texture = _lettuceSheet;
				break;
			case IngredientType.patty:
				sprite.Texture = _pattySheet;
				break;
			case IngredientType.cheese:
				sprite.Texture = _cheeseSheet;
				break;
			case IngredientType.onion:
				sprite.Texture = _onionSheet;
				break;
			case IngredientType.tomato:
				sprite.Texture = _tomatoSheet;
				break;
			case IngredientType.pickles:
				sprite.Texture = _picklesSheet;
				break;
			case IngredientType.sauce:
				sprite.Texture = _sauceSheet;
				break;
			case IngredientType.topBun:
				sprite.Texture = _topBunSheet;
				break;
		}
	}

	public void SetBurgerCount(int count)
	{
		GlobalResources.Instance.SetBurgerCount(count);
		_burgerCountTimer.Start();
	}

	public void StartBurgerImageWipe()
	{
		_wipeDelayTimer.Start();
	}
	private void OnGameOver()
	{

		_gameoverLabel.Text = "game over";
		_gameOverLabelBack.Text = "game over";
	}

	private void OnRestartGame()
	{
		WipeBurgerImage();
		_currentIngredientIndex = 0;
		_gameoverLabel.Text = "";
		_gameOverLabelBack.Text = "";
	}

	public void WipeBurgerImage()
	{
		foreach (Node child in _burgerImageMenu.GetChildren())
		{
			if (child is Sprite2D sprite)
			{
				sprite.Frame = 0;
			}
		}

	}
	private void OnWipeDelayTimerTimeout()
	{
		WipeBurgerImage();
		_isDumpingBurger = false;
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GenerateNewOrder);
		IngredientInventory.Instance.SetCurrentIngredientIndex(0);
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

	private void OnRetryButtonPressed()
	{
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.RestartGame);
		_isFocusGrabbed = false;
		
	}
}
