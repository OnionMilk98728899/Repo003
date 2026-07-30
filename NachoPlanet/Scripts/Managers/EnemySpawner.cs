using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemySpawner : Node2D
{
	[Export] private Node2D[] spawnPoints;
	[Export] private UnitManager unitMan;
	[Export] private PackedScene[] myEnemyScenes;
	[Export] private Timer spawnTimer;
	[Export] private float spawnInterval;
	[Export] private int MAX_ENEMY_INDEX;
	private int waveCount, enemyTypeCount, enemyIndex;
	private BaseEnemy myEnemy;
	private (int, int) enemyGroup;
	private List<(int, int)> waveGroup = new List<(int, int)>();

	public override void _Ready()
	{
		spawnTimer.WaitTime = spawnInterval;
		spawnTimer.Start();
	}


	private void GenerateWave()
	{
		//waveCount++;
		enemyIndex++;
		enemyIndex = Mathf.Clamp(enemyIndex, 1, MAX_ENEMY_INDEX);

		int randWaveCount = GD.RandRange(1, 2);

		for (int i = 0; i <= randWaveCount; i++)
		{
			int randEnemy = GD.RandRange(1, enemyIndex);
			int randQuant = GD.RandRange(1, 2);
			waveGroup.Add((randEnemy, randQuant));
		}
	}

	private void SpawnEnemy(int enemyID)
	{

		switch (enemyID)
		{
			case 1:
				myEnemy = myEnemyScenes[0].Instantiate<BaseEnemy>();
				break;
			case 2:
				break;
			case 3:
				break;
			case 4:
				break;
			case 5:
				break;
			case 6:
				break;
			case 7:
				break;
			case 8:
				break;


		}

		if (unitMan.enemyCount < 35)
		{
			unitMan.AddChild(myEnemy);
			unitMan.enemyCount++;
			int nodeID = GD.RandRange(0, spawnPoints.Length - 1);
			Vector2 spawnLocation = new Vector2(spawnPoints[nodeID].Position.X + GD.RandRange(16, 32), spawnPoints[nodeID].Position.Y + GD.RandRange(16, 32));
			myEnemy.GlobalPosition = spawnLocation;
		}



	}

	private void OnSpawnTimerTimeout()
	{

		if (unitMan.enemyCount < 35)
		{
			GenerateWave();

			foreach ((int, int) wave in waveGroup)
			{
				for (int i = 0; i <= wave.Item2; i++)
				{
					SpawnEnemy(wave.Item1);
				}
			}

			waveGroup.Clear();
			spawnTimer.Start();
		}
		else
		{
			spawnTimer.Start();
		}

	}



}
