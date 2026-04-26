using Godot;
using System;

[GlobalClass]
public partial class AudioLibrary : Resource
{
    [Export] public AudioStream track1, track2, introTrack, onionScreen, godotScreen, deathTheme, turn, jump, collect, collect2, death, badCollect, 
    enemyConverted, ingredLand, meanieNoise, bottomBunDrop, lettuceDrop, pattyDrop, cheeseDrop, tomatoDrop, onionDrop, picklesDrop, sauceDrop, 
    topBunDrop, meanieKill, meanieDrop, timeTick, coinTick, chefKiss, burgerFlames, deathCrash, knockout, dizzy;
    
}