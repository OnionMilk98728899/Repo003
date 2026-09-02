using Godot;
using System;

public partial class GlobalStats : Node2D
{
    public static GlobalStats Instance { get; private set; }
    public Hud myHud;
    [Export] public int playerHealth, maxPlayerHealth;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public void SetPlayerHealth(int health)
    {
        playerHealth += health;
        myHud.SetDropCount(playerHealth);
        GD.Print("Stats set");
    }

    public void ZeroPlayerHealth()
    {
        playerHealth = 0;
        myHud.SetDropCount(playerHealth);
    }
    
}
