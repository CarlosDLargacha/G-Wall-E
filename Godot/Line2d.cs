using Godot;
using System;

public partial class Line2d : Node2D
{
    public override void _Draw()
    {
        WLine parent = (WLine)GetParent();
        DrawLine(parent.Point1, parent.Point2, parent.Color, 2);  
    }
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
