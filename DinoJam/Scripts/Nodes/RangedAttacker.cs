using Godot;
using System;

public partial class RangedAttacker : EnemyAttacker
{
    [Export] private PackedScene projectileScene;
    [Export] private int projectileCount;
    [Export] private float projSpeed;
    private EnemyProjectile myProjectile;
    public override Vector2 Attack(Vector2 target, Vector2 velocity)
    {
        if (!hasAttacked)
        {
            attackTimer.Start();
            for (int i = 0; i < projectileCount; i++)
            {
                
                myProjectile = projectileScene.Instantiate<EnemyProjectile>();
                ProjectileManager.Instance.AddChild(myProjectile);
                myProjectile.GlobalPosition = GlobalPosition;
                myProjectile.SetStats(target, projSpeed);            
            }
            hasAttacked = true;
        }
        
        velocity/=2;
        return velocity;
    }
}
