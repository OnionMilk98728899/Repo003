using Godot;
using System;

public partial class GlobalSignals : Node
{
    public static GlobalSignals Instance { get; private set; }
    [Signal] public delegate void ActivateFruitStandEventHandler(bool active);
    [Signal] public delegate void SellCupOfJuiceEventHandler();
    [Signal] public delegate void PlayerCloseToTreeEventHandler(bool isClose);
    [Signal] public delegate void OverfilledCupEventHandler(int overfill);

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
    public void EmitActivateFruitStand(bool isActive)
    {
        EmitSignal(SignalName.ActivateFruitStand, isActive);
    }
    public void EmitSellCupOfJuice()
    {
        EmitSignal(SignalName.SellCupOfJuice);
        GD.Print("Selling Cup of Juice");
    }

    public void EmitPlayerCloseToTree(bool close)
    {
        EmitSignal(SignalName.PlayerCloseToTree, close);
    }

    public void EmitOverfilledCup(int over)
    {
        EmitSignal(SignalName.OverfilledCup, over);
    }
}
