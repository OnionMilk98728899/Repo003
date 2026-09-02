using Godot;
using System;

public partial class RangedAttacker : EnemyAttacker
{
    public override Vector2 Attack(Vector2 target, Vector2 velocity)
    {
        return velocity;
    }
}
