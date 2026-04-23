using Godot;
using System;
using System.Threading.Tasks;

public partial class ItemCollector : Node2D
{
	[Signal] public delegate void ItemEatenEventHandler();
	[Signal] public delegate void PlayerKnockoutEventHandler();
	[Export] private CollisionShape2D _collectorCollider;
	private Item _myItem;
	private bool _isJumping;
	private enum unitType
	{
		meanie, player
	}
	[Export] private unitType _myUnitType;
	private void OnItemCollectorBodyEntered(Node2D body)
	{
		if (_myUnitType == unitType.player)
		{
			if (body.IsInGroup("Items") && !_isJumping)
			{
				_myItem = body.GetNode<Item>("..");
				_myItem.ItemCollected();
			}

			if (body.IsInGroup("Meanies"))
			{
				EnemyMovement enemy = body.GetNode<EnemyMovement>("..");

				if(enemy._currentState == EnemyMovement.state.mean)
				{
					if (_isJumping)
					{
						enemy.TurnNeutral();
						AudioManager.Instance.PlaySFX(AudioManager.Instance._sfxPlayer, AudioManager.Instance._audioLibrary.enemyConverted);
					}
					else
					{
						GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.GameOver);
						AudioManager.Instance.PlaySFX(AudioManager.Instance._sfxPlayer, AudioManager.Instance._audioLibrary.deathCrash);
					}
					
				}
				else if(enemy._currentState == EnemyMovement.state.neutral)
				{
					if (_isJumping)
					{
						enemy.TurnNice();
						AudioManager.Instance.PlaySFX(AudioManager.Instance._sfxPlayer,AudioManager.Instance._audioLibrary.enemyConverted);
					}
					else
					{
						EmitSignal(SignalName.PlayerKnockout);
					}
				}
				else if (enemy._currentState == EnemyMovement.state.nice)
				{
					if (!_isJumping)
					{
						enemy.KillMeanie();
						GlobalResources.Instance.CountNewBurgerScore(15);
						GlobalSignals.Instance.EmitSignal(GlobalSignals.SignalName.AddTimeToSpecialTime, 1);
						AudioManager.Instance.PlaySFX(AudioManager.Instance._sfxPlayer, AudioManager.Instance._audioLibrary.meanieKill);
					}
					
				}
				
			}
		}
		if (_myUnitType == unitType.meanie)
		{
			if (body.IsInGroup("Items"))
			{
				_myItem = body.GetNode<Item>("..");
				_myItem.ItemEaten();
				EmitSignal(SignalName.ItemEaten);
				AudioManager.Instance.PlaySFX(AudioManager.Instance._meanieSFX,AudioManager.Instance._audioLibrary.meanieNoise);
			}
		}

	}

	public void DisableEnableCollider(bool isActive)
	{
		_isJumping = !isActive;
	}
}



