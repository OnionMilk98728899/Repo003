using Godot;
using Game.Ingredients;

public partial class OrderManager : Node
{
    public static OrderManager Instance { get; private set; }
    private Burger _currentOrder;
    private GraphicInterface _myGui;
    private int _goldenCount;
    public override void _EnterTree()
    {
        if (Instance == null) { Instance = this; }
        else { QueueFree(); }
    }

    public override void _Ready()
    {
        //_myGui = GetTree().CurrentScene.GetNode<GraphicInterface>("GUI");
        GlobalSignals.Instance.SceneReady += OnSceneReady;
    }

    private void OnSceneReady(Node scene)
    {
        GD.Print("Called On Scene Ready");
        if (scene.HasNode("GUI"))
        {
            _myGui = scene.GetNode<GraphicInterface>("GUI");
        }
    }
    public void IncreaseGoldenCount(int increase) { _goldenCount += increase; }
    public void ResetGoldenCount() { _goldenCount = 0; }
    public int GetGoldenCount() { return _goldenCount; }
    public Burger GetCurrentOrder() { return _currentOrder; }
    public void SetCurrentOrder(Burger burger)
    {
        _currentOrder = burger;
        _myGui.SetUpOrderWindow(_currentOrder);
    }





}
