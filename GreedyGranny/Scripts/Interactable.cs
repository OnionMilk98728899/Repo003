using Godot;
using System;

public partial class Interactable : Node2D
{

	private granny myGranny;
	private void OnArea2DBodyEntered(Node2D body)
	{
		if(body.IsInGroup("Player")){

			myGranny = body.GetNode<granny>(".");
			myGranny.isInteractable = true;
		}

	}

	private void OnArea2DBodyExited(Node2D body)
	{
		if(body.IsInGroup("Player")){

			myGranny.isInteractable = false;
		}

	}
}



