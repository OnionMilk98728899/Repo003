using Godot;
using System;

public partial class EnemyMovement : Node2D
{
	[Signal] public delegate void DestroyMeanieEventHandler();
	[Export] private CharacterBody2D _enemyBody;
	[Export] private Timer _moveDecisionTimer, _moveCountDownTimer, _eatTimer, _destroyTimer;
	[Export] private AnimationPlayer _enemyAnim, _arrowAnim;
	[Export] private Sprite2D _arrowSprite;
	[Export] private int SPEED;
	[Export] private Label debugLabel;

	private Vector2 _currentPosition, _targetPosition = Vector2.Zero, _enemyVelocity, _enemyPosition, _BOARD_ORIGIN_POSITION = new Vector2(152, 54);
	private (int, int) _currentSquare, _targetSquare;
	private string _arrowAnimPath;
	private bool _isEating, _isDropping;
	public enum state
	{
		mean, nice, destroying
	}
	public enum moveState
	{
		preparing, moving, idle
	}
	public state _currentState;
	public moveState _currentMoveState;

	public override void _Ready()
	{
		GlobalSignals.Instance.InitiateSpecialTime += TurnNice;
		GlobalSignals.Instance.ResumeNormalMode += TurnMean;

		if (GlobalResources.Instance._currentGameState == GlobalResources.gameState.special)
		{
			_currentState = state.nice;
		}
		else
		{
			_currentState = state.mean;
		}
		_currentMoveState = moveState.idle;
		_isDropping = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		AnimateEnemy();
		MoveEnemy(delta);
		//debugLabel.Text = _currentMoveState.ToString();
	}

	private void DetermineMoveDecision()
	{
		float bias = .75f;
		float rand = GD.Randf();
		if (rand <= bias)
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
		(int, int) difference = (_targetSquare.Item1 - _currentSquare.Item1, _targetSquare.Item2 - _currentSquare.Item2);

		if (difference.Item1 < 0)
		{
			_arrowAnimPath = "Arrow_Left";
		}
		if (difference.Item1 > 0)
		{
			_arrowAnimPath = "Arrow_Right";
		}
		if (difference.Item2 < 0)
		{
			_arrowAnimPath = "Arrow_Up";
		}
		if (difference.Item2 > 0)
		{
			_arrowAnimPath = "Arrow_Down";
		}
	}

	private void MoveEnemy(double delta)
	{
		if (_currentMoveState == moveState.idle)
		{
			_enemyVelocity = Vector2.Zero;
			//SnapEnemyToGrid(_targetPosition);
		}
		if (_currentMoveState == moveState.preparing)
		{
			_enemyVelocity = Vector2.Zero;
		}
		if (_currentMoveState == moveState.moving)
		{
			float step = SPEED * (float)delta;
			Vector2 currentPos = _enemyBody.GlobalPosition;
			Vector2 newPos = currentPos.MoveToward(_targetPosition, step);

			_enemyBody.GlobalPosition = newPos;

			if (newPos == _targetPosition)
			{
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


	private void AnimateEnemy()
	{
		if (_currentState == state.nice)
		{
			_arrowAnim.Stop();
			_arrowSprite.Frame = 48;

			if (_isDropping)
			{
				_enemyAnim.Play("Drop");
			}
			else
			{
				_enemyAnim.Play("Nice");
			}
		}
		else if (_currentState == state.mean)
		{
			if (_isEating)
			{
				_enemyAnim.Play("Eat");
			}
			if (_isDropping)
			{
				_enemyAnim.Play("Drop");
			}
			if (!_isEating && !_isDropping)
			{
				_enemyAnim.Play("Mean");
			}
		}
		else if (_currentState == state.destroying)
		{
			_enemyAnim.Play("Destroy");
		}

		if (_currentMoveState == moveState.preparing && _currentState == state.mean)
		{
			_arrowAnim.Play(_arrowAnimPath);
		}
		if (_currentMoveState == moveState.moving)
		{
			_arrowSprite.Frame = 48;
		}


	}

	public void InitializeDecisionMaking()
	{
		_moveDecisionTimer.Start();
		_isDropping = false;
	}

	public void SetCurrentBoardSquare((int, int) square)
	{
		_currentSquare = square;
	}

	public void TurnNice(double time)
	{
		if (IsInstanceValid(_moveDecisionTimer) && IsInstanceValid(_moveCountDownTimer))
		{
			//_currentMoveState = moveState.idle;
			_currentState = state.nice;
			_moveDecisionTimer.Stop();
			_moveCountDownTimer.Stop();
		}

	}

	private void TurnMean()
	{
		if (IsInstanceValid(_moveDecisionTimer))
		{
			_currentState = state.mean;
			_moveDecisionTimer.Start();
		}

	}

	public void KillMeanie()
	{
		_currentState = state.destroying;
		_destroyTimer.Start();
		BoardManager.Instance._occupiedMeanieSquares.Remove(_currentSquare);
		//_moveDecisionTimer.Stop();
	}

	private void OnMoveDecisionTimerTimeout()
	{
		if (_currentState == state.mean)
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
		if (!_isEating)
		{
			_isEating = true;
			_eatTimer.Start();
		}
	}

	private void OnEatTimerTimeout()
	{
		_isEating = false;
	}

	private void OnDestroyTimerTimeout()
	{
		EmitSignal(SignalName.DestroyMeanie);
	}


}
