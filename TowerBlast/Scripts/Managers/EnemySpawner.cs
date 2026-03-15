using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	[Export] private PackedScene _enemy1Scene;
	[Export] private Timer _spawnTimer;
	[Export] private EnemyManager _enemyMan;
	private Enemy1 _myEnemy1;
	private BasicEnemyMovement _enemyMovement;
	private bool _isSpawnerActive, _testEnemySpawned;
	private Vector2 _SPAWN_POSITION = new Vector2(336, -8), _targetPosition;

	/// new Vector2(336, -8)

	public override void _Ready()
	{
		GlobalSignals.Instance.EnterExitBuildMode += ActivateDeactivateSpawner;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isSpawnerActive)
		{
			if (_spawnTimer.IsStopped())
			{

				_spawnTimer.Start();
				
			}
		}
	}

	private void ActivateDeactivateSpawner(bool isActive)
	{
		_isSpawnerActive = !isActive;
	}

	private void OnSpawnTimerTimeout()
	{
		//if (!_testEnemySpawned)
		//{
			_myEnemy1 = _enemy1Scene.Instantiate<Enemy1>();
			_myEnemy1.GlobalPosition = _SPAWN_POSITION;
			_enemyMovement = _myEnemy1.GetNode<BasicEnemyMovement>("BasicEnemyMovement");
			_enemyMovement.SetEnemyPath(MyGlobalResources._enemyPath);
			_enemyMan.AddChild(_myEnemy1);
			_testEnemySpawned = true;
		//}

	}

}
