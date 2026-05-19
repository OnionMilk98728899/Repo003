using Godot;
using System;

public partial class RhythmManager : Node
{
    [Export] public AudioStreamPlayer MusicPlayer;
    [Export] public double BPM = 120.0;

    private double secondsPerBeat;

    public override void _Ready()
    {
        secondsPerBeat = 60.0 / BPM;

        MusicPlayer.Play();
    }

    public override void _Process(double delta)
    {
        double songTime = GetSongTime();

        // Current beat position as decimal
        double beat = songTime / secondsPerBeat;

        // Example:
        // beat 4.25 means we're 25% into beat 4
        GD.Print($"Beat Position: {beat}");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept"))
        {
            CheckHit();
        }
    }

    private void CheckHit()
    {
        double songTime = GetSongTime();

        double currentBeat = songTime / secondsPerBeat;

        // Nearest whole beat
        double nearestBeat = Math.Round(currentBeat);

        // Distance from exact beat
        double error = Math.Abs(currentBeat - nearestBeat);

        // Timing windows
        if (error < 0.05)
        {
            GD.Print("PERFECT");
        }
        else if (error < 0.10)
        {
            GD.Print("GOOD");
        }
        else if (error < 0.20)
        {
            GD.Print("OK");
        }
        else
        {
            GD.Print("MISS");
        }
    }

    private double GetSongTime()
    {
        return MusicPlayer.GetPlaybackPosition()
            + AudioServer.GetTimeSinceLastMix()
            - AudioServer.GetOutputLatency();
    }
}
