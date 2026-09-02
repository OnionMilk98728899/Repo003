using Godot;
using System;

public enum projectileType {spear, ball}
public partial class EnemyProjectile : CharacterBody2D
{
    [Export] private Sprite2D projSprite;
    private Vector2 target;

}
