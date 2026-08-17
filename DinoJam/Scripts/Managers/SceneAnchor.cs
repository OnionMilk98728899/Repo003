using Godot;
using System;

public partial class SceneAnchor : Node2D
{
    [Export] private Node2D[] sceneAnchorPoints;
    private int anchorCounter;

    public override void _Ready()
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.SetCameraAnchors, sceneAnchorPoints);
    }



}
