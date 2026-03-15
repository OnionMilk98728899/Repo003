using Godot;
using System;

public partial class Door_2D : Node2D
{
	[Export] private Sprite2D doorSprite;
	[Export] private AnimationPlayer doorAnim;
	[Export] private Timer doorTimer;
	[Export] private Label debugLabel;
	[Export] private CollisionShape2D myCollider, doorLockColl;
	public enum direction{
		left, right, down, up
	};
	public enum doorState{
		open, closed, opening
	}
	public enum enterExit{
		enter, exit
	}
	public enterExit enterExitStatus;
	public direction myDirection;
	public doorState myDoorState;
	private string animationtoPlay;
	private double elapsedTime;
	private Vector2 doorColliderPosition, lockPosition;
	public bool isClosed, isOpening, isOpen, newRoomTriggered;

	private int doorEntryCounter = 0;
	public override void _Ready()
	{
		doorTimer = GetNode<Timer>("DoorTimer");
		doorAnim = GetNode<AnimationPlayer>("DoorAnim");
	}

	public override void _PhysicsProcess(double delta)
	{
		//debugLabel.Text = myDoorState.ToString() + "  " + elapsedTime;
		elapsedTime += delta;

		if(elapsedTime > 1 && myDoorState == doorState.opening){
			OpenDoorRemotely();
		}
	}

	public void DetermineDoorDirection(){

		switch(myDirection){

			case direction.up:
				doorSprite.Frame = 0;
				doorColliderPosition = new Vector2(24,58);
				lockPosition = new Vector2(24,27);
				
				animationtoPlay = "Opening_Up";

			break;
			case direction.down:
				doorSprite.Frame = 18;
				Vector2 adjustedPosII = new Vector2(GlobalPosition.X, GlobalPosition.Y -32);
				GlobalPosition = adjustedPosII;
				doorColliderPosition = new Vector2(24,8);
				lockPosition = new Vector2(24,36);
				animationtoPlay = "Opening_Down";

			break;
			case direction.left:
				doorSprite.Frame = 9;
				doorColliderPosition = new Vector2(33,24);
				lockPosition = new Vector2(9,24);
				animationtoPlay = "Opening_Left";

			break;
			case direction.right:
				doorSprite.Frame = 27;
				Vector2 adjustedPos = new Vector2(GlobalPosition.X - 32, GlobalPosition.Y+1);
				GlobalPosition = adjustedPos;
				doorColliderPosition = new Vector2(40,23);
				lockPosition = new Vector2(39,24);
				animationtoPlay = "Opening_Right";
			break;
	   	}
		
		myCollider.Position = doorColliderPosition;
		doorLockColl.Position = lockPosition;
	}

	private void OnDoorAreaEntered(Node2D body)
	{
		
		doorEntryCounter++;
		switch(myDoorState){

			case doorState.closed:
				doorAnim.Play(animationtoPlay);
				if(isClosed){
					doorTimer.Start();
					isClosed = false;
				}
				if(enterExitStatus == enterExit.exit && !newRoomTriggered){
					GlobalSignals.Instance.EmitExitDoorTriggered();
					newRoomTriggered = true;
				}
			break;
			case doorState.opening:
				//doorTimer.Start();

			break;
			case doorState.open:
				// if(enterExitStatus == enterExit.exit && !newRoomTriggered){
				// 	GlobalSignals.Instance.EmitExitDoorTriggered();
				// 	newRoomTriggered = true;
				// }
			break;
		}
	}

	public void OpenDoorRemotely(){
		if(isClosed){
			doorAnim.Play(animationtoPlay);
			isClosed = false;
		}
	}

	private void OnDoorTimerTimeout(){

		
		if(myDoorState == doorState.opening){
			doorAnim.Play(animationtoPlay);
		}

		myDoorState = doorState.open;

	}


}
