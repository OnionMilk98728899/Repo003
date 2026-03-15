using Godot;
using System;

public partial class EffectsManager : Node2D
{
		public static EffectsManager Instance { get; private set; }


	public override void _Ready()
	{

		Instance = this;
	}
}

