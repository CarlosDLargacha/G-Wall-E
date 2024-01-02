using Godot;
using System;

public partial class WLine : GraphNode
{
    [Export]
    public Vector2 Point1 { get; set; }
	public Vector2 Point2 { get; set; }
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
