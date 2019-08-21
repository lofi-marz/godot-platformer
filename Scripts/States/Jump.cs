using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Platformer.Scripts.States
{
	class Jump : State
	{
		bool finishedJump;
		public override void OnEnter()
		{
			var player = (PlayerCharacter)GetOwner();
			if (!player.IsOnFloor())
			{
				EmitSignal(nameof(Finished), "previous");
			}
			player.Velocity.y = -player.JumpVelocity;
			
			finishedJump = false;
		}

		public override void Update(float delta)
		{
			var player = (PlayerCharacter)GetOwner();

			if (finishedJump)
			{
	
				player.Velocity.y *= player.JumpReleaseDrag;
			}

			if (player.IsOnFloor())
			{
				EmitSignal(nameof(Finished), "idle");
			}
		}

		public override void HandleInput()
		{
			if (!Input.IsActionPressed("game_up"))
			{
				finishedJump = true;
				GD.Print("Jump done");
			}
			var isLeft = Input.IsActionPressed("game_left");
			var isRight = Input.IsActionPressed("game_right");
			GD.Print($"{isLeft}, {isRight}");
			var inputHDirection = (isLeft ? -1 : 0) + (isRight ? 1 : 0);
			if (inputHDirection != 0)
			{
				var player = (PlayerCharacter)GetOwner();
				var sprite = (AnimatedSprite)player.GetNode("./Sprite");
				sprite.FlipH = (inputHDirection < 0);
				player.Velocity.x += inputHDirection * player.AirAcc;
				player.Velocity.x = Maths.Clamp(player.Velocity.x, player.MaxAirXSpeed);
			}
			else
			{
				//EmitSignal(nameof(Finished), "idle");
			}

		}
	}
}
