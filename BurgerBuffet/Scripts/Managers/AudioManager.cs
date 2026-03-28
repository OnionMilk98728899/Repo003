using Godot;
using System;

public partial class AudioManager : Node2D
{
	public static AudioManager Instance {get; private set;}
	[Export] public AudioStreamPlayer2D _musicPlayer, _sfxPlayer;
	[Export] public AudioLibrary _audioLibrary;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public void PlaySFX(AudioStream audio)
	{
		AudioStreamPlayer2D player = _sfxPlayer;

		player.Stream = audio;
		player.Play();
	}

}
