using Godot;
using System;

public partial class ContactAttacker : EnemyAttacker
{
    private enum attackType{ground, air, jump}
    [Export] private attackType myAttackType;
    public override Vector2 Attack(Vector2 target, Vector2 velocity)
    {
        if (!hasTarget)
        {
            attackTarget = target;
            hasTarget = true;
            attackTimer.Start();
        }

        float xDistance = target.X - GlobalPosition.X;
        xDirection = Mathf.Sign(xDistance);
        float yDistance = target.Y - GlobalPosition.Y;
        yDirection = Mathf.Sign(yDistance);

        if(myAttackType == attackType.air)
        {
            velocity = new Vector2(xDirection * attackSpeed, yDirection * attackSpeed);
        }
        else if(myAttackType == attackType.ground || myAttackType == attackType.jump)
        {
             velocity = new Vector2(xDirection * attackSpeed, velocity.Y);
        }
        
        return velocity;
    }
}
