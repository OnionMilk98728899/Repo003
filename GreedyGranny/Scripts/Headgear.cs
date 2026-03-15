using Godot;
using System;

public partial class Headgear : Node
{
    public static Headgear Instance { get; set; }
    public enum HeadgearType{ 

        None, BikeHelm, Hardhat, Firehat, Snorkel, Sunhat, Viking, Visor, Winterhat
    }

    public HeadgearType currentHeadgear { get; set;}

    public void SetCurrentHeadgear(HeadgearType myType){
        currentHeadgear = myType;
    }

}
