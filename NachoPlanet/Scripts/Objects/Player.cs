using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public override void _Ready()
	{
		GameManager.Instance.player = this;
		GD.Print("player data loaded");
	}

	public void OnSceneEntered()
	{
		
	   
	}

	public CharacterBody2D GetPlayerBody()
	{
		return this;
	}
}
