using Godot;
using System.Linq;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public partial class NoteSpawner : Node
{

    private Godot.Collections.Array chartData = [];

    [Export] PackedScene notesScene;
    [Export] Node3D noteContainer;
    [Export] Conductor conductor;
    [Export] private float speedMultiplier = 1.0f;

    private Vector3 containerStartPosition;

    public override void _Ready()
    {
        containerStartPosition = noteContainer.Position;
        LoadMIDI();
    }

    public override void _Process(double delta)
    {
        float beatsElapsed = conductor.SongPosition / conductor.SecPerBeat;

        float distance = beatsElapsed / 4f * speedMultiplier;

        noteContainer.Position = new Vector3(
            containerStartPosition.X,
            containerStartPosition.Y,
            containerStartPosition.Z - distance
        );
    }

    private void LoadMIDI()
    {
        byte[] midiBytes = Godot.FileAccess.GetFileAsBytes(
            "res://Charts/Music1.mid"
        );

        using MemoryStream stream = new MemoryStream(midiBytes);

        MidiFile midiFile = MidiFile.Read(stream);

        var tempoMap = midiFile.GetTempoMap();

        var notes = midiFile.GetNotes();

        foreach (var midiNote in notes)
        {
            int pitch = (int)midiNote.NoteNumber;

            
            int lane = pitch - 60;

          
            if (lane < 0 || lane > 3)
                continue;

            double startTime =
                midiNote.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds;

            double duration =
                midiNote.LengthAs<MetricTimeSpan>(tempoMap).TotalSeconds;

            SpawnNote(lane, startTime, duration);
        }
    }


    private void SpawnNote(int lane, double startTime, double duration)
    {
        Node3D noteObject = notesScene.Instantiate<Node3D>();

        noteContainer.AddChild(noteObject);

        float laneSpacing = 0.2f;
        float centerOffset = (4 - 1) * laneSpacing / 2f;

        float beatPosition = (float)(startTime / conductor.SecPerBeat);

        noteObject.Position = new Vector3(
            lane * laneSpacing - centerOffset,
            0,
            -beatPosition * 0.25f * speedMultiplier
        );

        if (noteObject is Note note)
        {
            note.lane = lane;
        }

        GD.Print($"Lane: {lane}, Time: {startTime}, Duration: {duration}");
    }

}
