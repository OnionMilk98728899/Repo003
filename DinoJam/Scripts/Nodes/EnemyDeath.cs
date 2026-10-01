using Godot;
using System;

public partial class EnemyDeath : Node2D
{
    public float gravity, maxGravity;
    [Export] public Sprite2D enemySprite;
    public virtual Vector2 Death(Vector2 velocity)
    {
        return velocity;
    }

    public void SetGravity(float grav, float maxGrav)
    {
        gravity = grav;
        maxGravity = maxGrav;
    }
}