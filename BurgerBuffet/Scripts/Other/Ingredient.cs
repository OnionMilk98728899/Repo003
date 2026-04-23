using System.Collections.Generic;

namespace Game.Ingredients

{

    public struct Burger
    {
        public IngredientType[] ingredients;
        //private Dictionary<IngredientType, bool> _goldenList;
    }

    public enum IngredientType
    {
        bottomBun, lettuce, patty, cheese, onion, tomato, pickles, sauce, topBun
    }

  
}