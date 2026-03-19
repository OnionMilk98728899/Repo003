using Godot;
using System;

public partial class GameBoard : Node2D
{
	private void OnBoardBoundaryEntered(Node2D body)
	{
		GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GameOver);
		//GD.Print("Restart!");
	}
}



