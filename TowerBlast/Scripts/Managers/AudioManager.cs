using Godot;
using System;
using Game.Upgrades;
using System.Collections.Generic;

public partial class AudioManager : Node2D
{
	public static AudioManager Instance {get; private set;}
	//[Export] public AudioStreamPlayer[] _sfxPlayers;
	[Export] public AudioStreamPlayer2D _musicPlayer, _sfxPlayer;
	[Export] public AudioLibrary _audioLibrary;
	public override void _Ready()
	{
		Instance = this;
	}

	public void PlaySFX(AudioStream audio)
	{
		AudioStreamPlayer2D player = _sfxPlayer;

		player.Stream = audio;
		player.Play();
	}

	public void PlayWeaponFX(WeaponType weapon)
	{
		switch (weapon)
		{
			case WeaponType.sword:
			break;
			case WeaponType.hammer:
			break;
			case WeaponType.axe:
			break;
			case WeaponType.dagger:
			break;
			case WeaponType.spear:
			break;
			case WeaponType.club:
			break;
			case WeaponType.bomb:
			break;
			case WeaponType.scepter:
			break;
			case WeaponType.bow:
			break;
			case WeaponType.shield:
			break;
			case WeaponType.potion:
			break;
			case WeaponType.shuriken:
			break;
		}
	}

	public void PlayTrapFX(TrapType trap)
	{
		switch (trap)
		{
			case TrapType.ballista:
			break;
			case TrapType.spear:
			break;
			case TrapType.shuriken:
			break;
			case TrapType.cannon:
			break;
			case TrapType.bigcannon:
			break;
			case TrapType.tricannon:
			break;
			case TrapType.spikes:
			break;
			case TrapType.firetile:
			break;
			case TrapType.icetile:
			break;
			case TrapType.poisontile:
			break;
			case TrapType.lightningtile:
			break;
			case TrapType.magic:
			break;
			case TrapType.fireorb:
			break;
			case TrapType.iceorb:
			break;
			case TrapType.poisonorb:
			break;
			case TrapType.lightningorb:
			break;
			case TrapType.swarm:
			break;
			case TrapType.bugswarm:
			break;
			case TrapType.ratswarm:
			break;
			case TrapType.treasure:
			break;

		}
	}

}
