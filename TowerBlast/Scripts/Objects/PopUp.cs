using Godot;
using System;
using Game.Palette;
//using GColor = Godot.Color;

public partial class PopUp : Node2D
{
	[Export] private Timer _decayTimer;
	[Export] private CharacterBody2D _popUpBody;
	[Export] private RichTextLabel _popUpLabel, _labelBackground;
	private Vector2 _popUpVelocity;
	private int _speed;
	private string _popUpText;

	public override void _Ready()
	{
		_decayTimer.Start();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_decayTimer.IsStopped())
		{
			_popUpVelocity.Y = -(float)(_speed * _decayTimer.TimeLeft);
			_popUpBody.Velocity = _popUpVelocity;
			_popUpBody.MoveAndSlide();
		}
	}

	public void SetProperties(Vector2 position, double decayTime, int speed, string text, string color)
	{
		_popUpBody.GlobalPosition = position;
		_decayTimer.WaitTime = decayTime;
		_speed = speed;
		_popUpLabel.Text = $"[color={color}]{text}[/color]";
		_labelBackground.Text = $"{text}";
		//_popUpLabel.Text.Modulate = color;
	}

	private void OnDecayTimerTimeout()
	{
		QueueFree();
	}
}
