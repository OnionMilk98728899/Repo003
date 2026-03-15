using Godot;
using System;

public partial class MonsterAnimator : Node2D
{
	[Export] private AnimationPlayer monsterAnim;
	[Export] private Sprite2D monsterSprite;

	public enum moveState
	{
		idle,
		waiting,
		wandering,
		seeking,
		drinking,
		mutating

	}
	public moveState currentMoveState;

	public override void _Ready()
	{

	}
	public override void _PhysicsProcess(double delta)
	{

	}

	private void OnMoveStateChanged(int state)
	{
		currentMoveState = (moveState)state;

		switch (currentMoveState)
		{
			case moveState.idle:
				monsterAnim.Play("Idle");
				break;
			case moveState.waiting:
				monsterAnim.Play("Idle");
				break;
			case moveState.wandering:
				monsterAnim.Play("Walk");
				break;
			case moveState.seeking:
				monsterAnim.Play("Walk");
				break;
			case moveState.drinking:
				monsterAnim.Play("Walk");
				break;
			case moveState.mutating:
				monsterAnim.Play("Mutate");
				break;
		}

	}


	private void OnDirectionChanged(bool isLeft)
	{
		GD.Print("Flipped LEFT?  " + isLeft);
		monsterSprite.FlipH = isLeft;
		
	}


}
