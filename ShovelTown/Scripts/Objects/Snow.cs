using Godot;
using System;

public partial class Snow : Node2D
{
    [Export] private Sprite2D snowSprite;
    [Export] private int depthCoeff;


    public override void _Ready()
    {
        SetSnowVisualDepth();
    }


    public void ScoopSnow(int power)
    {
        depthCoeff -= power;
        SetSnowVisualDepth();
    }

    private void SetSnowVisualDepth()
    {
        
        if (depthCoeff <= 0)
        {
            snowSprite.Frame = 26;
        }
        else
        {
            snowSprite.Frame = (depthCoeff * 2) - 1;
        }
    }



}
