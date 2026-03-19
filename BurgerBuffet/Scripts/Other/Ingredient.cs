namespace Game.Ingredients
{

    public struct Burger
    {
        public IngredientType[] ingredients;
    }

    public enum IngredientType
    {
        bottomBun, lettuce, patty, cheese, onion, tomato, pickles, sauce, topBun
    }
}