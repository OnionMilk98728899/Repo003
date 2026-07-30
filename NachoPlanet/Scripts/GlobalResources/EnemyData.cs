using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public float moveSpeed { get; set; }
    [Export] public int maxHealth { get; set; }
    [Export] public float attackAggression { get; set; }
    [Export] public float attackDamage { get; set; }
    [Export] public float attackRange { get; set; }
     [Export] public float spriteFrames { get; set; }
    [Export] public Texture2D spriteTexture { get; set; }
}