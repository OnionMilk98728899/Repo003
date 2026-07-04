using Game.MagicTypes;
using Godot;
using System;
using System.Data;

public partial class Magic : Node2D
{
	[Export] private CharacterBody2D magicBody;
	[Export] private Sprite2D magicSprite;
	[Export] private AnimationPlayer magicAnim;
	[Export] private float magicSpeed;
	public enum noteType { eighth, quarter, doubleeighth }
	public enum unitType { player, enemy }
	public enum magicState { fly, neutralize, explode }
	public unitType currentUnitType;
	public magicState currentState;
	public noteType currentNoteType;
	public ProjectileColor myColor;
	private Vector2 magicVelocity, magicDirection;

	public override void _Ready()
	{
		currentState = magicState.fly;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentState == magicState.fly)
		{
			Fly();
		}

		AnimateMagic();
	}

	public void SetMagicStats(ProjectileColor color, Vector2 inputDirection, unitType unit, float speed)
	{
		myColor = color;
		currentUnitType = unit;
		magicSpeed = speed;

		magicDirection = inputDirection;

		if(currentUnitType== unitType.player)
		{
			if (magicDirection.X > 0){magicSprite.FlipH = false;}
			else{magicSprite.FlipH = true;}
		}
		else
		{
			if (magicDirection.X > 0){magicSprite.FlipH = true;}
			else{magicSprite.FlipH = false;}
		}
		
	}

	public void SetNoteType()
	{
		float rand = GD.RandRange(0, 2);
		switch (rand)
		{
			case 0:
				currentNoteType = noteType.eighth;
				break;
			case 1:
				currentNoteType = noteType.quarter;
				break;
			case 2:
				currentNoteType = noteType.doubleeighth;
				break;
		}
	}


	private void Fly()
	{
		magicVelocity.X = magicSpeed * magicDirection.X;
		magicBody.Velocity = magicVelocity;
		magicBody.MoveAndSlide();
	}

	private void AnimateMagic()
	{
		if (currentUnitType == unitType.player)
		{
			magicAnim.Play(currentState.ToString() + myColor.ToString());
		}
		else if (currentUnitType == unitType.enemy)
		{
			magicAnim.Play(currentNoteType.ToString() + currentState.ToString() + myColor.ToString());
		}

	}

	private void OnTargetDetectorBodyEntered(Node2D body)
	{
		if (currentUnitType == unitType.player)
		{
			if (body.IsInGroup("Enemy"))
			{
				currentState = magicState.explode;
			}
		}
		else if (currentUnitType == unitType.enemy)
		{
			if (body.IsInGroup("Player"))
			{

			}
		}
	}

	public void OnMagicExploded()
	{
		QueueFree();
	}

}
