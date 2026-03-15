using Godot;
using System;

public partial class ProjectileManager : Node2D
{
	public static ProjectileManager Instance { get; private set; }


	public override void _Ready()
	{

		Instance = this;
	}
}
