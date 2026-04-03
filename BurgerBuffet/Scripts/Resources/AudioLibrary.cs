using Godot;
using System;

[GlobalClass]
public partial class AudioLibrary : Resource
{
    [Export] public AudioStream mainTheme, deathTheme, turn, jump, collect, collect2, death, badCollect, enemyConverted;
    
}