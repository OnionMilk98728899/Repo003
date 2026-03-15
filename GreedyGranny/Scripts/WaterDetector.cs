using Godot;
using System;

public partial class WaterDetector : Area2D
{
	[Signal] public delegate void waterStateChangedEventHandler(bool waterStatus);

	// [Signal] public delegate void surfaceStateChangedEventHandler(bool surfaceStatus);
	private bool isInWater, isOnSurface, isSubmerged;

	private void OnBodyEntered(Node2D body){

		if (body is TileMap)
		{

			if (!isInWater)
			{
				if (GetOverlappingBodies().Count >= 1)
				{
					isInWater = true;
					EmitSignal("waterStateChanged", isInWater);
				}
			}
		}

	}

	private void OnBodyExited(Node2D body){

		if(isInWater){
			if(GetOverlappingBodies().Count == 0){

				isInWater = false;
				EmitSignal("waterStateChanged", isInWater);
			}
		}

	}

}


