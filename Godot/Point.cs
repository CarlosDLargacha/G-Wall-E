using Godot;
using System;

public partial class Point : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		WPoint Parent = (WPoint)GetParent();
		this.SelfModulate = Parent.Color;
	}
}
