using Godot;
using Game.Ingredients;

public partial class OrderManager : Node
{
    public static OrderManager Instance {get; private set;}
    private Burger _currentOrder;
    private Gui _myGui;
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
        _myGui = GetTree().CurrentScene.GetNode<Gui>("GUI");
    }

    public void SetCurrentOrder(Burger burger)
    {
        _currentOrder = burger;
        _myGui.SetUpOrderWindow(_currentOrder);
    }

    public Burger GetCurrentOrder()
    {
        return _currentOrder;
    }

}