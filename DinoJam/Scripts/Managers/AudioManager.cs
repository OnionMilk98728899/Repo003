using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node2D
{
	public static AudioManager Instance {get; private set;}
	[Export] public AudioStreamPlayer2D musicPlayer, sfx1Player,  sfx2Player, enemySFX1;
	[Export] public AudioLibrary audioLibrary;
	private int trackCounter;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		musicPlayer.Finished += OnMMusicPlayerFinished;
       // PlayMusic(audioLibrary.musicLevel1);
	}

	public void PlaySFX(AudioStreamPlayer2D player, AudioStream audio)
	{
		player.Stream = audio;
		player.Play();
	}

	public void PlayMusic(AudioStream audio)
	{
		AudioStreamPlayer2D player = musicPlayer;
		player.Stream = audio;
		player.Play();
	}

	public void PlayRandomMusicTrack()
	{
		AudioStreamPlayer2D player = musicPlayer;
	
		//player.Play();
	}

	private void OnMMusicPlayerFinished()
	{

		
	}


	////////////////////////////////TEST LOGIC

	

}

