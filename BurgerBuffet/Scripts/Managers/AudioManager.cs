using Godot;
using System;

public partial class AudioManager : Node2D
{
	public static AudioManager Instance {get; private set;}
	[Export] public AudioStreamPlayer2D _musicPlayer, _sfxPlayer,  _sfx2Player, _burgerImagePlayer, _meanieSFX, _timerFX, _scoreSFX, _burgerFlamesFX, 
	_playerDizzyFX;
	[Export] public AudioLibrary _audioLibrary;
	private int _trackCounter;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		
	}

	public void PlaySFX(AudioStreamPlayer2D player, AudioStream audio)
	{
		player.Stream = audio;
		player.Play();
	}

	public void PlayMusic(AudioStream audio)
	{
		AudioStreamPlayer2D player = _musicPlayer;
		player.Stream = audio;
		player.Play();
	}

	public void PlayRandomMusicTrack()
	{
		AudioStreamPlayer2D player = _musicPlayer;
		float rand = GD.Randf();
		 if(rand >= .5)
		{
			player.Stream  = _audioLibrary.track1;
		}
		else
		{
			player.Stream = _audioLibrary.track2;
		}
		player.Play();
	}


}
