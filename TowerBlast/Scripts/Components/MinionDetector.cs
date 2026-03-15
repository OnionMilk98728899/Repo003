using Godot;
using System;
using System.Collections.Generic;

public partial class MinionDetector : Node2D
{
	[Signal] public delegate void EngageAttackEventHandler(bool isAttacking);
	[Export] private Timer _attackTimer;
	[Export] private BasicEnemyMovement _myEnemyMovement;
	private MinionMovement _myMinion;
	private List<MinionMovement> _minionList = new List<MinionMovement>();
	private bool _isAMinionDetected, _isInAttackMode;
	private int _damage;


	public override void _PhysicsProcess(double delta)
	{
		if (_isInAttackMode)
		{
			if (_attackTimer.IsStopped())
			{
				_attackTimer.Start();
			}
		}
	}

	public void SetDamage(int damage)
	{
		_damage = damage;
	}
	public void SetAttackMode(bool isAttacking)
	{
		_isInAttackMode = isAttacking;
	}

	public bool GetAttackMode()
	{
		return _isInAttackMode;
	}
	private void OnMinionDetectorEntered(Node2D body)
	{
		if (body.IsInGroup("Minions"))
		{
			if(body.GetNode<MinionMovement>("..").GetTargetId() == _myEnemyMovement)
			{
				_myMinion = body.GetNode<MinionMovement>("..");
				_isInAttackMode = true;
			}
		}
	}

	public void EngageWithFreshlySpawnedMinion(CharacterBody2D body)
	{
		_myMinion = body.GetNode<MinionMovement>("..");
		_isInAttackMode = true;
	}

	private void OnMinionDetectorExited(Node2D body)
	{

	}

	private void OnAttackTimerTimeout()
	{
		if (_isInAttackMode)
		{
			_myMinion.DamageMinion(_damage);
		}
	}


	
}



