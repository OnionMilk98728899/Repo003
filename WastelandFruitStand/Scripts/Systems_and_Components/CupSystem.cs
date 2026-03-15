using Godot;
using System;
using System.Collections.Generic;


public partial class CupSystem : Control
{
	private PlayerInventory playerInventory;
	private PlayerActionController playerAction;
	[Export] private PackedScene cupScene;
	[Export] public Timer cupBuffer;
	private Vector2 cupPlacementPos;
	private Cup myCup;
	private int myIndex, selectedCupIndex = 0;
	private List<Cup> cupList;
	private bool cupSelected;
	public bool pouringActive;
	private float dirtiness;

	public override void _Ready()
	{
		cupList = new List<Cup>();
		playerInventory = GetNode<PlayerInventory>("/root/TownScene/PlayerPackage/PlayerCharacter/PlayerInventory");
		playerAction = GetNode<PlayerActionController>("/root/TownScene/PlayerPackage/PlayerCharacter/PlayerActionController");

	}
	public override void _PhysicsProcess(double delta)
	{
		HandleDirectionInput();
		if (pouringActive && playerAction.GetAction5Status())
		{
			pouringActive = false;
			DeselectAllCups();
		}

	}
	public bool GetCupSelected()
	{
		return cupSelected;
	}

	public void SetACup()
	{
		if (playerInventory.GetTotalCupsQuantity() > 0)
		{
			if (cupList.Count <= 2)
			{
				myCup = cupScene.Instantiate<Cup>();
				
				myCup.DetermineCupType(playerInventory.TakeNextBestCup());
				myCup.placementIndex = cupList.Count;
				myCup.GlobalPosition = DetermineCupPlacement();

				cupList.Add(myCup);
				CallDeferred("add_child", myCup);
			}
			else
			{
				GD.PrintErr("NO SPACE to place CUP!");
			}

		}
		else
		{
			GD.PrintErr("NO CUPS!");
		}
	}

	public Cup GetNextCupToFill()
	{
		myCup = null;
		pouringActive = true;

		int index = 0;
		GD.Print("SelectedIndex  = " + selectedCupIndex);
		foreach (Cup cup in GetChildren())
		{

			if (index == selectedCupIndex)
			{
				myCup = cup;
				cupSelected = true;
			}
			else
			{
				cup.DeselectCup();
			}
			index++;
		}

		return myCup;
	}

	private void HandleDirectionInput()
	{
		if (cupList.Count > 0 && pouringActive && cupBuffer.IsStopped())
		{
			if (Input.IsActionJustPressed("ui_left"))
			{
				GD.Print("CupList count is  " + cupList.Count + " and total cups in Children is " + GetChildren().Count);
				if (selectedCupIndex > 0)
				{
					selectedCupIndex--;
					GD.Print("SelectedIndex is " + selectedCupIndex);
				}
				else
				{
					selectedCupIndex = cupList.Count - 1;
					GD.Print("SelectedIndex is " + selectedCupIndex);
				}
				GetNextCupToFill().SelectCup();

			}
			if (Input.IsActionJustPressed("ui_right"))
			{
				if (selectedCupIndex < cupList.Count - 1)
				{
					selectedCupIndex++;
					GD.Print("SelectedIndex is " + selectedCupIndex);
				}
				else
				{
					selectedCupIndex = 0;
					GD.Print("SelectedIndex is " + selectedCupIndex);
				}
				GetNextCupToFill().SelectCup();

			}
		}
	}

	private Vector2 DetermineCupPlacement()
	{
		
		switch (cupList.Count)
		{
			case 0:
				cupPlacementPos = new Vector2(12, -10);
				break;
			case 1:
				cupPlacementPos = new Vector2(60, -10);
				break;
			case 2:
				cupPlacementPos = new Vector2(108, -10);
				break;
		}
		return cupPlacementPos;
	}

	public void RemoveCupFromList()
	{
		ReshuffleCupPlacement();
		cupList.Remove(cupList[selectedCupIndex]);
	}

	private void ReshuffleCupPlacement()
	{
		if (selectedCupIndex < cupList.Count)
		{
			if (selectedCupIndex == 0)
			{
				//Move Cups 1 and 2
				foreach (Cup cup in GetChildren())
				{
					if (cup.placementIndex == 1)
					{
						cup.Position = new Vector2(12, -10);
						cup.placementIndex = 0;
					}
					if (cup.placementIndex == 2)
					{
						cup.Position = new Vector2(60, -10);
						cup.placementIndex = 1;
					} 
				}
			}
			if (selectedCupIndex == 1)
			{
				//Move Cup 2
				foreach (Cup cup in GetChildren())
				{
					if (cup.placementIndex == 2)
					{
						cup.Position = new Vector2(60, -10);
						cup.placementIndex = 1;
					}
				}
			}
		}
	}

	public int GetCupCountOnTable()
	{
		return cupList.Count;
	}

	public void ResetSelectedCupIndex()
	{
		cupBuffer.Start();
	}

	public void DeselectAllCups()
	{
		foreach (Cup cup in GetChildren())
		{
			cup.DeselectCup();
		}

	}

	private void OnCupBufferTimeout()
	{
		if (selectedCupIndex > 0)
		{
			selectedCupIndex--;
		}
		else
		{
			selectedCupIndex++;
		}
		if (cupList.Count <= 1)
		{
			selectedCupIndex = 0;
		}
		cupList[selectedCupIndex].SelectCup();
		GD.Print("CupList count is  " + cupList.Count + " and SelectedIndex is " + selectedCupIndex);
	}
	

}
