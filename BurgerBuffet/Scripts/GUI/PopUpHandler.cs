using Godot;
using System;

public partial class PopUpHandler : Control
{
	public static PopUpHandler Instance {get; private set;}

	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}
}
