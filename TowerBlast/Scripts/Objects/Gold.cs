using Godot;
using System;
using System.Data;

public partial class Gold : Node2D
{
	//[Signal] public delegate void CoinFellEventHandler(Vector2 position, Gold gold);
	[Export] private Texture2D _stackedTexture;
	[Export] private CharacterBody2D _goldBody;
	[Export] private AnimationPlayer _goldAnim;
	[Export] private Sprite2D _goldSprite;
	[Export] private int _gravity, _speed;
	[Export] private Timer _collectTimer, _spinTimer;
	private int _goldQuantity, _randX;
	private float _startVelocity = -80;
	private Vector2 _flightDirection, _coinVelocity;

	public enum _state
	{
		solo, stacked, collecting
	}
	private _state _currentState;

	public override void _Ready()
	{
		//_currentState = _state.solo;
		//_spinTimer.WaitTime = GD.RandRange(1,5);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_currentState == _state.solo)
		{
			_goldAnim.Play("GoldSpin");
		}
		else if (_currentState == _state.collecting)
		{
			_goldAnim.Play("GoldCollect");
		}

		Fly();
		_goldBody.Velocity = _coinVelocity;
		_goldBody.MoveAndSlide();
	}

	private void DetermineBehavior()
	{
		if (_currentState == _state.solo)
		{
			_coinVelocity.Y = _startVelocity;
			_randX = GD.RandRange(-50, 50);
			_spinTimer.WaitTime = GD.RandRange(1,5);
			_goldQuantity = 1;
		}
	}

	public void SetState(_state state)
	{
		_currentState = state;
		DetermineBehavior();
	}
	public _state GetState()
	{
		return _currentState;
	}

	public void AddAGoldPieceToStack()
	{
		_goldQuantity += 1;
		_goldSprite.Frame = _goldQuantity;
	}

	public void BeginNewStack()
	{
		if (_currentState == _state.solo)
		{
			_goldAnim.Stop(true);
			_currentState = _state.stacked;
			_goldSprite.Texture = _stackedTexture;
			_goldSprite.Hframes = 37;
			_goldSprite.Frame = 1;
			_goldSprite.Position = new Vector2(_goldSprite.Position.X, _goldSprite.Position.Y - 4);

			GD.Print("Began New Stack");
		}

	}

	public Vector2 GetCoinPosition()
	{
		return _goldBody.GlobalPosition;
	}
	public void SetCoinPosition(Vector2 position)
	{
		_goldBody.GlobalPosition = position;
	}


	public void SetFlightDirection(Vector2 direction)
	{
		_flightDirection = direction;
	}

	private void CoinFallFlat()
	{
		GlobalSignals.Instance.EmitSignal("CoinFellFlat", _goldBody.GlobalPosition, this);
	}

	private void Fly()
	{
		if (!_goldBody.IsOnFloor())
		{
			_coinVelocity.Y += (.1f) * _gravity;
			_coinVelocity.X = _randX;
		}
		else
		{
			_coinVelocity = Vector2.Zero;
		}
	}

	private void OnCollectionAreaEntered(Node2D body)
	{
		_collectTimer.Start();
		_currentState = _state.collecting;
		MyGlobalResources.Instance.SetPlayerGold(_goldQuantity);
		//_goldAnim.Play("GoldCollect");
	}

	private void OnCollectTimerTimeout()
	{
		QueueFree();
	}

	private void OnSpinTimerTimeout()
	{
		if (_currentState == _state.solo)
		{
			CoinFallFlat();
		}

	}

}
