using Godot;
using System;

public partial class EnemyMovement : Node2D
{
	[Export] private CharacterBody2D _enemyBody;
	[Export] private Timer _moveDecisionTimer, _moveCountDownTimer, _eatTimer;
	[Export] private AnimationPlayer _enemyAnim, _arrowAnim;
	[Export]private Sprite2D _arrowSprite;
	[Export] private int SPEED;
	[Export]private Label debugLabel;

	private Vector2 _currentPosition, _targetPosition = Vector2.Zero, _enemyVelocity, _enemyPosition, _BOARD_ORIGIN_POSITION = new Vector2(152, 54);
	private (int,int) _currentSquare, _targetSquare;
	private string _arrowAnimPath;
	public enum state
	{
		mean, nice, destroying
	}
	public enum moveState
	{
		preparing, moving, idle, eating
	}
	public state _currentState;
	public moveState _currentMoveState;

	public override void _Ready()
	{
		if (GlobalResources.Instance._currentGameState == GlobalResources.gameState.special)
		{
			_currentState = state.nice;
		}
		else
		{
			_currentState = state.mean;
		}
		_currentMoveState = moveState.idle;
		_moveDecisionTimer.Start();
	}

	public override void _PhysicsProcess(double delta)
	{
		AnimateEnemy();
		MoveEnemy(delta);
			
		if(_currentMoveState == moveState.preparing || _currentMoveState == moveState.moving)
		{
			// if((_targetPosition.X - _BOARD_ORIGIN_POSITION.X) % 16 != 0 ||(_targetPosition.Y -_BOARD_ORIGIN_POSITION.Y)% 16 != 0)
			// {
			// 	debugLabel.Text = _targetPosition.X + " " + _targetPosition.Y;
			// }
			// if((_enemyBody.GlobalPosition.X - _BOARD_ORIGIN_POSITION.X) % 16 != 0 ||(_enemyBody.GlobalPosition.Y -_BOARD_ORIGIN_POSITION.Y)% 16 != 0)
			// {
			// 		debugLabel.Text = _targetPosition.X + ":::" + _targetPosition.Y;
			// }

		}
	}

	private void DetermineMoveDecision()
	{
		float bias = .75f;
		float rand = GD.Randf();
		if(rand <= bias) 
		{
			_targetSquare = BoardManager.Instance.FindAvailableSquareForMeanie(_currentSquare);
			_targetPosition = new Vector2(_BOARD_ORIGIN_POSITION.X + (_targetSquare.Item1 * 16), _BOARD_ORIGIN_POSITION.Y + (_targetSquare.Item2 * 16));
			DetermineArrowDirection();
			_moveCountDownTimer.Start();
			_moveDecisionTimer.Stop();
			_currentMoveState = moveState.preparing;
		}
		else
		{
			_moveDecisionTimer.Start();
		}
	}

	private void DetermineArrowDirection()
	{
		(int,int) difference = (_targetSquare.Item1 - _currentSquare.Item1, _targetSquare.Item2 - _currentSquare.Item2);

		if(difference.Item1 < 0)
		{
			_arrowAnimPath = "Arrow_Left";
		}
		if(difference.Item1 > 0)
		{
			_arrowAnimPath = "Arrow_Right";
		}
		if(difference.Item2 < 0)
		{
			_arrowAnimPath = "Arrow_Up";
		}
		if(difference.Item2 > 0)
		{
			_arrowAnimPath = "Arrow_Down";
		}
	}

	private void MoveEnemy(double delta)
	{
		if(_currentMoveState == moveState.idle)
		{
			_enemyVelocity = Vector2.Zero;
			//SnapEnemyToGrid(_targetPosition);
		}
		if(_currentMoveState == moveState.preparing)
		{
			_enemyVelocity = Vector2.Zero;
		}
		if (_currentMoveState == moveState.moving|| _currentMoveState == moveState.eating )
		{
			Vector2 direction = _targetPosition - _enemyBody.GlobalPosition;
			_enemyVelocity += direction * SPEED/100;

			if(Mathf.Abs(_enemyBody.GlobalPosition.X  - _targetPosition.X) <= 1 && Mathf.Abs(_enemyBody.GlobalPosition.Y - _targetPosition.Y) <= 1)
			{
				//SnapEnemyToGrid(_targetPosition);
				_currentMoveState = moveState.idle;
				_moveDecisionTimer.Start();
				BoardManager.Instance._occupiedMeanieSquares.Remove(_currentSquare);
				_currentSquare = _targetSquare;
				BoardManager.Instance._occupiedMeanieSquares.Add(_targetSquare);
				
			}
		}

		_enemyBody.Velocity = _enemyVelocity;
		_enemyBody.MoveAndSlide();
	}

	// private void SnapEnemyToGrid(Vector2 target)
	// {
	// 	if(target != _enemyBody.GlobalPosition && target != Vector2.Zero)
	// 	{
	// 		_enemyBody.GlobalPosition = target;
	// 	}
	// }

	private void AnimateEnemy()
	{
		if (_currentState == state.nice)
		{
			_enemyAnim.Play("Nice");
		}
		else if (_currentState == state.mean)
		{
			_enemyAnim.Play("Mean");
		}
		else if (_currentState == state.destroying)
		{
			_enemyAnim.Play("Destroying");
		}

		if(_currentMoveState == moveState.preparing)
		{
			_arrowAnim.Play(_arrowAnimPath);
		}
		if(_currentMoveState == moveState.moving)
		{
			_arrowSprite.Frame = 48;
		}
		if(_currentMoveState == moveState.eating)
		{
			_enemyAnim.Play("Eat");

		}
	}

	public void SetCurrentBoardSquare((int,int) square)
	{
		_currentSquare = square;
	}

	private void OnMoveDecisionTimerTimeout()
	{
		if(_currentState == state.mean)
		{
			DetermineMoveDecision();
		}
		
	}

	private void OnMoveCountdownTimerTimeout()
	{
		_currentMoveState = moveState.moving;
		
	}

	private void OnItemCollectorItemEaten()
	{
		if (_eatTimer.IsStopped())
		{
			_currentMoveState = moveState.eating;
			_eatTimer.Start();
		}
	}

	private void OnEatTimerTimeout()
	{
		_currentMoveState = moveState.idle;
	}


}
