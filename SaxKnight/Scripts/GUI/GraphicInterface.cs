using Godot;
using System;

public partial class GraphicInterface : CanvasLayer
{
	public static GraphicInterface Instance { get; private set;}
	[Export] private Sprite2D beatRingSprite;

	public override void _EnterTree()
	{
		Instance = this;
	}
	public void LightUpBeatMarkerOnBeat(bool islit)
	{
		if (islit)
		{
			beatRingSprite.Frame = 0;
		}
		else
		{
			beatRingSprite.Frame = 1;
		}

	}

}
