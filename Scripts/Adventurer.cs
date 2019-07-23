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
		Dash,
		Run,
		Skid,
		GroundAttack,
		Crouch,
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
				prevState = _currentState;
				_currentState = value;
			}
		} }
	State _currentState;
	State prevState;
	float timer = 0f; //How long we have been in the current state for

	[Export]
	float maxGroundSpeed = 500f; //Ground x speed is clamped to this value
	float maxAirXSpeed = 500f; //Air x speed is clamped to this value
	float maxAirYSpeed = 750f;
	float groundAcc = 500f;
	float airAcc = 150f;
	float gravity = 60f; 
	float jumpVelocity = 900f; //The initial vertical velocty provided when jumping
	float jumpReleaseDrag = 0.75f; //When the player releases the jump button, they slow down in order to be able to control the jump height
	float groundDrag = 0.6f; //Player is slowed down while they are not inputting a move
	float airDrag = 0.8f;
	

	bool isGrounded;
	bool isJumping;
	bool finishedJump;
	bool finishedDash;

	int facingHDirection;
	int inputHDirection;
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
		timer += 1;

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
		UpdateDirection();
		if (inputHDirection != 0 && CheckStates(State.Skid, State.Stand))
		{
			CurrentState = State.Dash;
			DashState();
		}

		if (Input.IsActionJustPressed("game_up") && CheckStates(State.Dash, State.Run, State.Skid, State.Stand, State.Crouch))
		{
			CurrentState = State.Jump;
		}

		if (Input.IsActionJustPressed("game_attack") && CheckStates(State.Dash, State.Run, State.Skid, State.Stand))
		{
			CurrentState = State.GroundAttack;
		}

		if (Input.IsActionJustPressed("game_down") && CheckStates(State.Dash, State.Run, State.Skid, State.Stand))
		{
			CurrentState = State.Crouch;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		var velLabel = (RichTextLabel)GetNode("./Velocity");

		velLabel.Text = $"{CurrentState.ToString().PadRight(10, ' ')} {Velocity.x.ToString().PadDecimals(1).PadZeros(3):F1}, {Velocity.y.ToString().PadDecimals(1).PadZeros(3):F1}";
		
		
	}

	public void CycleStates() //Each state contains its own check for whether or not it should run, so we just cycle through them all
	{
		StandState();
		AirState();
		RunState();
		DashState();
		SkidState();
		JumpState();
		GroundAttackState();
		CrouchState();
	}
	public bool CheckStates(params State[] states)
	{
		foreach (var state in states)
		{
			if (CurrentState == state) return true;
		}
		return false;
	}
	public void StandState() //The player is standing still on the ground and not inputting anything
	{
		if (CurrentState != State.Stand) return;

		sprite.Play("idle");
		Velocity.y = 0f;
		Velocity.x *= groundDrag;
		Velocity.x = (float)Math.Round(Velocity.x, 2) == 0 ? (float)Math.Round(Velocity.x, 2) : Velocity.x;
		isJumping = false;
	}
	public void DashState()
	{
		finishedDash = false;
		if (CurrentState != State.Dash) return;

		sprite.Play("run");

		UpdateDirection();


		if (inputHDirection != 0)
		{
			sprite.FlipH = (inputHDirection < 0);

			Velocity.x += inputHDirection * groundAcc * 0.5f;
			if (Math.Abs(Velocity.x) > maxGroundSpeed)
			{
				Velocity.x = maxGroundSpeed * Math.Sign(Velocity.x);
			}
			CurrentState = State.Dash;
		}
		else
		{
			CurrentState = State.Skid;
		}




		if (timer > 5)
		{
			CurrentState = State.Run;
		}




	}
	public void AirState() //The player is in the air as a result of jumping or falling
	{
		if (CurrentState != State.Air) return;
		UpdateDirection();

		if (inputHDirection != 0)
		{
			Velocity.x += inputHDirection * airAcc;
			Velocity.x *= airDrag;

		}
		if (Velocity.y > maxAirYSpeed)
		{
			Velocity.y = maxAirYSpeed;
		}

		if (!Input.IsActionPressed("game_up"))
		{
			finishedJump = true;
		}
		/*if (Input.IsActionJustPressed("dodge"))
		{
			GD.Print("Dodge");
			var hDirection = (Input.IsActionPressed("game_left") ? -1 : 0) + (Input.IsActionPressed("game_right") ? 1 : 0);
			var vDirection = (Input.IsActionPressed("game_up") ? -1 : 0) + (Input.IsActionPressed("game_down") ? 1 : 0);
			Velocity += new Vector2(hDirection * 500f, vDirection * 500f);
		}*/
		if (isJumping && finishedJump && Velocity.y < 0)
		{
			Velocity.y *= jumpReleaseDrag;
		}
		Velocity.x = Maths.Clamp(Velocity.x, maxAirXSpeed);

		if (Velocity.y > 0)
		{
			sprite.Play("fall");
		}

	}
	public void RunState() //The player has inputted a run
	{
		if (CurrentState != State.Run) return;
		sprite.Play("run");
		UpdateDirection();

		if (inputHDirection != 0)
		{
			sprite.FlipH = (inputHDirection < 0);
			
			Velocity.x += inputHDirection * groundAcc;
			Velocity.x = Maths.Clamp(Velocity.x, maxGroundSpeed);

			CurrentState = State.Run;
		}
		else
		{
			CurrentState = State.Skid;
		}
	}
	public void CrouchState()
	{
		if (CurrentState != State.Crouch) return;
		sprite.Play("crouch");
		if (Input.IsActionPressed("game_down"))
		{
			Velocity.x *= groundDrag*0.9f;
		}
		else
		{
			CurrentState = State.Stand;
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
	public void GroundAttackState()
	{
		if (CurrentState != State.GroundAttack) return;
		sprite.Play("attack1");
		GD.Print(timer);
		if (timer > 12/12)
		{
			CurrentState = State.Stand;
		}

	}
	public void JumpState() //The character has inputted a jump
	{
		if (CurrentState != State.Jump) return;
		finishedJump = false;
		sprite.Play("jump");
		isJumping = true;
		
		if (isGrounded)
		{
			Velocity.y = -jumpVelocity;
		}


	}
	public void LandingState()
	{

	}
	
	public void UpdateDirection()
	{
		inputHDirection = (Input.IsActionPressed("game_left") ? -1 : 0) + (Input.IsActionPressed("game_right") ? 1 : 0);
		if (inputHDirection != 0)
		{
			facingHDirection = inputHDirection;
		}
	}


}
