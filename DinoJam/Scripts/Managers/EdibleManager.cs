using Godot;
using System;


public partial class EdibleManager : Node2D
{
    public static EdibleManager Instance {get; private set;}

    public override void _EnterTree()
    {
        Instance = this;  
    }

}
