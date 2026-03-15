using Godot;
using System;
using System.Diagnostics.Tracing;

public partial class HatManager : Node2D
{
    [Signal] public delegate void HatChangedEventHandler();
    public Headgear.HeadgearType currentHat;
    [Export] private Texture2D hardHatText, vikingText, bikeHelmText, sunhatText, snorkelText, winterhatText, firehatText, visorText;
    private Texture2D currentTexture;
    private Sprite2D hatSprite;
    //private int hatIndex;
    private Headgear headGear;

    public override void _Ready()
    {
        hatSprite = GetNode<Sprite2D>("HatSprite");
        headGear = GetNode<Headgear>("/root/Headgear");
        SwitchHat(Headgear.HeadgearType.None);
    }


    public void SwitchHat(Headgear.HeadgearType thisHat){

        switch(thisHat){

            case Headgear.HeadgearType.None:
            currentTexture = null;
            currentHat = Headgear.HeadgearType.None;
            //hatIndex = 0;
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.None);
            break;
            case Headgear.HeadgearType.Hardhat:
            currentTexture = hardHatText;
            currentHat = Headgear.HeadgearType.Hardhat; 
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.Hardhat);    
            //hatIndex = 1;
            break;
            case Headgear.HeadgearType.Viking:
            currentTexture = vikingText;
            currentHat = Headgear.HeadgearType.Viking;
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.Viking);
            //hatIndex = 2;
            break;
            case Headgear.HeadgearType.BikeHelm:
            currentTexture = bikeHelmText;
            currentHat = Headgear.HeadgearType.BikeHelm;
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.BikeHelm);
            //hatIndex = 3;
            break;
            case Headgear.HeadgearType.Sunhat:
            currentTexture = sunhatText;
            currentHat = Headgear.HeadgearType.Sunhat;
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.Sunhat);
            //hatIndex = 4;
            break;
            case Headgear.HeadgearType.Snorkel:
            currentTexture = snorkelText;
            currentHat = Headgear.HeadgearType.Snorkel;
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.Snorkel);
            //hatIndex = 5;
            break;
            case Headgear.HeadgearType.Visor:
            currentTexture = visorText;
            currentHat = Headgear.HeadgearType.Visor;
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.Visor);
            //hatIndex = 6;
            break;
            case Headgear.HeadgearType.Firehat:
            currentTexture = firehatText;   
            currentHat = Headgear.HeadgearType.Firehat;
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.Firehat);
            //hatIndex = 7;
            break;
            case Headgear.HeadgearType.Winterhat:
            currentTexture = winterhatText; 
            currentHat = Headgear.HeadgearType.Winterhat;
                headGear.SetCurrentHeadgear(Headgear.HeadgearType.Winterhat);
            //hatIndex = 8;
            break;
            
        }
 
        hatSprite.Texture = currentTexture;
        EmitSignal("HatChanged");
    }

    private void OnHatCollected(int hatIndex){

        switch(hatIndex){
            case 1:
            SwitchHat(Headgear.HeadgearType.BikeHelm);
            break;
            case 2:
            SwitchHat(Headgear.HeadgearType.Firehat);
            break;
            case 3:
            SwitchHat(Headgear.HeadgearType.Hardhat);
            break;
            case 4:
            SwitchHat(Headgear.HeadgearType.Snorkel);
            break;
            case 5:
            SwitchHat(Headgear.HeadgearType.Sunhat);
            break;
            case 6:
            SwitchHat(Headgear.HeadgearType.Viking);
            break;
            case 7:
            SwitchHat(Headgear.HeadgearType.Visor);
            break;
            case 8:
            SwitchHat(Headgear.HeadgearType.Winterhat);
            break;
        }
    }
}
