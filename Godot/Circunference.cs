using Godot;
using System;

public partial class Circunference : GraphNode
{
    [Export]
    public Vector2 Center { get; set; }
    public float Radius { get; set; }
    public Godot.Color Color { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
