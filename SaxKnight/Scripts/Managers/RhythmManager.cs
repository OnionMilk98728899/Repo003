using Godot;
using System;

public partial class RhythmManager : Node
{
    public static RhythmManager Instance { get; private set; }
    [Export] public AudioStreamPlayer2D musicPlayer;
    [Export] public double BPM = 125;

    private double secondsPerBeat;

    private double lastPlaybackPosition = 0;
    private double accumulatedTime = 0;
    public override void _EnterTree()
    {
        Instance = this;
    }


    public override void _Ready()
    {
        secondsPerBeat = 60.0 / BPM;
        OnSceneLoaded();

    }

    public override void _Process(double delta)
    {
        UpdateSongTime();

        double beat = accumulatedTime / secondsPerBeat;
        if (Input.IsActionJustPressed("ui_accept"))
        {
            CheckHit();
        }

        //GD.Print(beat);
    }

    private void OnSceneLoaded()
    {
        musicPlayer = AudioManager.Instance.beatPlayer;
        //AudioManager.Instance.PlayMusic(AudioManager.Instance.beatPlayer, AudioManager.Instance.audioLibrary.beat1, true);
    }

    private void CheckHit()
    {
        // Ensure timing is current
        UpdateSongTime();
        // Distance from perfect timing
        double error = DetectBeatWindow();

        // Timing windows
        if (error < 0.10)
        {
            GD.Print("PERFECT");
        }
        else if (error < 0.20)
        {
            GD.Print("GOOD");
        }
        else if (error < 0.40)
        {
            GD.Print("OK");
        }
        else
        {
            GD.Print("MISS");
        }
    }

    private void UpdateSongTime()
    {
        double currentPos =
            musicPlayer.GetPlaybackPosition()
            + AudioServer.GetTimeSinceLastMix()
            - AudioServer.GetOutputLatency();

        // Detect loop wrap
        if (currentPos < lastPlaybackPosition)
        {
            accumulatedTime += currentPos;
        }
        else
        {
            accumulatedTime += currentPos - lastPlaybackPosition;
        }

        lastPlaybackPosition = currentPos;
        //DetectBeatWindow();
        if(DetectBeatWindow() <= .1)
        {
            GraphicInterface.Instance.LightUpBeatMarkerOnBeat(true);
        }
        else
        {
             GraphicInterface.Instance.LightUpBeatMarkerOnBeat(false);
        }
    }


    private double DetectBeatWindow()
    {
        double currentBeat = accumulatedTime / secondsPerBeat;

        // Fractional distance to nearest beat
        double beatError = Math.Abs(currentBeat - Math.Round(currentBeat));

        // Convert beat error into seconds
        double secondsError = beatError * secondsPerBeat;

        return secondsError;
    }
}
















