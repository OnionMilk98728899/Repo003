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

    private void ResetInventory(){
        _currentIngredientIndex = 0;
    }

    public void AddCollectedItemToInventory(IngredientType ingredient)
    {
        if(OrderManager.Instance.GetCurrentOrder().ingredients[_currentIngredientIndex] == ingredient)
        {
            //GD.Print($"GREAT! you collected an {OrderManager.Instance.GetCurrentOrder().ingredients[_currentIngredientIndex]}");
            _myIngredients.Add(ingredient);
            _currentIngredientIndex ++;
            _myGui.AddIngredientToBurgerImage(ingredient);

            if(_currentIngredientIndex == OrderManager.Instance.GetCurrentOrder().ingredients.Length)
            {
                //GD.Print("\n\nORDER CLEARED!!!!!!\n\n");
                _currentIngredientIndex = 0;
                _myGui.StartBurgerImageWipe();
            }
        }
        else
        {
            // GD.PrintErr($"Wrong Ingredient! You collected a {ingredient},\nbut we needed a "
            // +$"{OrderManager.Instance.GetCurrentOrder().ingredients[_currentIngredientIndex]}, ({_currentIngredientIndex})");
        }

       
        
    }
}