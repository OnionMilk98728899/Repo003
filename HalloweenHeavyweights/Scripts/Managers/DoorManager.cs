using Godot;
using System;
using System.Collections.Generic;

public partial class DoorManager : Node2D
{
	private List<Door_2D> doorsOnFloor;

	public void CreateNewDoorList(){
		doorsOnFloor = new List<Door_2D>();
	}

	public void AddDoorToDoorList(Door_2D myDoor)
	{
		doorsOnFloor.Add(myDoor);
	}

	public void ClearDoorList(){
		foreach(Door_2D door in doorsOnFloor){
			door.QueueFree();
		}
	}

}
