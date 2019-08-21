using Godot;
using System;

public class PhysicsParams : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	[Export]
	public float MaxGroundSpeed = 500f; //Ground x speed is clamped to this value
	[Export]
	public float MaxAirXSpeed = 500f; //Air x speed is clamped to this value
	[Export]
	public float maxAirYSpeed = 750f;
	[Export]
	public float groundAcc = 500f;
	[Export]
	public float AirAcc = 150f;
	[Export]
	public float Gravity = 60f;
	[Export]
	public float JumpVelocity = 900f; //The initial vertical velocty provided when jumping
	[Export]
	public float JumpReleaseDrag = 0.75f; //When the player releases the jump button, they slow down in order to be able to control the jump height
	[Export]
	public float GroundDrag = 0.6f; //Player is slowed down while they are not inputting a move
	[Export]
	public float AirDrag = 0.8f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
