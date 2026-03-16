using Godot;
using System;

public partial class GameBoard : Node2D
{
	private void OnBoardBoundaryEntered(Node2D body)
	{
        GD.Print("Game Reset!");
	}
}



