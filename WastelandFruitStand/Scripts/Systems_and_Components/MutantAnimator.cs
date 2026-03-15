using Godot;
using System;

public partial class MutantAnimator : Node2D
{
	[Export] private AnimationPlayer mutantAnim;
	[Export] private Sprite2D mutantSprite;

	public enum moveState
	{
		spawning,
		lookAround,
		wandering,
		seeking,
		preattacking,
		attacking,
		hurt,
		dying,
		dead,
		destroying

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
			case moveState.spawning:
				mutantAnim.Play("Spawn");
				break;
			case moveState.lookAround:
				mutantAnim.Play("Look_Around");
				break;
			case moveState.wandering:
				mutantAnim.Play("Walk");
				break;
			case moveState.seeking:
				mutantAnim.Play("Walk");
				break;
			case moveState.preattacking:
				mutantAnim.Play("Pre_Attack");
				break;
			case moveState.attacking:
				mutantAnim.Play("Attack");
				break;
			case moveState.hurt:
				mutantAnim.Play("Hurt");
				break;
			case moveState.dying:
				mutantAnim.Play("Die");
				break;
			case moveState.dead:
				mutantAnim.Play("Dead");
				break;
			case moveState.destroying:
				mutantAnim.Play("Destroy");
				break;
		}
	}

	private void OnDirectionChanged(bool isLeft)
	{
		mutantSprite.FlipH = isLeft;
	}
}
