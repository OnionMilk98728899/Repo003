using Godot;
using System;

public partial class ContactAttacker : EnemyAttacker
{
    public override Vector2 Attack(Vector2 target, Vector2 velocity)
    {
        if (!hasTarget)
        {
            attackTarget = target;
            hasTarget = true;
            attackTimer.Start();
        }

        float distance = target.X - GlobalPosition.X;
        direction = Mathf.Sign(distance);
        velocity = new Vector2(direction * attackSpeed, 0);
        return velocity;
    }
}
