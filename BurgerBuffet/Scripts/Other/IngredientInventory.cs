using System.Collections.Generic;
using Game.Ingredients;
using Godot;

public partial class IngredientInventory : Node
{
    public static IngredientInventory Instance { get; private set; }
    public List<IngredientType> _myIngredients = new List<IngredientType>();
    private int _currentIngredientIndex;
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
        GlobalSignals.Instance.GameOver += ResetInventory;
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
        _currentIngredientIndex = index;
    }

    public void AddCollectedItemToInventory(IngredientType ingredient)
    {
        if (_currentIngredientIndex <= OrderManager.Instance.GetCurrentOrder().ingredients.Length -1)
        {
            if (OrderManager.Instance.GetCurrentOrder().ingredients[_currentIngredientIndex] == ingredient)
            {
                _myIngredients.Add(ingredient);
                _currentIngredientIndex++;
                _myGui.AddIngredientToBurgerImage(ingredient);

                if (_currentIngredientIndex == OrderManager.Instance.GetCurrentOrder().ingredients.Length)
                {
                    _myGui.StartBurgerImageWipe();
                    _myGui.IncreaseBurgerCount();
                }
            }
            else
            {
                
            }
        }
    }
}