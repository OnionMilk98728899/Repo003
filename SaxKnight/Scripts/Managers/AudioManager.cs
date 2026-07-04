using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node2D
{
	public static AudioManager Instance {get; private set;}
	[Export] public AudioStreamPlayer2D beatPlayer, rhythmPlayer;
	[Export] public AudioLibrary audioLibrary;
	private int trackCounter, measureCounter;
	public bool isLoopingCurrentTrack;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		//musicPlayer.Finished += OnMMusicPlayerFinished;
	}

	public void PlaySFX(AudioStreamPlayer2D player, AudioStream audio)
	{
		player.Stream = audio;
		player.Play();
	}

	public void PlayMusic(AudioStreamPlayer2D player, AudioStream audio, bool isaloop)
	{
		isLoopingCurrentTrack = isaloop;
		player.Stream = audio;
		player.Play();
	}

	// public void PlayRandomMusicTrack()
	// {
	// 	AudioStreamPlayer2D player = musicPlayer;
	// 	float rand = GD.Randf();
	// 	 if(rand >= .5)
	// 	{
	// 		player.Stream  = audioLibrary.track1;
	// 	}
	// 	else
	// 	{
	// 		player.Stream = audioLibrary.track2;
	// 	}
	
	// 	player.Play();
	// }

	// private void OnMMusicPlayerFinished()
	// {

		
	// }
	private void OnBeatPlayerFinished()
	{
		if (isLoopingCurrentTrack)
		{
			measureCounter++;
			beatPlayer.Play();
		}
		if(measureCounter >= 2)
		{
			//AudioManager.Instance.PlayMusic(AudioManager.Instance.rhythmPlayer, AudioManager.Instance.audioLibrary.guitarTrack1, true);
		}
	}

	

}


