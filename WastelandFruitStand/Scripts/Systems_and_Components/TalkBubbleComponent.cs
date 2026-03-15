using Godot;
using System;

public partial class TalkBubbleComponent : Node2D
{
	[Export] private Sprite2D bubbleSprite, expressionSprite;
	[Export] private Texture2D alertEx, thirstyEx, unhappyEx, happyEx;
	[Export] private AnimationPlayer bubbleAnim;
	[Export] private CharacterBody2D speakerBody;
	[Export] float thirstCoefficient;
	private Vector2 speakerPosition, bubbleOffset;
	private string bubbleDirectionAnimation;

	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		thirstCoefficient -= .01f;
		if (thirstCoefficient < 0)
		{
			thirstCoefficient = 0;
		}
		if (bubbleAnim.IsPlaying())
		{
			bubbleSprite.GlobalPosition = speakerBody.GlobalPosition + bubbleOffset;
			//expressionSprite.GlobalPosition = bubbleSprite.GlobalPosition;
		}
	}

	public void OnStandNoticed()
	{
		PlayRandomBubbleAnimation();
		expressionSprite.Texture = alertEx;
	}

	private void PlayRandomBubbleAnimation()
	{
		int decider = GD.RandRange(0, 2);

		switch (decider)
		{
			case 0:
				bubbleDirectionAnimation = "LeftBubble";
				bubbleOffset = new Vector2(-18, -18);
				break;
			case 1:
				bubbleDirectionAnimation = "RightBubble";
				bubbleOffset = new Vector2(18, -18);
				break;
			case 2:
				bubbleDirectionAnimation = "MiddleBubble";
				bubbleOffset = new Vector2(0, -18);
				break;

		}

		bubbleAnim.Play(bubbleDirectionAnimation);
	}

}
