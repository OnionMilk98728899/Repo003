using Godot;
using System;

public partial class Hud : CanvasLayer
{

    [Export] private RichTextLabel lifeLabel;
    private int dropCount;
    public override void _Ready()
    {
        dropCount = GlobalStats.Instance.playerHealth;
        GlobalStats.Instance.myHud = this;
        lifeLabel.Text = dropCount.ToString();
    }

    public void SetDropCount(int count)
    {
        dropCount = count;
        lifeLabel.Text = dropCount.ToString();
    }

}
