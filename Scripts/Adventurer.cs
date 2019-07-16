using Godot;
using Platformer.Scripts;
using System;

public class Adventurer : KinematicBody2D
{
	public enum State
	{
		Stand,
		Jump,
		JumpSquat,
		Landing,
		FreeFall,
		Air,
		Run,
		Skid,
	}
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	AnimatedSprite sprite;


	Vector2 Velocity;
	State CurrentState {
		get { return _currentState; }
		set {
			if (value != _currentState)
			{
				timer = 0;
				_currentState = value;
			}
		} }
	State _currentState;
	float timer = 0f; //How long we have been in the current state for

	float maxGroundSpeed = 500f; //Ground x speed is clamped to this value
	float maxAirSpeed = 400f; //Air x speed is clamped to this value
	float groundAcc = 500f;
	float airAcc = 200f;
	float gravity = 60f; 
	float jumpVelocity = 900f; //The initial vertical velocty provided when jumping
	float jumpReleaseDrag = 0.75f; //When the player releases the jump button, they slow down in order to be able to control the jump height
	float groundDrag = 0.8f; //Player is slowed down while they are not inputting a move
	

	bool isGrounded;
	bool isJumping;
	bool continuingJump;
	int jumpsLeft = 1;
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
    {

		CurrentState = State.Air;
		sprite = (AnimatedSprite)GetNode("AnimatedSprite");


	}


	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		timer += delta;

		CycleStates();

		Velocity.y += gravity; //Always apply gravity
		Velocity = MoveAndSlide(Velocity, new Vector2(0, -1)); //Delta is already taken into account
		isGrounded = IsOnFloor();
		if (CurrentState == State.Air && isGrounded)
		{
			CurrentState = State.Stand;

		} else if (!isGrounded)
		{
			CurrentState = State.Air;

		}

		//Cancelling one of the ground states into a run
		var hDirection = (Input.IsActionPressed("game_left") ? -1 : 0) + (Input.IsActionPressed("game_right") ? 1 : 0);
		if (hDirection != 0 && CheckStates(State.Skid, State.Stand))
		{
			CurrentState = State.Run;
			RunState();
		}

		if (Input.IsActionPressed("game_up") && CheckStates(State.Run, State.Skid, State.Stand))
		{
			CurrentState = State.Jump;
		}


	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		var velLabel = (RichTextLabel)GetNode("./Velocity");

		velLabel.Text = $"{CurrentState} {Velocity.x:F2}, {Velocity.y:F2}";
		
		
	}

	public void CycleStates() //Each state contains its own check for whether or not it should run, so we just cycle through them all
	{
		StandState();
		AirState();
		RunState();
		SkidState();
		JumpState();
	}
	public void StandState() //The player is standing still on the ground and not inputting anything
	{
		if (CurrentState != State.Stand) return;
		sprite.Play("idle");
		Velocity.y = 0f;
		Velocity.x *= groundDrag;
		isJumping = false;
	}

	public void AirState() //The player is in the air as a result of jumping or falling
	{
		if (CurrentState != State.Air) return;
		var direction = (Input.IsActionPressed("game_left")?-1:0) + (Input.IsActionPressed("game_right")?1:0);

		if (direction != 0)
		{
			Velocity.x += direction * airAcc;
			Velocity.x *= groundDrag;
		}
		if (Input.IsActionJustReleased("move_up"))
		{
			continuingJump = false;
		}
		if (isJumping && !continuingJump && Velocity.y < 0)
		{
			Velocity.y *= jumpReleaseDrag;
		}
		if (Math.Abs(Velocity.x) > maxAirSpeed)
		{
			Velocity.x = maxAirSpeed * Math.Sign(Velocity.x);
		}

	}

	public void RunState() //The player has inputted a run
	{
		if (CurrentState != State.Run) return;
		sprite.Play("run");
		var direction = (Input.IsActionPressed("game_left") ? -1 : 0) + (Input.IsActionPressed("game_right") ? 1 : 0);

		if (direction != 0)
		{
			sprite.FlipH = (direction < 0);
			
			Velocity.x += direction * groundAcc;
			if (Math.Abs(Velocity.x) > maxGroundSpeed)
			{
				Velocity.x = maxGroundSpeed * Math.Sign(Velocity.x);
			}
			CurrentState = State.Run;
		}
		else
		{
			CurrentState = State.Skid;

		}
	}

	public void SkidState() //The character has ended a run
	{
		if (CurrentState != State.Skid) return;
		sprite.Play("idle");
		Velocity.x *= groundDrag;
		if (Math.Abs(Math.Round(Velocity.x, 2)) == 0f)
		{
			Velocity.x = 0;
			CurrentState = State.Stand;

		}

	}

	public void JumpState() //The character has inputted a jump
	{
		if (CurrentState != State.Jump) return;
		
		sprite.Play("jump");
		isJumping = true;
		continuingJump = true;
		if (isGrounded)
		{
			Velocity.y = -jumpVelocity;
		}


	}

	public void LandingState()
	{

	}
	public bool CheckStates(params State[] states)
	{
		foreach (var state in states)
		{
			if (CurrentState == state) return true;
		}
		return false;
	}
	public class PlayerMover



}
