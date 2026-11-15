using Godot;

public partial class Conductor : AudioStreamPlayer3D
{
    [Export] int bpm = 100;
    [Export] int measures = 4;
    [Export] Timer startTimer;

    float songPosition = 0.0f;
    int songPosInBeats = 1;
    float secPerBeat;
    int lastReportedBeat = 0;
    int beatsBeforeStart = 0;
    int measure = 1;

    int closest = 0;
    float timeOffBeat = 0.0f;

    public float SongPosition => songPosition;
    public float SecPerBeat => secPerBeat;

    [Signal]
    public delegate void BeatEventHandler(int position);

    [Signal]
    public delegate void MeasureEventHandler(int position);

    public override void _Ready()
    {
        secPerBeat = 60.0f / bpm;
        StartSong();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Playing)
        {
            songPosition = GetPlaybackPosition() + (float)AudioServer.GetTimeSinceLastMix();
            songPosition -= (float)AudioServer.GetOutputLatency();
            songPosInBeats = (int)Mathf.Floor(songPosition / secPerBeat) + beatsBeforeStart;
            ReportBeat();
        }
    }

    private void StartSong()
    {
        Stop();

        songPosition = 0.0f;
        songPosInBeats = 1;
        lastReportedBeat = 0;
        beatsBeforeStart = 1;
        measure = 1;

        Seek(0);
        Play();
    }

    private void ReportBeat()
    {
        if (lastReportedBeat < songPosInBeats)
        {
            if (measure > measures)
                measure = 1;

            EmitSignal(SignalName.Beat, songPosInBeats);
            EmitSignal(SignalName.Measure, measure);
            lastReportedBeat = songPosInBeats;
            measure += 1;
            
        }
    }

    private void PlayWithBeatOffset(int num)
    {
        beatsBeforeStart = num;
        startTimer.WaitTime = secPerBeat;
        startTimer.Start();
    }

    private Vector2 ClosestBeat(int nth)
    {
        closest = (int)(Mathf.Round((songPosition / secPerBeat) / nth) * nth);
        timeOffBeat = Mathf.Abs(closest * secPerBeat - songPosition);
        return new Vector2(closest, timeOffBeat);
    }


    private void PlayFromBeat(int beat, int offset)
    {
        Play();
        Seek(beat * secPerBeat);
        beatsBeforeStart = offset;
        measure = beat % measures;
    }

    private void OnStartTimerTimeout()
    {
        songPosInBeats += 1;
        if (songPosInBeats < beatsBeforeStart - 1)
            startTimer.Start();
        else if (songPosInBeats == beatsBeforeStart - 1)
        {
            startTimer.WaitTime = startTimer.WaitTime - (AudioServer.GetTimeToNextMix() + AudioServer.GetOutputLatency());
            startTimer.Start();
        }
        else
        {
            Play();
            startTimer.Stop();
        }
        ReportBeat();
    }
}
