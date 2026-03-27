using Godot;
using System;

public partial class EnemyMovement : Node2D
{
	[Export] private CharacterBody2D _enemyBody;
	[Export] private Timer _moveDecisionTimer, _moveCountDownTimer;
	[Export] private AnimationPlayer _enemyAnim, _arrowAnim;
	[Export]private Sprite2D _arrowSprite;
	[Export] private int SPEED;
	[Export]private Label debugLabel;

	private Vector2 _currentPosition, _targetPosition, _enemyVelocity, _enemyPosition, _BOARD_ORIGIN_POSITION = new Vector2(152, 54);
	private (int,int) _currentSquare, _targetSquare;
	private string _arrowAnimPath;
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
		//debugLabel.Text =  " " + _currentSquare ;
		// if(_currentMoveState == moveState.preparing || _currentMoveState == moveState.moving){
			
		// 	debugLabel.Text =  " "+ _currentSquare + "\t" + _targetSquare;
		// }
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
	}

	private void DetermineArrowDirection()
	{
		(int,int) difference = (_targetSquare.Item1 - _currentSquare.Item1, _targetSquare.Item2 - _currentSquare.Item2);

		if(difference.Item1 < 0)
		{
			GD.Print("Arrow Left");
			_arrowAnimPath = "Arrow_Left";
		}
		if(difference.Item1 > 0)
		{
			GD.Print("Arrow Right");
			_arrowAnimPath = "Arrow_Right";
		}
		if(difference.Item2 < 0)
		{
			GD.Print("Arrow Up");
			_arrowAnimPath = "Arrow_Up";
		}
		if(difference.Item2 > 0)
		{
			GD.Print("Arrow Down");
			_arrowAnimPath = "Arrow_Down";
		}

	
	}

	private void MoveEnemy(double delta)
	{
		if(_currentMoveState == moveState.idle)
		{
			_enemyVelocity = Vector2.Zero;
		}
		if(_currentMoveState == moveState.preparing)
		{
			_enemyVelocity = Vector2.Zero;
		}
		if (_currentMoveState == moveState.moving)
		{
			Vector2 direction = _targetPosition - _enemyBody.GlobalPosition;
			_enemyBody.GlobalPosition += direction * SPEED/100;

			if(Mathf.Abs(_enemyBody.GlobalPosition.X  - _targetPosition.X) <= 1 && Mathf.Abs(_enemyBody.GlobalPosition.Y - _targetPosition.Y) <= 1)
			{
				GD.Print("Resetting position and updating");
				_enemyBody.GlobalPosition = _targetPosition;
				_currentMoveState = moveState.idle;
				_moveDecisionTimer.Start();
				_currentSquare = _targetSquare;
			}
		}
	}

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
		_moveDecisionTimer.Start();

	}

	private void OnMoveCountdownTimerTimeout()
	{
		_currentMoveState = moveState.moving;
		
	}



}
