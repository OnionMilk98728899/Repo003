using Godot;
using System;

public partial class BossDamageable : Node2D
{
	[Signal] public delegate void isHurtEventHandler(bool hurt);
	[Signal] public delegate void isDyingEventHandler(bool dying);
	[Signal] public delegate void canDamagePlayerAgainEventHandler();
	[Export] private int hitPoints;

	private Area2D grubArea, grubJumpArea;
	private bool playerInJumpArea, plyerinBodyArea, takingDamage, hurtColliderOff, dying, dead;
	private PlayerMovement pMove;
	private CollisionShape2D grubColl;
	public  Timer hurtTimer;
	

	public override void _Ready()
	{
		hurtTimer = GetNode<Timer>("../HurtTimer");
		grubColl = GetNode<CollisionShape2D>("BigGrubArea/BigGrubCollider");
		grubArea = GetNode<Area2D>("BigGrubArea");
	}

	public override void _PhysicsProcess(double delta)
	{

	}

	private void OnJumpAreaEntered(Node2D body)
	{
		if(body.IsInGroup("Player")){
			playerInJumpArea = true;
		}
	}
	private void OnJumpAreaExited(Node2D body)
	{
		if(body.IsInGroup("Player")){
			playerInJumpArea =false;
		}
	}
	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			pMove = body.GetNode<PlayerMovement>("./PlayerMovement");

			if (playerInJumpArea)
			{
				takingDamage = true;
				EmitSignal("isHurt", takingDamage);
				grubArea.SetCollisionMaskValue(3, false);
				hurtColliderOff = true;
				hurtTimer.Start();
				hitPoints--;
				GD.Print($"hitPoints = {hitPoints} ");
				pMove.BouncePlayer();
			}
			if(!playerInJumpArea && !pMove.charging && !pMove.isStomping){
				pMove.TakeDamage(GlobalPosition);
			}
		}
	}

	private void OnHurtTimerTimeout()
	{

		if(hurtColliderOff){
			hurtTimer.Start();
			takingDamage = false;
			EmitSignal("isHurt", takingDamage); 
			
			hurtColliderOff = false;
		}else{
			grubArea.SetCollisionMaskValue(3, true);
			EmitSignal("canDamagePlayerAgain");
		}
		if(hitPoints <= 0){
			GD.Print("Dying!!!!!!!!!");
			grubArea.SetCollisionMaskValue(3, false);
			dying = true;
			EmitSignal("isDying", dying);
		}
		
	}

	private void DisableGrubCollider(){
		
	}
}






