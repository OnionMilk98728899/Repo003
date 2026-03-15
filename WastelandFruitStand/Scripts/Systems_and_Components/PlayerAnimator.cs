using Godot;
using System;

public partial class PlayerAnimator : Node2D
{
	[Export] private AnimationPlayer playerAnim;
	[Export] private PlayerActionController playerAction;
	[Export] public Timer attackTimer;
	private bool hasPressed, isPicking, isHurt;
	public enum moveState
	{
		idle, run, jump, fall, land,  hurt, recovering, attack
	}

	public moveState mvmtState, lastMoveState;
	public override void _Ready()
	{

	}

	public override void _PhysicsProcess(double delta)
	{

		DetermineAnimState();
	}

	private void OnPlayerMoveStateChanged(int state)
	{
		if (mvmtState != moveState.attack)
		{
			mvmtState = (moveState)state;
			lastMoveState = mvmtState;
		}
		
	}

	private void DetermineAnimState()
	{
		isHurt = false;
		switch (mvmtState)
		{
			case moveState.idle:
				if (!isPicking)
				{
					playerAnim.Play("Idle");
				}
				else
				{
					playerAnim.Play("PickFruit");
				}
				break;
			case moveState.run:
				playerAnim.Play("Run");
				break;
			case moveState.jump:
				playerAnim.Play("Jump");
				break;
			case moveState.fall:
				playerAnim.Play("Fall");
				break;
			case moveState.land:
				playerAnim.Play("Land");
				break;
			case moveState.attack:
				playerAnim.Play("Attack");
				break;
			case moveState.hurt:
				playerAnim.Play("Hurt");
				isHurt = true;
				break;
			case moveState.recovering:
				playerAnim.Play("Recover");
				break;
		}
		if (playerAction.GetAction2Status() && !isHurt)
		{
			if (!hasPressed)
			{
				lastMoveState = mvmtState;
				mvmtState = moveState.attack;
				attackTimer.Start();
				hasPressed = true;
				
			}
		}
		if (playerAction.GetAction3Status() && !isHurt)
		{
			if (!hasPressed && playerAction.GetCloseToTree())
			{
				lastMoveState = mvmtState;
				mvmtState = moveState.idle;
				attackTimer.Start();
				isPicking = true;
				hasPressed = true;
			}
		}
	}

	private void OnAttackTimerTimeout()
	{
		hasPressed = false;
		isPicking = false;
		mvmtState = lastMoveState;
	}

}
