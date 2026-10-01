using Godot;
using System;

public partial class ElementalTile : StaticBody2D
{
    public enum tileType{vines, liteSnow, ice, poisonGas, gasLeft, gasRight, gasTop, gasBottom, lavaJet, lavaPool, lavaRiverLeft, lavaRiverRight, lavaFalls}
    [Export] private Texture2D vinesTexture, liteSnowTexture, iceTexture, poisonGasTexture, gasLeftTexture, gasRightTexture, gasTopTexture, gasBottomTexture,
    lavaJetTexture, lavaPoolTexture, lavaRiverLeftTexture, lavaRiverRightTexture, lavaFallsTexture;
    [Export] private tileType myTileType;
    [Export] private StaticBody2D myBody;


    public void SetTileType(int type)
    {
        myTileType = (tileType)type;
    }

    private void SetTilePreferences()
    {
        switch (myTileType)
        {
            case tileType.liteSnow:
            break;
            case tileType.vines:
            break;
            case tileType.lavaPool:
            break;
            case tileType.poisonGas:
            break;
            case tileType.ice:
            break;
        }
    }
    private void OnElementDetectorBodyEntered(Node2D body)
    {
        if(body is Edible edible)
        {
            if(edible.myEdibleType == Edible.edibleType.water && myTileType == tileType.lavaJet)
            {
                
            }else if(edible.myEdibleType == Edible.edibleType.water && myTileType == tileType.lavaPool)
            {
                
            }else if(edible.myEdibleType == Edible.edibleType.water && myTileType == tileType.ice)
            {
                
            }else if (edible.myEdibleType == Edible.edibleType.fire && myTileType == tileType.liteSnow)
            {
                
            }else if(edible.myEdibleType == Edible.edibleType.fire && myTileType == tileType.vines)
            {
                
            }else if(edible.myEdibleType == Edible.edibleType.air && myTileType == tileType.poisonGas)
            {
                
            }
        }
    }
}
