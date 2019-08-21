using Godot;
using System;

public class PlayerCamera : Camera2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	Node2D Parent;
    public override void _Ready()
    {
		Parent = (Node2D)GetNode("../Player");
		SetProcess(true);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		SetPosition(Parent.Position);
		
	}
}
