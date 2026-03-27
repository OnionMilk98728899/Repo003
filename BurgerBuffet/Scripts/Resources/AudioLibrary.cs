using Godot;
using System;

[GlobalClass]
public partial class AudioLibrary : Resource
{
    [Export] public AudioStream turn, collect, collect2, death;
    
}