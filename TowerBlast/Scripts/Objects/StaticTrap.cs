using Game.Damage;
using Game.Upgrades;
using Godot;
using System;
using System.Collections.Generic;


public partial class StaticTrap : Node2D
{
	[Export]
	private Texture2D _spikeTexture, _fireTileTexture, _iceTileTexture, _lightningTileTexture, _poisonTileTexture, _swarmTrapTexture,
	_wormTrapTexture, _ratTrapTexture, _treasureTrapTexture;
	[Export] private CharacterBody2D _trapBody;
	[Export] private AnimationPlayer _trapAnim;
	[Export] private Sprite2D _trapSprite;
	[Export] private PackedScene _minionScene;
	[Export] private Timer _spawnTimer, _goldGeneratorTimer;
	private Minion _myMinion;
	private Damage _damages;
	private List<Minion> _minionList = new List<Minion>();
	private Vector2 _spawnPosition;
	public TrapType _myTrapType;
	private string _animToPlay;
	private int _damage, _maxMinions, _currentTreasure;
	

	public enum _minionType
	{
		rat, worm, mites
	}
	public override void _Ready()
	{
		GlobalSignals.Instance.EnterExitBuildMode += PauseUnpauseSpawnTimer;

		_spawnTimer.WaitTime = GD.RandRange(40,60)/10;
		DetermineTrapBehavior();
		AdjustBodyPosition();
	}
	public override void _PhysicsProcess(double delta)
	{

	}
	private void EngageTrap()
	{
		_trapAnim.Play(_animToPlay);
		switch (_myTrapType)
		{
			case TrapType.swarm:
				SpawnMinion(_minionType.mites);
				break;
			case TrapType.ratswarm:
				SpawnMinion(_minionType.rat);
				break;
			case TrapType.bugswarm:
				SpawnMinion(_minionType.worm);
				break;
		}
	}

	private void SpawnMinion(_minionType minionType)
	{
		_myMinion = _minionScene.Instantiate<Minion>();
		if(_minionList.Count > 0)
		{
			Vector2 modifiedPosition = new Vector2(_spawnPosition.X + (_minionList.Count * 8), _spawnPosition.Y);
			_myMinion.SetMinionSpawnPosition(modifiedPosition);
			_myMinion.GetNode<MinionMovement>("MinionMovement").SetDebugLabelOffSet(_minionList.Count * 25);
		}
		else
		{
			_myMinion.SetMinionSpawnPosition(_spawnPosition);
		}
		_myMinion.SetMinionType(minionType);
		_myMinion.OnMinionDied += OnMinionDied;
		_minionList.Add(_myMinion);
		CallDeferred("AddChildToTrap", _myMinion);
	}

	private void OnMinionDied()
	{
		_minionList.Remove(_myMinion);
	}

	private void AddChildToTrap(Minion myMinion)
	{
		AddChild(myMinion);
	}

	private void AdjustBodyPosition()
	{
		if (_myTrapType != TrapType.swarm && _myTrapType != TrapType.bugswarm && _myTrapType != TrapType.ratswarm &&
		_myTrapType != TrapType.treasure)
		{
			_trapBody.Position = new Vector2(_trapBody.Position.X , _trapBody.Position.Y + 8);
		}
		else
		{
			_spawnPosition = new Vector2(_trapBody.Position.X, _trapBody.Position.Y + 16);
		}
	}

	public void SetDamage(Damage myDamage)
	{
		_damages = myDamage;
	}
	public Damage GetDamage()
	{
		return _damages;
	}

	public void SetTrapType(Texture2D texture, int hframes, int maxMinions, string trapAnim)
	{
		_trapSprite.Texture = texture;
		_trapSprite.Hframes = hframes;
		_animToPlay = trapAnim;
		if(_myTrapType == TrapType.swarm || _myTrapType == TrapType.ratswarm ||_myTrapType == TrapType.bugswarm)
		{
			_maxMinions = maxMinions;
		}
	}

	public void DetermineTrapBehavior()
	{
		switch (_myTrapType)
		{
			case TrapType.spikes:
				SetTrapType(_spikeTexture, 14, 0, "SpikeTrap");
				SetDamage(new Damage{baseDamage = 20});
				break;
			case TrapType.firetile:
				SetTrapType(_fireTileTexture, 14, 0, "FlameTrap");
				SetDamage(new Damage{fireDamage = 20});
				break;
			case TrapType.icetile:
				SetTrapType(_iceTileTexture, 14,  0, "IceTrap");
				SetDamage(new Damage{iceDamage = 20, movePenalty = 40});
				break;
			case TrapType.lightningtile:
				SetTrapType(_lightningTileTexture, 14,  0, "ElectricTrap");
				SetDamage(new Damage{elecDamage = 20});
				break;
			case TrapType.poisontile:
				SetTrapType(_poisonTileTexture, 14,   0,"PoisonTrap");
				SetDamage(new Damage{poisonDamage = 20});
				break;
			case TrapType.swarm:
				SetTrapType(_swarmTrapTexture, 10,   2,"SwarmTrap");
				break;
			case TrapType.bugswarm:
				SetTrapType(_wormTrapTexture, 10, 4,"BugSwarmTrap");
				break;
			case TrapType.ratswarm:
				SetTrapType(_ratTrapTexture, 10, 2, "RatSwarmTrap");
				break;
			case TrapType.treasure:
				SetTrapType(_treasureTrapTexture, 10, 0, "TreasureTrap");
				break;
		}
	}



	private void OnStaticDetectorBodyEntered(Node2D body)
	{
		if (body.GetNode<BasicEnemyMovement>("..") != null)
		{
			if(_myTrapType != TrapType.bugswarm && _myTrapType != TrapType.swarm && _myTrapType != TrapType.ratswarm)
			{
				EngageTrap();
			}
		}else if(body.GetNode<PlayerMovement>("..") != null)
		{
			if(_myTrapType == TrapType.treasure)
			{
				///PLAYER.COLLECTGOLD() here
				EngageTrap();
				_currentTreasure = 0;
			}
		}
	}
	public void FreePauseUnpauseTimer()
	{
		GlobalSignals.Instance.EnterExitBuildMode -= PauseUnpauseSpawnTimer;
	}

	private void PauseUnpauseSpawnTimer(bool isPaused)
	{
		if(_myTrapType == TrapType.bugswarm || _myTrapType == TrapType.swarm || _myTrapType == TrapType.ratswarm)
		{
			if (_spawnTimer.IsStopped())
			{
				_spawnTimer.Start();
			}
			else
			{
				_spawnTimer.Stop();
			}
		}

		if(_myTrapType == TrapType.treasure)
		{
			if (_goldGeneratorTimer.IsStopped())
			{
				_goldGeneratorTimer.Start();
			}
			else
			{
				_goldGeneratorTimer.Stop();
			}
		}
	}

	private void OnSpawnTimerTimeout()
	{
		if(_minionList.Count < _maxMinions)
		{
			EngageTrap();
			_spawnTimer.WaitTime= GD.RandRange(4,6);
		}
		else
		{
			GD.Print("Minions Maxed out!");
		}
	}

	private void OnGoldGeneratorTimeout()
	{
		_currentTreasure += 1;
		_trapAnim.Play("TreasureTrap_Accrue");
	}
}
