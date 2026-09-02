using Godot;
using System;

public partial class EnemyAttacker : Node2D
{
    [Signal] public delegate void AttackFinishedEventHandler();
    [Export] public Timer attackTimer;
    [Export] public float  attackSpeed;
    public Vector2 attackTarget;
    public float direction;
    public bool hasTarget;
    public virtual Vector2 Attack(Vector2 target, Vector2 velocity)
    {

        return velocity;
    }

    private void OnAttackTimerTimeout()
    {
        hasTarget = false;
        EmitSignal(SignalName.AttackFinished);
    }
}
