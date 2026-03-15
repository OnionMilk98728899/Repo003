using Godot;
using System;

public partial class FruitTree : Node2D
{
	[Export] private Node2D treeLayer;
	[Export] private RichTextLabel treeLabel;
	[Export] private Timer fruitTimer;
	[Export] private int maxFruitQuantity;
	[Export] private AnimationPlayer treeAnim;
	[Export] private Sprite2D treeSprite;
	[Export] private CharacterBody2D treeBody;
	private int fruitQuantity;
	private int newZ;
	private string animationString;
	private PlayerActionController playerAction;
	private PlayerInventory playerInventory;

	private bool isPlayerClose;

	public override void _Ready()
	{
		fruitQuantity = maxFruitQuantity;
		ManuallySortZIndex();
	}
	public override void _PhysicsProcess(double delta)
	{
		if (fruitQuantity > 0)
		{

			treeLabel.Text = fruitQuantity.ToString();
		}
		else
		{
			treeLabel.Text = "OUT OF FRUIT!";
		}
		DetermineTreeAnimation();
		treeLabel.Text = newZ.ToString();
		PickFruit();
		
	}

	private void PickFruit()
	{
		if (playerAction != null)
		{
			if (playerAction.GetAction3Status() && isPlayerClose)
			{
				if (fruitQuantity > 0)
				{
					fruitQuantity -= 1;
					playerInventory.AddFruit(1);
				}
				else
				{
					treeLabel.Text = "SORRY!";
				}
				if (fruitQuantity == maxFruitQuantity - 1)
				{
					fruitTimer.Start();
				}

			}
		}
	}

	private void OnTreeAreaBodyEntered(Node2D body)
	{
		if (body.GetNode<PlayerActionController>("../PlayerActionController") != null)
		{
			playerAction = body.GetNode<PlayerActionController>("../PlayerActionController");
			playerInventory = body.GetNode<PlayerInventory>("../PlayerInventory");
			isPlayerClose = true;
			GlobalSignals.Instance.EmitPlayerCloseToTree(isPlayerClose);
		}

	}

	private void OnTreeAreaBodyExited(Node2D body)
	{
		if (body.GetNode<PlayerActionController>("../PlayerActionController") != null)
		{
			isPlayerClose = false;
			GlobalSignals.Instance.EmitPlayerCloseToTree(isPlayerClose);
		}
	}


	private void ManuallySortZIndex()
	{
		newZ = (int)treeBody.GlobalPosition.Y;
		if (newZ != treeLayer.ZIndex)
		{
			treeLayer.ZIndex = newZ;
		}
	}

	private void OnFruitTimerTimeout()
	{
		if (fruitQuantity < maxFruitQuantity)
		{
			fruitQuantity++;
			fruitTimer.Start();
		}

	}

	private void DetermineTreeAnimation()
	{
		animationString = fruitQuantity.ToString();
		treeAnim.Play(animationString);

	}

}
