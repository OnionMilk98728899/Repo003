using System.Collections.Generic;
using Game.Ingredients;
using Godot;

public partial class IngredientInventory : Node
{
    public static IngredientInventory Instance { get; private set; }
    //public List<IngredientType> _myIngredients = new List<IngredientType>();
    private int _currentIngredientIndex, _burgerCount;
    private GraphicInterface _myGui;
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
        _myGui = GetTree().CurrentScene.GetNode<GraphicInterface>("GUI");
        GlobalSignals.Instance.RestartGame += ResetInventory;
    }

    private void ResetInventory()
    {
        _currentIngredientIndex = 0;
    }

    public int GetCurrentIngredientIndex()
    {
        return _currentIngredientIndex;
    }

    public void SetCurrentIngredientIndex(int index)
    {
        if (index == 0)
        {
            _currentIngredientIndex = index;
        }
        else
        {
            _currentIngredientIndex += index;
        }
        
    }

    public void AddCollectedItemToInventory(IngredientType ingredient)
    {
        _myGui.AddIngredientToBurgerImage(ingredient);
    }
}