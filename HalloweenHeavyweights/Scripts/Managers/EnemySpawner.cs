using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class EnemySpawner : Node2D
{
	[Export] private PackedScene enemyScene;
	private CharacterBody2D myEnemy;
	private SpawnComponent mySpawnComponent;
   	[Export] private Timer initiateDelayTimer;
	[Export] private EnemyManager enemyMan;
	[Export] private AnimationPlayer spawnAnimator;
	[Export] private Sprite2D spawnSprite;
	private (int,int) spawn_Set;
	private List<(int,int)> mySetsToSpawn;
	private Godot.Collections.Array<Vector2I> spawnLocationList;
	private Vector2 roomOffsetCoords;
	private int spawnSetQuantity, enemyTypeIdentifier, enemySpawnCount, spawnLocationCounter;
	private bool spawnDelayActive;

	public override void _Ready(){

		GlobalSignals.Instance.GenerateEnemySpawnLocations += ReceiveSpawnLocations;

	}


	private void CreateNewBatch(){

		spawnSetQuantity = GD.RandRange(1,2);

		for(int x = 0; x <= spawnSetQuantity; x++){

			enemyTypeIdentifier = GD.RandRange(1,1);
			enemySpawnCount = GD.RandRange(1,4);

			spawn_Set = (enemyTypeIdentifier, enemySpawnCount);

			mySetsToSpawn.Add(spawn_Set);

		}
	}

	private async void SpawnNewBatch(){

		spawnLocationCounter = 0;

		foreach((int,int) spawn_Set in mySetsToSpawn){

			for(int x = 0; x <= spawn_Set.Item2; x++){

				if(!spawnDelayActive){

					myEnemy = enemyScene.Instantiate<CharacterBody2D>();
					mySpawnComponent = myEnemy.GetNode<SpawnComponent>("SpawnComponent");

					myEnemy.GlobalPosition = new Vector2((spawnLocationList[spawnLocationCounter].X + roomOffsetCoords.X )*16 , 
					(spawnLocationList[spawnLocationCounter].Y + roomOffsetCoords.Y )*16);

					spawnSprite.GlobalPosition = myEnemy.GlobalPosition;

					mySpawnComponent.PlaySpawnAnimation("white");
					
					spawnLocationCounter++;
					if(spawnLocationCounter > spawnLocationList.Count){
						spawnLocationCounter = 0;
					}
					enemyMan.AddChild(myEnemy);

					await ToSignal(GetTree().CreateTimer(.4f), SceneTreeTimer.SignalName.Timeout);

				}
			}
		}
	}

	private void ReceiveSpawnLocations(Godot.Collections.Array<Vector2I> nodeList, Vector2 roomOffset){

		spawnLocationList = nodeList;
		mySetsToSpawn = new List<(int,int)>();
		roomOffsetCoords = roomOffset;
		initiateDelayTimer.Start();	
		
	}


	private void OnInitiatieDelayTimerTimeout(){

		CreateNewBatch();
		CallDeferred(nameof(SpawnNewBatch));

	}

}
