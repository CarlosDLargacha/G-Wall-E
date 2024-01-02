using Godot;
using System;

public partial class WArc : GraphNode
{
    [Export]
    public Vector2 Center { get; set; }
    public float Distance { get; set; }
    public Godot.Color Color { get; set; }
    public float StartAngle { get; set; }
    public float EndAngle { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
