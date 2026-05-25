using Godot;
using System;

public partial class BasicEnemyMovement : Node2D
{
    [Export] private CharacterBody2D enemyBody;
    [Export] private AnimationPlayer enemyAnim;
    [Export] private Sprite2D enemySprite;
    [Export] private float speed, maxSpeed;
    private Vector2 enemyVelocity;

    public override void _Ready()
    {
    
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }

    private void ApplyMovement()
    {
        
    }

}
