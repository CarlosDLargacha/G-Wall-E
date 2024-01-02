using Godot;
using System;

public partial class WButton : Button
{
	public void _on_pressed()
	{
        // Code to execute when the button is pressed.

        Control ParentNode = (Control)GetParent();

        HSplitContainer SiblingNode = (HSplitContainer)ParentNode.GetChild(1);

        GraphEdit graphEdit = (GraphEdit)SiblingNode.GetChild(1);

        graphEdit.CallDeferred("Create");
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
