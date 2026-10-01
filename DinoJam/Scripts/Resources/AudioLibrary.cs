using Godot;
using System;

[GlobalClass]
public partial class AudioLibrary : Resource
{
    [Export] public AudioStream musicLevel1, playerJump, playerLand, playerEat, playerCollide, playerHurt, playerDie, playerSpit, playerDigest, 
    playerChargeStep, switchFlip, enemyHurt, enemyDie;
    
}