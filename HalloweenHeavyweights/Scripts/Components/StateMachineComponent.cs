using Godot;
using System;

public partial class StateMachineComponent : Node2D
{
	[Export] private AnimationPlayer bodyAnim;
	[Export] private string movingUpAnim, movingDownAnim, hurtDownAnim, hurtUpAnim, attackDownAnim, attackUpAnim;
	private string animationNodePath;
	private bool isHurt, isAttacking, isMovingUp;
	private enum State{
		movingUp, movingDown, hurtDown, hurtUp, attackDown, attackUp
	}
	private State currentState;

	public override void _PhysicsProcess(double delta)
	{
		DetermineStateAndBehavior();
	}

	private void DetermineStateAndBehavior(){

		if (!isHurt)
		{
			if (!isAttacking)
			{
				if (!isMovingUp)
				{
					currentState = State.movingDown;
				}
				else
				{
					currentState = State.movingUp;
				}

			}
			else{

				if (!isMovingUp)
				{
					currentState = State.attackDown;
				}
				else
				{
					currentState = State.attackUp;
				}

			}

		}else
		{
			if (!isMovingUp)
			{
				currentState = State.hurtDown;
			}
			else
			{
				currentState = State.hurtUp;
			}
		}

		switch(currentState){

			case State.movingUp:
				animationNodePath = movingUpAnim;
			break;
			case State.movingDown:
				animationNodePath = movingDownAnim;
			break;
			case State.attackUp:
				animationNodePath = attackUpAnim;
			break;
			case State.attackDown:
				animationNodePath = attackDownAnim;
			break;
			case State.hurtUp:
				animationNodePath = hurtUpAnim;
			break;
			case State.hurtDown:
				animationNodePath = hurtDownAnim;
			break;
		}

		bodyAnim.Play(animationNodePath);

	}


	private void OnMovementDirectionChanged(bool movingUp)
	{
		isMovingUp = movingUp;

	}
	private void OnInitiateMeleeAttack(bool attacking)
	{
		isAttacking = attacking;
	}
	private void OnHurtEnemyUnit(bool hurt)
	{
		isHurt = hurt;
	}
}



