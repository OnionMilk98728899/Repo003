using Godot;
using System;

public partial class GlobalStats : Node2D
{
    public static GlobalStats Instance { get; private set; }
    public Hud myHud;
    [Export] public int playerHealth, maxPlayerHealth;
    private Player myPlayer;
    public TileMap myMap;
    public Vector2 playerLocation;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public void InitializeTilemapReference(TileMap tilemap)
    {
        myMap = tilemap;
    }

    public void InitializePlayerReference(Player player)
    {
        myPlayer = player;
    }

    public override void _PhysicsProcess(double delta)
    {
        playerLocation = myPlayer.GlobalPosition;
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
