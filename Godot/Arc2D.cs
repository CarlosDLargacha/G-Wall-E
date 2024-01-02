using Godot;
using System;

public partial class Arc2D : Node2D
{
    public override void _Draw()
    {
        WArc parent = (WArc)GetParent();
        DrawArc(parent.Center, parent.Distance, parent.StartAngle, parent.EndAngle, 100, parent.Color);
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
