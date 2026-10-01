using Godot;
using System;

public partial class SpinDeath : EnemyDeath
{
    [Export] private CharacterBody2D enemyBody;
    public override Vector2 Death(Vector2 velocity)
    {
        enemyBody.Rotation += .001f * velocity.X;
        enemyBody.SetCollisionMaskValue(1, false);
        velocity.Y += gravity;
        if(velocity.Y > maxGravity)
        {
            velocity.Y = maxGravity;
        }
        return velocity;
    }
}