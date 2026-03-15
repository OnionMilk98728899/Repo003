using Godot;
using System;

public partial class SpawnComponent : Node2D
{
	[Signal] public delegate void SpawningSicknessEventHandler(double freezeTime);
	[Export] double spawnFreezeTime;
	[Export] private AnimationPlayer spawnAnim;
	[Export] private Sprite2D mySpawnSprite;
	[Export] private PathfindComponent myPathfinder;

	public void PlaySpawnAnimation(string spawnType){

		mySpawnSprite.Position = myPathfinder.GlobalPosition;
		EmitSpawningSickness();
		switch(spawnType){
			case "green":
				spawnAnim.Play("Green_Smoke_Spawn");
			break;
			case "purple":
				spawnAnim.Play("Purple_Smoke_Spawn");
			break;
			case "white":
				spawnAnim.Play("Light_Column_Spawn");
			break;
			case "red":
				spawnAnim.Play("Electric_Smoke_Spawn");
			break;

			
		}
	}

	private void EmitSpawningSickness(){
		EmitSignal(SignalName.SpawningSickness, (float)spawnFreezeTime);
	}
}
