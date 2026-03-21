using System.Collections.Generic;
using Godot;
public partial class ItemInventory : Node
{
    public static ItemInventory Instance { get; private set;}
    public List<(int,int)> _occupiedSquares = new List<(int,int)>();
    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    public override void _Ready()
    {
        GlobalSignals.Instance.GameOver += ResetOccupiedSquares;
    }

    private void ResetOccupiedSquares()
    {
        _occupiedSquares.Clear();
    }
}