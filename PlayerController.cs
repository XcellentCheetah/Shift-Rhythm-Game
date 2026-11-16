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

        if (@event.IsActionPressed("Lane0"))
        {
            GD.Print("YESSIR");
            GameManager.instance.CheckLaneHit(0);
        }

        if (@event.IsActionPressed("Lane1"))
            GameManager.instance.CheckLaneHit(1);

        if (@event.IsActionPressed("Lane2"))
            GameManager.instance.CheckLaneHit(2);

        if (@event.IsActionPressed("Lane3"))
            GameManager.instance.CheckLaneHit(3);
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
