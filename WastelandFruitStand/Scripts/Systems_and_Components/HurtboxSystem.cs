using Godot;
using System;
using System.ComponentModel;

public partial class HurtboxSystem : Node2D
{
	[Export] private string targetGroup;
	[Export] private Area2D hurtBoxArea;
	[Export] private int damageMultiplier;
	[Export] private int maskLayer;
	public event Action<CharacterBody2D, int> OnHurt;

	public override void _Ready()
	{
		hurtBoxArea.SetCollisionMaskValue(maskLayer, true);
	}
	private void OnHurtAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D character)
		{
			if (character.GetNode<CharacterBody2D>(".").IsInGroup(targetGroup))
			{
				OnHurt?.Invoke(character, damageMultiplier);
			}
		}
	}
}



