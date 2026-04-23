using Godot;
using System;

public partial class Popup : Node2D
{
	[Export] private RichTextLabel _popupLabel;
	[Export] private double _lifetime;
	private double _life;
	private int _popupCount;
	private Vector2 _popUpPosition;
	public enum scoreType{burger, master, special}
	public scoreType _myScoreType;

	public override void _PhysicsProcess(double delta)
	{
		MovePopup(delta);
	}

	public void SetPopUpStats(Vector2 position, int score, scoreType type)
	{
		GlobalPosition = position;
		_popUpPosition = position;

		_popupCount = score;
		if(score > 0)
		{
			_popupLabel.Text = $"+{score}";
		}
		else
		{
			_popupLabel.Text = $"-{score}";
		}
		_myScoreType = type;
	}


	private void MovePopup(double delta)
	{
		_popUpPosition.Y -= .5f;
		GlobalPosition = _popUpPosition;
		_life += delta;
		if (_life > _lifetime)
		{
			SendSignalToCountScore();
			QueueFree();
		}
	}

	private void SendSignalToCountScore()
	{
		switch (_myScoreType)
		{
			case scoreType.burger:
			GlobalResources.Instance.IncreaseBurgerScore(_popupCount);
			break;
			case scoreType.master:
			GlobalResources.Instance.IncreaseScore();
			break;
			// case scoreType.special:
			// break;
		}
	}


}
