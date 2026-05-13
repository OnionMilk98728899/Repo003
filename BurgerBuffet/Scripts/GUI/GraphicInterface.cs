using Game.Ingredients;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class GraphicInterface : Control
{
	[Export] private RichTextLabel _burgersLabel, _burgersInteger, _timeLabel, _timeInteger, _scoreLabel, _scoreInteger, _ordersUpLabel, _gameoverLabel,
	_multiplierLabel;
	[Export]
	private Texture2D _bottomBunTexture, _bottomBunSheet, _lettuceTexture, _lettuceSheet, _pattyTexture, _pattySheet, _cheeseTexture, _cheeseSheet,
	_onionTexture, _onionSheet, _tomatoTexture, _tomatoSheet, _picklesTexture, _picklesSheet, _sauceTexture, _sauceSheet, _topBunTexture, _topBunSheet;
	[Export] private TextureButton _quitButton, _retryButton;
	[Export] private Texture2D _focusedButtonTexture, _pressedButtonTexture, _normalButtonTexture;
	[Export] private Control _burgerImageMenu, _gameOverMenu;
	[Export] private Sprite2D[] _burgerIngredientSprites;
	[Export] private Sprite2D _gameOverBackGround, _flamesSprite;
	[Export] private AnimationPlayer _imageAnim, _burgerAnim1, _burgerAnim2, _burgerAnim3, _burgerAnim4, _multAnim, _flamesAnim, _chefAnim;
	[Export] private Timer _wipeDelayTimer, _burgerCountTimer, _ordersUpTimer, _scoreTimer, _specialTimeDisplayTimer, _pressDelayTimer;
	//[Export] private Label debugLabel;
	
	private TextureButton _selectedButton, _pressedButton;
	private Sprite2D _currentIngredientSprite;
	//public List<IngredientType> _queuedIngredients = new List<IngredientType>();
	private Vector2 _spritePosition, _orderWindowOriginPosition, _burgerImageOriginPosition, _imagePosition, _flamesOriginPosition, _flamesPosition;
	private IngredientType _currentType, _nextType, _nextNextType;
	private Dictionary<IngredientType, AudioStream> _ingredientSounds;
	private int _currentIngredientIndex;
	private bool _isDumpingBurger, _isFocusGrabbed, _isPressed;

	public override void _Ready()
	{
		_burgerIngredientSprites = _burgerImageMenu.GetChildren().OfType<Sprite2D>().ToArray();
		_orderWindowOriginPosition = new Vector2(472, 226);
		_burgerImageOriginPosition = new Vector2(64, 178);
		GlobalSignals.Instance.GameOver += OnGameOver;
		GlobalSignals.Instance.RestartGame += OnRestartGame;
		GlobalSignals.Instance.AddTimeToSpecialTime += AddNewSpecialTime;
		WipeBurgerImage();
		HideRevealButtons(false);
		InitializeBurgerSounds();
		_selectedButton = _retryButton;
		_flamesOriginPosition = _flamesSprite.Position;
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GlobalResources.Instance._currentGameState == GlobalResources.gameState.gameOver)
		{
			HandleButtonInput();
			_selectedButton = GetViewport().GuiGetFocusOwner() as TextureButton;
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
			_burgersLabel.Text = "burger";
			_burgersInteger.Text = GlobalResources.Instance.GetBurgerScore().ToString();
		}
		else
		{
			_burgersLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"burger"}[/rainbow][/shake]";
			_burgersInteger.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{GlobalResources.Instance.GetBurgerScore()}[/rainbow][/shake]";
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
			_ordersUpLabel.Text = "Order's up!";
		}
		else
		{
			_ordersUpLabel.Text = $"[shake rate=20.0 level=5 connected=1][rainbow freq=1.0 sat=0.8 val=0.8 speed=1.0]{"Order's up!"}[/rainbow][/shake]";
		}

		if(GlobalResources.Instance.GetMultiplier() > 1){_multiplierLabel.Text = $"x{GlobalResources.Instance.GetMultiplier()}";}
		else{_multiplierLabel.Text = "";}
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
			float vertSpace = 132 / order.ingredients.Length;
			_spritePosition.Y -= vertSpace;

		}

		_ordersUpTimer.Start();
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

	public async Task AddIngredientToBurgerImage(IngredientType ingredient, bool golden)
	{
		
		if (_isDumpingBurger)
		{
			return;
		}
		
		_currentIngredientIndex++;
		SetSpriteTextureToIngredient(ingredient, _burgerIngredientSprites[_currentIngredientIndex - 1]);
		
		string guild = "";
		if(golden){guild = "Golden";}

		switch (_currentIngredientIndex)
		{
			case 1:
				_burgerAnim1.Play($"Drop{_currentIngredientIndex}{guild}");
				break;
			case 2:
				_burgerAnim2.Play($"Drop{_currentIngredientIndex}{guild}");
				break;
			case 3:
				_burgerAnim3.Play($"Drop{_currentIngredientIndex}{guild}");
				break;
			case 4:
				_burgerAnim4.Play($"Drop{_currentIngredientIndex}{guild}");
				break;
			case 5:
				_burgerAnim1.Play($"Drop{_currentIngredientIndex}{guild}");
				break;
			case 6:
				_burgerAnim2.Play($"Drop{_currentIngredientIndex}{guild}");
				break;
			case 7:
				_burgerAnim3.Play($"Drop{_currentIngredientIndex}{guild}");
				break;
			case 8:
				_burgerAnim4.Play($"Drop{_currentIngredientIndex}{guild}");
				break;
		}


		if (ingredient == OrderManager.Instance.GetCurrentOrder().ingredients[_currentIngredientIndex - 1]) 
		{
			//////////  CORRECT INGREDIENT
			AudioManager.Instance.PlaySFX(AudioManager.Instance._sfxPlayer, AudioManager.Instance._audioLibrary.collect);
			
			if (golden)
			{
				SetBurgerCount(25);
				OrderManager.Instance.IncreaseGoldenCount(1);
				if(GlobalResources.Instance._currentGameState == GlobalResources.gameState.special)
				{
					GlobalResources.Instance.IncreaseMultiplier(OrderManager.Instance.GetGoldenCount() * 1.5f);
				}
				else
				{
					GlobalResources.Instance.IncreaseMultiplier(OrderManager.Instance.GetGoldenCount());
				}
			}
			else
			{
				SetBurgerCount(10);
				if(GlobalResources.Instance._currentGameState == GlobalResources.gameState.special){GlobalResources.Instance.IncreaseMultiplier(.5f);}
			}
			AddBurgerFlamesIfBurgerIsHot(GlobalResources.Instance.GetBurgerScore(), GlobalResources.Instance.GetMultiplier());
		
			if (_currentIngredientIndex == OrderManager.Instance.GetCurrentOrder().ingredients.Length)   
			{
				//////////  COMPLETED BURGER
				OrderManager.Instance.ResetGoldenCount();
				_imageAnim.Play("Clear");
				_currentIngredientIndex = 0;
				_isDumpingBurger = true;
				AudioManager.Instance.PlaySFX(AudioManager.Instance._sfxPlayer, AudioManager.Instance._audioLibrary.collect2);
				GlobalResources.Instance.CountSpecialTimeIncrease(5);
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.InitiateSpecialTime);
				PlayChefsKissIfBurgerIsGreat(GlobalResources.Instance.GetBurgerScore(), GlobalResources.Instance.GetMultiplier());
				CalculateBurgerScore();
				
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GenerateNewOrder);
				

			}

			await PlayIngredientSoundFX(ingredient);
		}
		else            	
		{
			//////// WRONG INGREDIENT
			if (_imageAnim.IsPlaying())
			{
				StartBurgerImageWipe();
			}
			else
			{
				_imageAnim.Play("Cancel");
			}
			
			_currentIngredientIndex = 0;
			_isDumpingBurger = true;
			AudioManager.Instance.PlaySFX(AudioManager.Instance._sfxPlayer, AudioManager.Instance._audioLibrary.badCollect);
			GlobalResources.Instance.ResetIndividualBurgerScore();
			GlobalResources.Instance.AddBurgerStrike();
			AddBurgerFlamesIfBurgerIsHot(0,0);
			if(GlobalResources.Instance.GetBurgerStrikes() >= 3)
			{
				GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GenerateNewOrder);
			}
		}
	}

	private void PlayChefsKissIfBurgerIsGreat(int score, float mult)
	{
		if(score*mult >= 300)
		{
			_chefAnim.Play("ChefKiss");
			AudioManager.Instance.PlaySFX(AudioManager.Instance._burgerFlamesFX, AudioManager.Instance._audioLibrary.chefKiss);
		}
	}

	public void AddBurgerFlamesIfBurgerIsHot(int score, float mult)
	{
		GD.Print("Score is  " + score + "  and mult  = " + mult);
		if(score*mult >= 150)
		{
			_flamesPosition = _flamesOriginPosition;
			_flamesPosition.Y -= _currentIngredientIndex*4;
			_flamesSprite.Position = _flamesPosition;
			_flamesSprite.Visible = true;
			_flamesAnim.Play("Flames");
			AudioManager.Instance.PlaySFX(AudioManager.Instance._burgerFlamesFX, AudioManager.Instance._audioLibrary.burgerFlames);
			
		}
		else
		{
			_flamesAnim.Stop();
			_flamesSprite.Visible = false;
		}
	}

	private void AddNewSpecialTime(double time)
	{
		GlobalResources.Instance.CountSpecialTimeIncrease(time);
		_specialTimeDisplayTimer.Start();
	}

	private void CalculateBurgerScore()
	{
		GlobalResources.Instance.CountScoreIncrease();
		_scoreTimer.Start();
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

	private void InitializeBurgerSounds()
	{
		_ingredientSounds = new Dictionary<IngredientType, AudioStream>()
	{
		{ IngredientType.bottomBun, AudioManager.Instance._audioLibrary.bottomBunDrop},
		{ IngredientType.lettuce, AudioManager.Instance._audioLibrary.lettuceDrop },
		{ IngredientType.patty, AudioManager.Instance._audioLibrary.pattyDrop },
		{ IngredientType.cheese, AudioManager.Instance._audioLibrary.cheeseDrop},
		{ IngredientType.tomato, AudioManager.Instance._audioLibrary.tomatoDrop },
		{ IngredientType.onion, AudioManager.Instance._audioLibrary.onionDrop},
		{ IngredientType.pickles, AudioManager.Instance._audioLibrary.picklesDrop},
		{ IngredientType.sauce, AudioManager.Instance._audioLibrary.sauceDrop},
		{ IngredientType.topBun, AudioManager.Instance._audioLibrary.topBunDrop}
	};
	}


	public async Task PlayIngredientSoundFX(IngredientType ingredient)
	{
		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);

		if (_ingredientSounds.TryGetValue(ingredient, out AudioStream sound))
		{
			AudioManager.Instance.PlaySFX(AudioManager.Instance._burgerImagePlayer, sound);
		}
	}

	public void SetBurgerCount(int count)
	{
		GlobalResources.Instance.CountNewBurgerScore(count);
		_burgerCountTimer.Start();
	}

	public void StartBurgerImageWipe()
	{
		_wipeDelayTimer.Start();
	}
	private void OnGameOver()
	{

		_gameoverLabel.Text = "Game Over";
		_gameOverBackGround.Visible = true;
		
	}

	private void OnRestartGame()
	{
		WipeBurgerImage();
		_currentIngredientIndex = 0;
		_gameoverLabel.Text = "";
		_gameOverBackGround.Visible = false;
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

		AddBurgerFlamesIfBurgerIsHot(0,0);

	}
	private void OnWipeDelayTimerTimeout()
	{
		WipeBurgerImage();
		_isDumpingBurger = false;
		//GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GenerateNewOrder);
		IngredientInventory.Instance.SetCurrentIngredientIndex(0);
	}

	private void OnQuitButtonPressed()
	{
		_pressedButton = _quitButton;
		_pressedButton.TextureFocused = _pressedButtonTexture;

		GetTree().Quit();

	}

	private void OnRetryButtonPressed()
	{

		_pressedButton = _retryButton;
		_pressedButton.TextureFocused = _pressedButtonTexture;

		_pressDelayTimer.Start();

	}

	private void OnPressDelayTimerTimeout()
	{
		//_isPressed = false;

		if (_pressedButton != null)
		{
			_pressedButton.TextureFocused = _focusedButtonTexture;
			_pressedButton = null;
		}

		_isFocusGrabbed = false;
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.RestartGame);
	}
}
