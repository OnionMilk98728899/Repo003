using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalSignals : Node
{
    public static GlobalSignals Instance { get; private set; }
    [Signal] public delegate void ExitDoorTriggeredEventHandler();
    [Signal] public delegate void AdjustCharPositionEventHandler(Vector2 newPos);
    [Signal] public delegate void GenerateEnemySpawnLocationsEventHandler(Godot.Collections.Array<Vector2I> myList, Vector2 roomOffset);

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    // public EffectsManager GetFXManager(){
    // }
    // public void SetFXManager(EffectsManager effectsMan){
    // }

    public void EmitExitDoorTriggered()
    {
        EmitSignal(SignalName.ExitDoorTriggered);
    }

    public void EmitAdjustCharPosition(Vector2 newPos)
    {
        EmitSignal(SignalName.AdjustCharPosition, newPos);
    }

    public void EmitGenerateEnemySpawnLocations(Godot.Collections.Array<Vector2I> locationList, Vector2 roomOffset)
    {
        EmitSignal(SignalName.GenerateEnemySpawnLocations, locationList, roomOffset);
    }


}
