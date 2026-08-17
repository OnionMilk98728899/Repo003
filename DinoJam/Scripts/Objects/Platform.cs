using Godot;
using System;

public partial class Platform : AnimatableBody2D
{
    [Export] private Node2D startPoint, endPoint;
    [Export] private float moveTime;

    private Vector2 startPosition, endPosition;

    public override void _Ready()
    {
        startPosition = startPoint.GlobalPosition;
        endPosition = endPoint.GlobalPosition;
        MovePlatform();
    }

    private void MovePlatform()
    {
        Tween tween = CreateTween();

        tween.SetLoops();

        tween.TweenProperty(
            this,
            "global_position",
            endPosition,
            moveTime
        );

        tween.TweenProperty(
            this,
            "global_position",
            startPosition,
            moveTime
        );
    }

}
