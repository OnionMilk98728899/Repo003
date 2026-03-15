using Godot;
using System;

public partial class EnvironmentDetector : Area2D
{
    [Signal] public delegate void LadderEnteredEventHandler();
    [Signal] public delegate void LadderExitedEventHandler();

    public void OnBodyEntered(TileMap body){ 
        EmitSignal("LadderEntered");
    }

    public void OnBodyExited(TileMap body){
        EmitSignal("LadderExited");
    }
}
