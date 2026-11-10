using Godot;

public partial class PlayerController : Node
{
    [Export] Node3D player;
    [Export] Vector3 leftPos;
    [Export] Vector3 rightPos;

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Shift"))
        {
            Shift();
        }
    }


    private void Shift()
    {
        if (player.Position == leftPos)
        {
            player.Position = rightPos;
        }
        else
        {
            player.Position = leftPos;
        }
    }
}
