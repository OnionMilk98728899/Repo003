using System.Collections.Generic;
//using System.Numerics;
using Godot;

namespace Game.ProjectileStats

{
    public struct ProjectileStats
    {
        public ProjectileType projType;
        public int damage;
        public int speed;
        public Vector2 target;

        //private Dictionary<IngredientType, bool> _goldenList;
    }

    public enum ProjectileType
    {
       bullet, rocket, grenade, spray
    }

  
}