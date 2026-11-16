using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
    public static GameManager instance { get; private set; }

    [Export] public Area3D perfectTrigger;

    private List<Note> perfectObjects;

    public override void _Ready()
    {
        instance = this;

        perfectObjects = new List<Note>();

        perfectTrigger.AreaEntered += OnPerfectTriggerEntered;
        perfectTrigger.AreaExited += OnPerfectTriggerExited;
    }

    public void CheckLaneHit(int lane)
    {
        GD.Print("Checking Lane Objects");
        foreach (Note perfectObject in perfectObjects)
        {
            if (lane == perfectObject.lane)
            {
                perfectObject.QueueFree();
                
            }
        }
    }

    private void OnPerfectTriggerEntered(Area3D area)
    {
        if (area.GetParent() is Note note)
        {
            perfectObjects.Add(note);
        }
    }

    private void OnPerfectTriggerExited(Area3D area)
    {
        if (area.GetParent() is Note note)
        {
            perfectObjects.Remove(note);
        }
    }

}
