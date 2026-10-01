using Godot;
using System;

public partial class EnemyAttacker : Node2D
{
    [Signal] public delegate void AttackFinishedEventHandler();
    [Export] public Timer attackTimer;
    [Export] public float  attackSpeed;
    public Vector2 attackTarget;
    public float xDirection, yDirection;
    public bool hasTarget, hasAttacked;
    public virtual Vector2 Attack(Vector2 target, Vector2 velocity)
    {

        return velocity;
    }

    private void OnAttackTimerTimeout()
    {
        hasAttacked = false;
        hasTarget = false;
        EmitSignal(SignalName.AttackFinished);
    }
}
