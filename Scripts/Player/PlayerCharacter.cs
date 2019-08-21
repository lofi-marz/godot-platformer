using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer.Scripts
{

	public class PlayerCharacter : KinematicBody2D
	{
		public Vector2 Velocity;
		


		[Export]
		public float MaxGroundSpeed = 500f; //Ground x speed is clamped to this value
		[Export]
		public float MaxAirXSpeed = 500f; //Air x speed is clamped to this value
		[Export]
		public float MaxAirYSpeed = 750f;
		[Export]
		public float GroundAcc = 500f;
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

		static void GetInputDirection()
		{

		}
		public override void _Ready()
		{
			base._Ready();



		}

		public override void _PhysicsProcess(float delta)
		{
			base._PhysicsProcess(delta);
            Velocity.y += Gravity; //Always apply gravity
            Velocity = MoveAndSlide(Velocity, new Vector2(0, -1)); //Delta is already taken into account
            var velLabel = (RichTextLabel)GetNode("./Velocity");
			var StateMachine = (PlayerStateMachine)GetNode("PlayerStateMachine");

			if (!StateMachine.Active) return;
			
			velLabel.Text = $"{StateMachine.CurrentState.ToString().PadRight(10, ' ')} {Velocity.x.ToString().PadDecimals(1).PadZeros(3):F1}, {Velocity.y.ToString().PadDecimals(1).PadZeros(3):F1}";
		}

		public void Jump()
		{

		}

		public void HandleRunInput()
		{

		}
	}
}
